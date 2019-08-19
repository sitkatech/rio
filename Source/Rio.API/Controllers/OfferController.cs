﻿using System.Collections.Generic;
using System.Net.Mail;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Rio.API.Services;
using Rio.API.Services.Authorization;
using Rio.API.Services.Filter;
using Rio.EFModels.Entities;
using Rio.Models.DataTransferObjects.Offer;
using Rio.Models.DataTransferObjects.Posting;
using Rio.Models.DataTransferObjects.User;

namespace Rio.API.Controllers
{
    public class OfferController : Controller
    {
        private readonly RioDbContext _dbContext;
        private readonly ILogger<OfferController> _logger;
        private readonly KeystoneService _keystoneService;
        private readonly string _rioWebUrl;

        public OfferController(RioDbContext dbContext, ILogger<OfferController> logger, KeystoneService keystoneService, IOptions<RioConfiguration> rioConfigurationOptions)
        {
            _dbContext = dbContext;
            _logger = logger;
            _keystoneService = keystoneService;
            _rioWebUrl = rioConfigurationOptions.Value.RIO_WEB_URL;
        }

        [HttpPost("/offers/new/{postingID}")]
        [OfferManageFeature]
        [RequiresValidJSONBodyFilter("Could not parse a valid Offer New JSON object from the Request Body.")]
        public IActionResult New([FromRoute] int postingID, [FromBody] OfferUpsertDto offerUpsertDto)
        {
            var posting = Posting.GetByPostingID(_dbContext, postingID);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if ((posting.PostingType.PostingTypeID == (int) PostingTypeEnum.OfferToSell) && (posting.AvailableQuantity < offerUpsertDto.Quantity))
            {
                ModelState.AddModelError("Quantity", "Exceeds remaining balance in posting");
                return BadRequest(ModelState);
            }

            var userDto = GetCurrentUser();
            var offer = Offer.CreateNew(_dbContext, postingID, userDto.UserID, offerUpsertDto);
            var smtpClient = HttpContext.RequestServices.GetRequiredService<SitkaSmtpClientService>();
            var currentTrade = Trade.GetByTradeID(_dbContext, offer.TradeID);
            var rioUrl = _rioWebUrl;

            // update trades status if needed
            if (offerUpsertDto.OfferStatusID == (int)OfferStatusEnum.Accepted)
            {
                var tradeDto = Trade.Update(_dbContext, offer.TradeID, TradeStatusEnum.Accepted);
                // write a water transfer record
                WaterTransfer.CreateNew(_dbContext, offer, tradeDto, posting);
                var mailMessages = GenerateAcceptedOfferEmail(rioUrl, offer, currentTrade, posting);
                foreach (var mailMessage in mailMessages)
                {
                    SendEmailMessage(smtpClient, mailMessage);
                }
            }
            else if (offerUpsertDto.OfferStatusID == (int)OfferStatusEnum.Rejected)
            {
                Trade.Update(_dbContext, offer.TradeID, TradeStatusEnum.Rejected);
                var mailMessage = GenerateRejectedOfferEmail(rioUrl, offer, currentTrade, posting);
                SendEmailMessage(smtpClient, mailMessage);
            }
            else if (offerUpsertDto.OfferStatusID == (int)OfferStatusEnum.Rescinded)
            {
                Trade.Update(_dbContext, offer.TradeID, TradeStatusEnum.Rescinded);
                var mailMessage = GenerateRescindedOfferEmail(rioUrl, offer, currentTrade, posting);
                SendEmailMessage(smtpClient, mailMessage);
            }
            else
            {
                var mailMessage = GeneratePendingOfferEmail(rioUrl, currentTrade, offer, posting);
                SendEmailMessage(smtpClient, mailMessage);
            }

            // get current balance of posting
            var acreFeetOfAcceptedTrades = Posting.CalculateAcreFeetOfAcceptedTrades(_dbContext, postingID);
            var postingStatusToUpdateTo = (int) PostingStatusEnum.Open;
            if (posting.Quantity == acreFeetOfAcceptedTrades)
            {
                postingStatusToUpdateTo = (int)PostingStatusEnum.Closed;
                // expire all other outstanding offers
                var postingCreateUserID = posting.CreateUser.UserID;
                var activeTradesForPosting = Trade.GetPendingTradesForPostingID(_dbContext, postingID);
                foreach (var activeTrade in activeTradesForPosting)
                {
                    var offerStatus = activeTrade.OfferCreateUserID == postingCreateUserID
                        ? OfferStatusEnum.Rescinded
                        : OfferStatusEnum.Rejected;
                    var offerUpsertDtoForRescindReject = new OfferUpsertDto();
                    offerUpsertDtoForRescindReject.TradeID = activeTrade.TradeID;
                    offerUpsertDtoForRescindReject.Price = activeTrade.Price;
                    offerUpsertDtoForRescindReject.Quantity = activeTrade.Quantity;
                    offerUpsertDtoForRescindReject.OfferStatusID = (int)offerStatus;
                    offerUpsertDtoForRescindReject.OfferNotes = $"Offer {offerStatus} because original posting is now closed";
                    Offer.CreateNew(_dbContext, postingID, postingCreateUserID, offerUpsertDtoForRescindReject);
                }
            }
            Posting.UpdateStatus(_dbContext, postingID,
                new PostingUpdateStatusDto { PostingStatusID = postingStatusToUpdateTo }, posting.Quantity - acreFeetOfAcceptedTrades);

            return Ok(offer);
        }

        private void SendEmailMessage(SitkaSmtpClientService smtpClient, MailMessage mailMessage)
        {
            mailMessage.IsBodyHtml = true;
            mailMessage.From = GetDefaultEmailFrom();
            AddReplyToEmail(mailMessage);
            AddAdminsAsBccRecipientsToEmail(mailMessage);
            smtpClient.Send(mailMessage);
        }

        private static List<MailMessage> GenerateAcceptedOfferEmail(string rioUrl, OfferDto offer, TradeDto currentTrade, PostingDto posting)
        {
            UserSimpleDto buyer;
            UserSimpleDto seller;
            if (currentTrade.CreateUser.UserID == posting.CreateUser.UserID)
            {
                if (posting.PostingType.PostingTypeID == (int)PostingTypeEnum.OfferToBuy)
                {
                    buyer = posting.CreateUser;
                    seller = currentTrade.CreateUser;
                }
                else
                {
                    buyer = currentTrade.CreateUser;
                    seller = posting.CreateUser;
                }
            }
            else
            {
                if (posting.PostingType.PostingTypeID == (int)PostingTypeEnum.OfferToBuy)
                {
                    buyer = posting.CreateUser;
                    seller = currentTrade.CreateUser;
                }
                else
                {
                    buyer = currentTrade.CreateUser;
                    seller = posting.CreateUser;
                }
            }

            var mailMessages = new List<MailMessage>();
            var messageBody = $@"Your offer to trade water has been accepted.
<ul>
    <li><strong>Buyer:</strong> {buyer.FullName} ({buyer.Email})</li>
    <li><strong>Seller:</strong> {seller.FullName} ({seller.Email})</li>
    <li><strong>Quantity:</strong> {offer.Quantity} acre-feet</li>
    <li><strong>Unit Price:</strong> {offer.Price:$#,##0.00} per acre-foot</li>
    <li><strong>Total Price:</strong> {(offer.Price * offer.Quantity):$#,##0.00}</li>
</ul>
To finalize this transaction, the buyer and seller must complete payment and any other terms of the transaction. Once payment is complete, the trade must be confirmed by both parties within the Water Trading Platform before the district will recognize the transfer.
<br /><br />
<a href=""{rioUrl}/confirm-transfer/{currentTrade.TradeID}"">Confirm Transfer</a>
{GetDefaultEmailSignature()}";
            var mailTos = new List<UserSimpleDto> {buyer, seller};
            foreach (var mailTo in mailTos)
            {
                var mailMessage = new MailMessage
                {
                    Subject = "Trade Accepted",
                    Body = $"Hello {mailTo.FullName},<br /><br />{messageBody}"
                };
                mailMessage.To.Add(new MailAddress(mailTo.Email, mailTo.FullName));
                mailMessages.Add(mailMessage);
            }
            return mailMessages;
        }

        private static MailMessage GenerateRejectedOfferEmail(string rioUrl, OfferDto offer,
            TradeDto currentTrade,
            PostingDto posting)
        {
            var offerAction = currentTrade.CreateUser.UserID == offer.CreateUser.UserID
                ? posting.PostingType.PostingTypeID == (int)PostingTypeEnum.OfferToBuy ? "buy" : "sell"
                : posting.PostingType.PostingTypeID == (int)PostingTypeEnum.OfferToBuy ? "sell" : "buy";

            var toUser = offer.CreateUser.UserID == posting.CreateUser.UserID ? currentTrade.CreateUser : posting.CreateUser;
            var messageBody =
                $@"
Hello {toUser.FullName},
<br /><br />
Your offer to {offerAction} water was rejected by the other party. You can see details of your transactions in the Water Trading Platform Landowner Dashboard. 
<br /><br />
<a href=""{rioUrl}/landowner-dashboard"">View Landowner Dashboard</a>
{GetDefaultEmailSignature()}";
            var mailMessage = new MailMessage
            {
                Subject = "Trade Rejected",
                Body = messageBody
            };
            mailMessage.To.Add(new MailAddress(toUser.Email, toUser.FullName));
            return mailMessage;
        }

        private static MailMessage GenerateRescindedOfferEmail(string rioUrl, OfferDto offer,
            TradeDto currentTrade,
            PostingDto posting)
        {
            var offerAction = currentTrade.CreateUser.UserID == offer.CreateUser.UserID
                ? posting.PostingType.PostingTypeID == (int)PostingTypeEnum.OfferToBuy ? "sell" : "buy"
                : posting.PostingType.PostingTypeID == (int)PostingTypeEnum.OfferToBuy ? "buy" : "sell";

            var toUser = offer.CreateUser.UserID == posting.CreateUser.UserID ? currentTrade.CreateUser : posting.CreateUser;
            var messageBody =
                $@"
Hello {toUser.FullName},
<br /><br />
An offer to {offerAction} water was rescinded by the other party. You can see details of your transactions in the Water Trading Platform Landowner Dashboard. 
<br /><br />
<a href=""{rioUrl}/landowner-dashboard"">View Landowner Dashboard</a>
{GetDefaultEmailSignature()}";
            var mailMessage = new MailMessage
            {
                Subject = "Trade Rescinded",
                Body = messageBody
            };
            mailMessage.To.Add(new MailAddress(toUser.Email, toUser.FullName));
            return mailMessage;
        }

        private static MailMessage GeneratePendingOfferEmail(string rioUrl, TradeDto currentTrade,
            OfferDto offer, PostingDto posting)
        {
            var offerAction = currentTrade.CreateUser.UserID == offer.CreateUser.UserID
                ? posting.PostingType.PostingTypeID == (int)PostingTypeEnum.OfferToBuy ? "sell" : "buy"
                : posting.PostingType.PostingTypeID == (int)PostingTypeEnum.OfferToBuy ? "buy" : "sell";
            var toUser = offer.CreateUser.UserID == posting.CreateUser.UserID ? currentTrade.CreateUser : posting.CreateUser;
            var messageBody =
                $@"
Hello {toUser.FullName},
<br /><br />
An offer to {offerAction} water has been presented for your review. 
<br /><br />
<a href=""{rioUrl}/trades/{currentTrade.TradeID}"">Respond to this offer</a>
{GetDefaultEmailSignature()}";
            var mailMessage = new MailMessage
            {
                Subject = "New offer to review",
                Body = messageBody
            };
            mailMessage.To.Add(new MailAddress(toUser.Email, toUser.FullName));
            return mailMessage;
        }

        private static void AddReplyToEmail(MailMessage mailMessage)
        {
            mailMessage.ReplyToList.Add("admin@rrbwsd.com");
        }

        private void AddAdminsAsBccRecipientsToEmail(MailMessage mailMessage)
        {
            var admins = Rio.EFModels.Entities.User.ListByRole(_dbContext, RoleEnum.Admin);
            foreach (var admin in admins)
            {
                mailMessage.Bcc.Add(admin.Email);
            }
        }

        private static string GetDefaultEmailSignature()
        {
            const string defaultEmailSignature = @"<br /><br />
Respectfully, the RRB WSD Water Trading Platform team
<br /><br />
***
<br /><br />
You have received this email because you are a registered user of the Rosedale-Rio Bravo WSD Water Trading Platform. 
<br /><br />
P.O. Box 20820<br />
Bakersfield, CA 93390-0820<br />
Phone: (661) 589-6045<br />
<a href=""mailto:admin@rrbwsd.com"">admin@rrbwsd.com</a>";
            return defaultEmailSignature;
        }

        private static MailAddress GetDefaultEmailFrom()
        {
            return new MailAddress("donotreply@sitkatech.com", "RRB Water Trading Platform");
        }

        [HttpGet("current-user-active-offers/{postingID}")]
        [OfferManageFeature]
        public ActionResult<IEnumerable<OfferDto>> GetActiveOffersForCurrentUserByPosting([FromRoute] int postingID)
        {
            var userDto = GetCurrentUser();
            var offerDtos = Offer.GetActiveOffersFromPostingIDAndUserID(_dbContext, postingID, userDto.UserID);
            return Ok(offerDtos);
        }

        [HttpGet("trade-activity/{userID}")]
        [UserViewFeature]
        public ActionResult<IEnumerable<TradeWithMostRecentOfferDto>> GetActiveTradesForUser([FromRoute] int userID)
        {
            var tradeWithMostRecentOfferDtos = Trade.GetTradesForUserID(_dbContext, userID);
            return Ok(tradeWithMostRecentOfferDtos);
        }

        [HttpGet("trades/{tradeID}")]
        [OfferManageFeature]
        public ActionResult<TradeDto> GetTradeByTradeID([FromRoute] int tradeID)
        {
            var tradeDto = Trade.GetByTradeID(_dbContext, tradeID);
            return Ok(tradeDto);
        }

        [HttpGet("trades/{tradeID}/offers")]
        [OfferManageFeature]
        public ActionResult<IEnumerable<OfferDto>> GetOffersByTradeID([FromRoute] int tradeID)
        {
            var offerDtos = Offer.GetByTradeID(_dbContext, tradeID);
            return Ok(offerDtos);
        }

        private UserDto GetCurrentUser()
        {
            var userGuid = _keystoneService.GetProfile().Payload.UserGuid;
            var userDto = Rio.EFModels.Entities.User.GetByUserGuid(_dbContext, userGuid);
            return userDto;
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
    }
}