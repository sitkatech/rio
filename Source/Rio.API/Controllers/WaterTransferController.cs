using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Rio.API.Services;
using Rio.API.Services.Authorization;
using Rio.EFModels.Entities;
using Rio.Models.DataTransferObjects.Account;
using Rio.Models.DataTransferObjects.WaterTransfer;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using Rio.Models.DataTransferObjects.User;

namespace Rio.API.Controllers
{
    [ApiController]
    public class WaterTransferController : ControllerBase
    {
        private readonly RioDbContext _dbContext;
        private readonly ILogger<OfferController> _logger;
        private readonly KeystoneService _keystoneService;
        private readonly RioConfiguration _rioConfiguration;

        public WaterTransferController(RioDbContext dbContext, ILogger<OfferController> logger, KeystoneService keystoneService, IOptions<RioConfiguration> rioConfiguration)
        {
            _dbContext = dbContext;
            _logger = logger;
            _keystoneService = keystoneService;
            _rioConfiguration = rioConfiguration.Value;
        }

        [HttpGet("water-transfers/{waterTransferID}")]
        [OfferManageFeature]
        public ActionResult<WaterTransferDto> GetByWaterTransferID([FromRoute] int waterTransferID)
        {
            var waterTransferDto = WaterTransfer.GetByWaterTransferID(_dbContext, waterTransferID);
            if (waterTransferDto == null)
            {
                return NotFound();
            }

            return Ok(waterTransferDto);
        }

        [HttpGet("water-transfers/{waterTransferID}/registrations")]
        [OfferManageFeature]
        public ActionResult<List<WaterTransferRegistrationSimpleDto>> GetWaterTransferRegistrationsByWaterTransferID([FromRoute] int waterTransferID)
        {
            var waterTransferRegistrationSimpleDtos = WaterTransferRegistration.GetByWaterTransferID(_dbContext, waterTransferID);
            if (waterTransferRegistrationSimpleDtos == null)
            {
                return NotFound();
            }

            return Ok(waterTransferRegistrationSimpleDtos);
        }

        [HttpPost("water-transfers/{waterTransferID}/register")]
        [OfferManageFeature]
        public ActionResult<WaterTransferDto> ConfirmTransfer([FromRoute] int waterTransferID, [FromBody] WaterTransferRegistrationDto waterTransferRegistrationDto)
        {
            if (!_rioConfiguration.ALLOW_TRADING)
            {
                return BadRequest();
            }

            var waterTransferDto = WaterTransfer.GetByWaterTransferID(_dbContext, waterTransferID);
            if (waterTransferDto == null)
            {
                return NotFound();
            }

            var currentUser = GetCurrentUser();

            var validationMessages = WaterTransfer.ValidateConfirmTransfer(waterTransferRegistrationDto, waterTransferDto, currentUser);
            validationMessages.ForEach(vm => {ModelState.AddModelError(vm.Type, vm.Message);});
            
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            waterTransferDto = WaterTransfer.ChangeWaterRegistrationStatus(_dbContext, waterTransferID, waterTransferRegistrationDto, WaterTransferRegistrationStatusEnum.Registered);
            if (waterTransferDto.BuyerRegistration.IsRegistered && waterTransferDto.SellerRegistration.IsRegistered)
            {
                var smtpClient = HttpContext.RequestServices.GetRequiredService<SitkaSmtpClientService>();
                var mailMessages = GenerateConfirmTransferEmail(_rioConfiguration.RIO_WEB_URL, waterTransferDto, smtpClient);
                foreach (var mailMessage in mailMessages)
                {
                    SendEmailMessage(smtpClient, mailMessage);
                }
            }

            return Ok(waterTransferDto);
        }

        [HttpPost("/water-transfers/{waterTransferID}/cancel")]
        [OfferManageFeature]
        public IActionResult CancelTrade([FromRoute] int waterTransferID, [FromBody] WaterTransferRegistrationDto waterTransferRegistrationDto)
        {
            if (!_rioConfiguration.ALLOW_TRADING)
            {
                return BadRequest();
            }

            var waterTransferDto = WaterTransfer.GetByWaterTransferID(_dbContext, waterTransferID);
            if (waterTransferDto == null)
            {
                return NotFound();
            }

            var currentUser = GetCurrentUser();

            var validationMessages = WaterTransfer.ValidateCancelTransfer(waterTransferRegistrationDto, waterTransferDto, currentUser);
            validationMessages.ForEach(vm => { ModelState.AddModelError(vm.Type, vm.Message); });

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            waterTransferDto = WaterTransfer.ChangeWaterRegistrationStatus(_dbContext, waterTransferID, waterTransferRegistrationDto,
                WaterTransferRegistrationStatusEnum.Canceled);
            var smtpClient = HttpContext.RequestServices.GetRequiredService<SitkaSmtpClientService>();
            var mailMessages = GenerateCancelTransferEmail(_rioConfiguration.RIO_WEB_URL, waterTransferDto, smtpClient);
            foreach (var mailMessage in mailMessages)
            {
                SendEmailMessage(smtpClient, mailMessage);
            }

            return Ok(waterTransferDto);
        }



        [HttpGet("water-transfers/{waterTransferID}/parcels/{userID}")]
        [OfferManageFeature]
        public ActionResult<List<WaterTransferRegistrationParcelDto>> GetParcelsForWaterTransferID([FromRoute] int waterTransferID, [FromRoute] int userID)
        {
            var waterTransferRegistrationParcelDtos = WaterTransferRegistrationParcel.ListByWaterTransferIDAndAccountID(_dbContext, waterTransferID, userID);
            if (waterTransferRegistrationParcelDtos == null)
            {
                return NotFound();
            }

            return Ok(waterTransferRegistrationParcelDtos);
        }

        [HttpPost("water-transfers/{waterTransferID}/selectParcels")]
        [OfferManageFeature]
        public ActionResult<List<WaterTransferDto>> SelectParcels([FromRoute] int waterTransferID, [FromBody] WaterTransferRegistrationDto waterTransferRegistrationDto)
        {
            if (!_rioConfiguration.ALLOW_TRADING)
            {
                return BadRequest();
            }

            var waterTransferDto = WaterTransfer.GetByWaterTransferID(_dbContext, waterTransferID);
            if (waterTransferDto == null)
            {
                return NotFound();
            }
            var validationMessages = WaterTransferRegistrationParcel.ValidateParcels(waterTransferRegistrationDto.WaterTransferRegistrationParcels, waterTransferDto);
            validationMessages.ForEach(vm => {ModelState.AddModelError(vm.Type, vm.Message);});

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var waterTransferRegistrationParcelDtos = WaterTransferRegistrationParcel.SaveParcels(_dbContext, waterTransferID, waterTransferRegistrationDto);
            return Ok(waterTransferRegistrationParcelDtos);
        }

        private void SendEmailMessage(SitkaSmtpClientService smtpClient, MailMessage mailMessage)
        {
            mailMessage.IsBodyHtml = true;
            mailMessage.From = smtpClient.GetDefaultEmailFrom();
            mailMessage.ReplyToList.Add(_rioConfiguration.LeadOrganizationEmail);
            SitkaSmtpClientService.AddBccRecipientsToEmail(mailMessage, EFModels.Entities.User.GetEmailAddressesForAdminsThatReceiveSupportEmails(_dbContext));
            smtpClient.Send(mailMessage);
        }

        private static List<MailMessage> GenerateConfirmTransferEmail(string rioUrl, WaterTransferDto waterTransfer,
            SitkaSmtpClientService smtpClient)
        {
            var receivingAccount = waterTransfer.BuyerRegistration.Account;
            var transferringAccount = waterTransfer.SellerRegistration.Account;
            var mailMessages = new List<MailMessage>();
            var messageBody = $@"The buyer and seller have both registered this transaction, and your annual water allocation has been updated.
<ul>
    <li><strong>Buyer:</strong> {receivingAccount.AccountName} ({string.Join(", ", receivingAccount.Users.Select(x=>x.Email))})</li>
    <li><strong>Seller:</strong> {transferringAccount.AccountName} ({string.Join(", ", transferringAccount.Users.Select(x => x.Email))})</li>
    <li><strong>Quantity:</strong> {waterTransfer.AcreFeetTransferred} acre-feet</li>
</ul>
<a href=""{rioUrl}/landowner-dashboard"">View your Landowner Dashboard</a> to see your current water allocation, which has been updated to reflect this trade.
<br /><br />
{smtpClient.GetDefaultEmailSignature()}";
            var mailTos = new List<AccountDto> { receivingAccount, transferringAccount }.SelectMany(x=>x.Users);
            foreach (var mailTo in mailTos)
            {
                var mailMessage = new MailMessage
                {
                    Subject = "Water Transfer Registered",
                    Body = $"Hello {mailTo.FullName},<br /><br />{messageBody}"
                };
                mailMessage.To.Add(new MailAddress(mailTo.Email, mailTo.FullName));
                mailMessages.Add(mailMessage);
            }
            return mailMessages;
        }
        private static List<MailMessage> GenerateCancelTransferEmail(string rioUrl, WaterTransferDto waterTransfer,
            SitkaSmtpClientService smtpClient)
        {
            var receivingAccount = waterTransfer.BuyerRegistration.Account;
            var transferringAccount = waterTransfer.SellerRegistration.Account;
            var mailMessages = new List<MailMessage>();
            var messageBody = $@"This transaction has been canceled, and your annual water allocation will not be updated.
<ul>
    <li><strong>Buyer:</strong> {receivingAccount.AccountName} ({string.Join(", ", receivingAccount.Users.Select(x => x.Email))})</li>
    <li><strong>Seller:</strong> {transferringAccount.AccountName} ({string.Join(", ", transferringAccount.Users.Select(x => x.Email))})</li>
    <li><strong>Quantity:</strong> {waterTransfer.AcreFeetTransferred} acre-feet</li>
</ul>
<a href=""{rioUrl}/landowner-dashboard"">View your Landowner Dashboard</a> to see your current water allocation, which has not been updated to reflect this trade.
<br /><br />
{smtpClient.GetDefaultEmailSignature()}";
            var mailTos = new List<AccountDto> { receivingAccount, transferringAccount }.SelectMany(x=>x.Users);
            foreach (var mailTo in mailTos)
            {
                var mailMessage = new MailMessage
                {
                    Subject = "Water Transfer Canceled",
                    Body = $"Hello {mailTo.FullName},<br /><br />{messageBody}"
                };
                mailMessage.To.Add(new MailAddress(mailTo.Email, mailTo.FullName));
                mailMessages.Add(mailMessage);
            }
            return mailMessages;
        }

        private UserDto GetCurrentUser()
        {
            var userGuid = _keystoneService.GetProfile().Payload.UserGuid;
            var userDto = Rio.EFModels.Entities.User.GetByUserGuid(_dbContext, userGuid);
            return userDto;
        }
    }
}