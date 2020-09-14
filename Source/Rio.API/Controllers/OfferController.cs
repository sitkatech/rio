using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Rio.API.Services;
using Rio.API.Services.Authorization;
using Rio.EFModels.Entities;
using Rio.Models.DataTransferObjects.Account;
using Rio.Models.DataTransferObjects.Offer;
using Rio.Models.DataTransferObjects.Posting;
using Rio.Models.DataTransferObjects.User;
using Rio.Models.DataTransferObjects.WaterTransfer;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;

namespace Rio.API.Controllers
{
    [ApiController]
    public class OfferController : ControllerBase
    {
        private readonly RioDbContext _dbContext;
        private readonly ILogger<OfferController> _logger;
        private readonly KeystoneService _keystoneService;
        private readonly RioConfiguration _rioConfiguration;    

        public OfferController(RioDbContext dbContext, ILogger<OfferController> logger, KeystoneService keystoneService, IOptions<RioConfiguration> rioConfiguration)
        {
            _dbContext = dbContext;
            _logger = logger;
            _keystoneService = keystoneService;
            _rioConfiguration = rioConfiguration.Value;
        }

        [HttpPost("/offers/new/{postingID}")]
        [OfferManageFeature]
        public IActionResult New([FromRoute] int postingID, [FromBody] OfferUpsertDto offerUpsertDto)
        {
            if (!_rioConfiguration.ALLOW_TRADING)
            {
                return BadRequest();
            }

            var posting = Posting.GetByPostingID(_dbContext, postingID);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if ((posting.PostingType.PostingTypeID == (int) PostingTypeEnum.OfferToSell) && (offerUpsertDto.OfferStatusID == (int) OfferStatusEnum.Pending || offerUpsertDto.OfferStatusID == (int)OfferStatusEnum.Accepted) && posting.AvailableQuantity < offerUpsertDto.Quantity)
            {
                ModelState.AddModelError("Quantity", "Exceeds remaining balance in posting");
                return BadRequest(ModelState);
            }

            if (offerUpsertDto.OfferStatusID != (int) OfferStatusEnum.Rescinded && Posting.HasOpenOfferByMe(_dbContext, posting, offerUpsertDto.CreateAccountID))
            {
                ModelState.AddModelError("Posting", "You currently have an open offer on this posting. Please wait until the other party responds to the current offer.");
                return BadRequest(ModelState);
            }

            var userDto = GetCurrentUser();
            var offer = Offer.CreateNew(_dbContext, postingID, offerUpsertDto);
            var smtpClient = HttpContext.RequestServices.GetRequiredService<SitkaSmtpClientService>();
            var currentTrade = Trade.GetByTradeID(_dbContext, offer.TradeID);
            var rioUrl = _rioConfiguration.WEB_URL;

            // update trades status if needed
            switch (offerUpsertDto.OfferStatusID)
            {
                case (int)OfferStatusEnum.Accepted:
                    var tradeDto = Trade.Update(_dbContext, offer.TradeID, TradeStatusEnum.Accepted);
                    // write a water transfer record
                    var waterTransfer = WaterTransfer.CreateNew(_dbContext, offer, tradeDto, posting);
                    var mailMessages = GenerateAcceptedOfferEmail(rioUrl, offer, currentTrade, posting, waterTransfer, smtpClient);
                    foreach (var mailMessage in mailMessages)
                    {
                        SendEmailMessage(smtpClient, mailMessage);
                    }
                    break;
                case (int)OfferStatusEnum.Rejected:
                    UpdateTradeStatusSendEmail(offer, smtpClient, GenerateRejectedOfferEmail(rioUrl, offer, currentTrade, posting, smtpClient), TradeStatusEnum.Rejected);
                    break;
                case (int)OfferStatusEnum.Rescinded:
                    UpdateTradeStatusSendEmail(offer, smtpClient, GenerateRescindedOfferEmail(rioUrl, offer, currentTrade, posting, smtpClient), TradeStatusEnum.Rescinded);
                    break;
                default:
                    SendEmailMessage(smtpClient, GeneratePendingOfferEmail(rioUrl, currentTrade, offer, posting, smtpClient));
                    break;
            }

            // get current balance of posting
            var acreFeetOfAcceptedTrades = Posting.CalculateAcreFeetOfAcceptedTrades(_dbContext, postingID);
            var postingStatusToUpdateTo = (int) PostingStatusEnum.Open;
            if (posting.Quantity == acreFeetOfAcceptedTrades)
            {
                postingStatusToUpdateTo = (int)PostingStatusEnum.Closed;
                // expire all other outstanding offers
                var postingCreateAccountID = posting.CreateAccount.AccountID;
                var activeTradesForPosting = Trade.GetPendingTradesForPostingID(_dbContext, postingID).ToList();
                foreach (var activeTrade in activeTradesForPosting)
                {
                    var offerStatus = activeTrade.OfferCreateAccount.AccountID == postingCreateAccountID
                        ? OfferStatusEnum.Rescinded
                        : OfferStatusEnum.Rejected;
                    var offerUpsertDtoForRescindReject = new OfferUpsertDto
                    {
                        TradeID = activeTrade.TradeID,
                        Price = activeTrade.Price,
                        CreateAccountID = postingCreateAccountID,
                        Quantity = activeTrade.Quantity,
                        OfferStatusID = (int) offerStatus,
                        OfferNotes = $"Offer {offerStatus} because original posting is now closed"
                    };
                    var resultingOffer = Offer.CreateNew(_dbContext, postingID, offerUpsertDtoForRescindReject);
                    switch (offerStatus)
                    {
                        case OfferStatusEnum.Rejected:
                            UpdateTradeStatusSendEmail(resultingOffer, smtpClient, GenerateRejectedOfferEmail(rioUrl, resultingOffer, Trade.GetByTradeID(_dbContext, activeTrade.TradeID), posting, smtpClient), TradeStatusEnum.Rejected);
                            break;
                        case OfferStatusEnum.Rescinded:
                            UpdateTradeStatusSendEmail(resultingOffer, smtpClient, GenerateRescindedOfferEmail(rioUrl, resultingOffer, Trade.GetByTradeID(_dbContext, activeTrade.TradeID), posting, smtpClient), TradeStatusEnum.Rescinded);
                            break;
                    }

                }
            }
            Posting.UpdateStatus(_dbContext, postingID,
                new PostingUpdateStatusDto { PostingStatusID = postingStatusToUpdateTo }, posting.Quantity - acreFeetOfAcceptedTrades);

            return Ok(offer);
        }

        private void UpdateTradeStatusSendEmail(OfferDto offer, SitkaSmtpClientService smtpClient, MailMessage mailMessage,
            TradeStatusEnum updatedStatus)
        {
            Trade.Update(_dbContext, offer.TradeID, updatedStatus);
            SendEmailMessage(smtpClient, mailMessage);
        }

        private void SendEmailMessage(SitkaSmtpClientService smtpClient, MailMessage mailMessage)
        {
            mailMessage.IsBodyHtml = true;
            mailMessage.From = smtpClient.GetDefaultEmailFrom();
            if (! string.IsNullOrWhiteSpace(_rioConfiguration.LeadOrganizationEmail))
            {
                mailMessage.ReplyToList.Add(_rioConfiguration.LeadOrganizationEmail);
            }
            SitkaSmtpClientService.AddBccRecipientsToEmail(mailMessage, EFModels.Entities.User.GetEmailAddressesForAdminsThatReceiveSupportEmails(_dbContext));
            smtpClient.Send(mailMessage);
        }

        private static List<MailMessage> GenerateAcceptedOfferEmail(string rioUrl, OfferDto offer,
            TradeDto currentTrade, PostingDto posting, WaterTransferDto waterTransfer, SitkaSmtpClientService smtpClient)
        {
            AccountDto buyer;
            AccountDto seller;
            if (currentTrade.CreateAccount.AccountID == posting.CreateAccount.AccountID)
            {
                if (posting.PostingType.PostingTypeID == (int)PostingTypeEnum.OfferToBuy)
                {
                    buyer = posting.CreateAccount;
                    seller = currentTrade.CreateAccount;
                }
                else
                {
                    buyer = currentTrade.CreateAccount;
                    seller = posting.CreateAccount;
                }
            }
            else
            {
                if (posting.PostingType.PostingTypeID == (int)PostingTypeEnum.OfferToBuy)
                {
                    buyer = posting.CreateAccount;
                    seller = currentTrade.CreateAccount;
                }
                else
                {
                    buyer = currentTrade.CreateAccount;
                    seller = posting.CreateAccount;
                }
            }


            var mailMessages = new List<MailMessage>();
            var messageBody = $@"Your offer to trade water has been accepted.
<ul>
    <li><strong>Buyer:</strong> {buyer.AccountName} ({string.Join(", ",  buyer.Users.Select(x=>x.Email))})</li>
    <li><strong>Seller:</strong> {seller.AccountName} ({string.Join(", ", seller.Users.Select(x => x.Email))})</li>
    <li><strong>Quantity:</strong> {offer.Quantity} acre-feet</li>
    <li><strong>Unit Price:</strong> {offer.Price:$#,##0.00} per acre-foot</li>
    <li><strong>Total Price:</strong> {(offer.Price * offer.Quantity):$#,##0.00}</li>
</ul>
To finalize this transaction, the buyer and seller must complete payment and any other terms of the transaction. Once payment is complete, the trade must be confirmed by both parties within the Water Accounting Platform before the district will recognize the transfer.
<br /><br />
<a href=""{rioUrl}/register-transfer/{waterTransfer.WaterTransferID}"">Confirm Transfer</a>
{smtpClient.GetDefaultEmailSignature()}";
            var mailTos = (new List<AccountDto> {buyer, seller}).SelectMany(x=>x.Users);
            foreach (var mailTo in mailTos)
            {
                var mailMessage = new MailMessage
                {
                    Subject = $"Trade {currentTrade.TradeNumber} Accepted",
                    Body = $"Hello {mailTo.FullName},<br /><br />{messageBody}"
                };
                mailMessage.To.Add(new MailAddress(mailTo.Email, mailTo.FullName));
                mailMessages.Add(mailMessage);
            }
            return mailMessages;
        }

        private static MailMessage GenerateRejectedOfferEmail(string rioUrl, OfferDto offer,
            TradeDto currentTrade,
            PostingDto posting, SitkaSmtpClientService smtpClient)
        {
            var offerAction = currentTrade.CreateAccount.AccountID == offer.CreateAccount.AccountID
                ? posting.PostingType.PostingTypeID == (int)PostingTypeEnum.OfferToBuy ? "buy" : "sell"
                : posting.PostingType.PostingTypeID == (int)PostingTypeEnum.OfferToBuy ? "sell" : "buy";

            var toAccount = offer.CreateAccount.AccountID == posting.CreateAccount.AccountID ? currentTrade.CreateAccount : posting.CreateAccount;
            var fromAccount = offer.CreateAccount.AccountID == posting.CreateAccount.AccountID
                ? posting.CreateAccount
                : currentTrade.CreateAccount;

            var properPreposition = offerAction == "sell" ? "to" : "from";
            var messageBody =
                $@"
Hello,
<br /><br />
Your offer to {offerAction} water {properPreposition} Account #{fromAccount.AccountNumber} ({fromAccount.AccountName}) was rejected by the other party. You can see details of your transactions in the Water Accounting Platform Landowner Dashboard. 
<br /><br />
<a href=""{rioUrl}/landowner-dashboard"">View Landowner Dashboard</a>
{smtpClient.GetDefaultEmailSignature()}";
            var mailMessage = new MailMessage
            {
                Subject = $"Trade {currentTrade.TradeNumber} Rejected",
                Body = messageBody
            };
            foreach (var user in toAccount.Users)
            {
                mailMessage.To.Add(new MailAddress(user.Email, user.FullName));
            }
            return mailMessage;
        }

        private static MailMessage GenerateRescindedOfferEmail(string rioUrl, OfferDto offer,
            TradeDto currentTrade,
            PostingDto posting, SitkaSmtpClientService smtpClient)
        {
            var offerAction = currentTrade.CreateAccount.AccountID == offer.CreateAccount.AccountID
                ? posting.PostingType.PostingTypeID == (int)PostingTypeEnum.OfferToBuy ? "sell" : "buy"
                : posting.PostingType.PostingTypeID == (int)PostingTypeEnum.OfferToBuy ? "buy" : "sell";

            var toAccount = offer.CreateAccount.AccountID == posting.CreateAccount.AccountID ? currentTrade.CreateAccount : posting.CreateAccount;
            var fromAccount = offer.CreateAccount.AccountID == posting.CreateAccount.AccountID
                ? posting.CreateAccount
                : currentTrade.CreateAccount;

            var properPreposition = offerAction == "sell" ? "to" : "from";
            var messageBody =
                $@"
Hello,
<br /><br />
An offer to {offerAction} water {properPreposition} Account #{fromAccount.AccountNumber} ({fromAccount.AccountName}) was rescinded by the other party. You can see details of your transactions in the Water Accounting Platform Landowner Dashboard. 
<br /><br />
<a href=""{rioUrl}/landowner-dashboard"">View Landowner Dashboard</a>
{smtpClient.GetDefaultEmailSignature()}";
            var mailMessage = new MailMessage
            {
                Subject = $"Trade {currentTrade.TradeNumber} Rescinded",
                Body = messageBody
            };
            foreach (var user in toAccount.Users)
            {
                mailMessage.To.Add(new MailAddress(user.Email, user.FullName));
            }
            return mailMessage;
        }

        private static MailMessage GeneratePendingOfferEmail(string rioUrl, TradeDto currentTrade,
            OfferDto offer, PostingDto posting, SitkaSmtpClientService smtpClient)
        {
            var offerAction = currentTrade.CreateAccount.AccountID == offer.CreateAccount.AccountID
                ? posting.PostingType.PostingTypeID == (int)PostingTypeEnum.OfferToBuy ? "sell" : "buy"
                : posting.PostingType.PostingTypeID == (int)PostingTypeEnum.OfferToBuy ? "buy" : "sell";
            var toAccount = offer.CreateAccount.AccountID == posting.CreateAccount.AccountID ? currentTrade.CreateAccount : posting.CreateAccount;
            var fromAccount = offer.CreateAccount.AccountID == posting.CreateAccount.AccountID
                ? posting.CreateAccount
                : currentTrade.CreateAccount;

            var properPreposition = offerAction == "sell" ? "to" : "from";
            var messageBody =
                $@"
Hello,
<br /><br />
An offer to {offerAction} water {properPreposition} Account #{fromAccount.AccountNumber} ({fromAccount.AccountName}) has been presented for your review. 
<br /><br />
<a href=""{rioUrl}/trades/{currentTrade.TradeNumber}"">Respond to this offer</a>
{smtpClient.GetDefaultEmailSignature()}";
            var mailMessage = new MailMessage
            {
                Subject = "New offer to review",
                Body = messageBody
            };
            foreach (var user in toAccount.Users)
            {
                mailMessage.To.Add(new MailAddress(user.Email, user.FullName));
            }
            return mailMessage;
        }

        [HttpGet("current-user-active-offers/{postingID}")]
        [OfferManageFeature]
        public ActionResult<IEnumerable<OfferDto>> GetActiveOffersForCurrentUserByPosting([FromRoute] int postingID)
        {
            var userDto = GetCurrentUser();
            var offerDtos = Offer.GetActiveOffersFromPostingIDAndUserID(_dbContext, postingID, userDto.UserID);
            return Ok(offerDtos);
        }

        [HttpGet("current-account-active-offers/{accountID}/{postingID}")]
        [OfferManageFeature]
        public ActionResult<IEnumerable<OfferDto>> GetActiveOffersForCurrentAccountByPosting([FromRoute] int accountID, [FromRoute] int postingID)
        {
            var offerDtos = Offer.GetActiveOffersFromPostingIDAndCreateAccountID(_dbContext, postingID, accountID);
            return Ok(offerDtos);
        }

        [HttpGet("all-trade-activity/{year}")]
        [UserViewFeature]
        public ActionResult<IEnumerable<TradeWithMostRecentOfferDto>> GetTradeActivity([FromRoute] int year)
        {
            var tradeWithMostRecentOfferDtos = Trade.GetTradesForYear(_dbContext, year);
            return Ok(tradeWithMostRecentOfferDtos);
        }

        [HttpGet("trade-activity/{accountID}")]
        [UserViewFeature]
        public ActionResult<IEnumerable<TradeWithMostRecentOfferDto>> GetTradeActivityForUser([FromRoute] int accountID)
        {
            var tradeWithMostRecentOfferDtos = Trade.GetTradesForAccountID(_dbContext, accountID);
            return Ok(tradeWithMostRecentOfferDtos);
        }

        [HttpGet("trades/{tradeNumber}")]
        [OfferManageFeature]
        public ActionResult<TradeDto> GetTradeByTradeNumber([FromRoute] string tradeNumber)
        {
            var tradeDto = Trade.GetByTradeNumber(_dbContext, tradeNumber);
            return Ok(tradeDto);
        }

        [HttpGet("trades/{tradeNumber}/offers")]
        [OfferManageFeature]
        public ActionResult<IEnumerable<OfferDto>> GetOffersByTradeNumber([FromRoute] string tradeNumber)
        {
            var offerDtos = Offer.GetByTradeNumber(_dbContext, tradeNumber);
            return Ok(offerDtos);
        }

        [HttpGet("offers/{offerID}")]
        [OfferManageFeature]
        public ActionResult<OfferDto> GetByOfferID([FromRoute] int offerID)
        {
            var offerDto = Offer.GetByOfferID(_dbContext, offerID);
            if (offerDto == null)
            {
                return NotFound();
            }

            return Ok(offerDto);
        }

        private UserDto GetCurrentUser()
        {
            var userGuid = _keystoneService.GetProfile().Payload.UserGuid;
            var userDto = Rio.EFModels.Entities.User.GetByUserGuid(_dbContext, userGuid);
            return userDto;
        }
    }
}