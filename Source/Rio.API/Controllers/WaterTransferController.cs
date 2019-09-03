using System.Collections.Generic;
using System.Net.Mail;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Rio.API.Services;
using Rio.API.Services.Authorization;
using Rio.EFModels.Entities;
using Rio.Models.DataTransferObjects.User;
using Rio.Models.DataTransferObjects.WaterTransfer;

namespace Rio.API.Controllers
{
    public class WaterTransferController : Controller
    {
        private readonly RioDbContext _dbContext;
        private readonly ILogger<OfferController> _logger;
        private readonly KeystoneService _keystoneService;
        private readonly string _rioWebUrl;

        public WaterTransferController(RioDbContext dbContext, ILogger<OfferController> logger, KeystoneService keystoneService, IOptions<RioConfiguration> rioConfigurationOptions)
        {
            _dbContext = dbContext;
            _logger = logger;
            _keystoneService = keystoneService;
            _rioWebUrl = rioConfigurationOptions.Value.RIO_WEB_URL;
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

        [HttpPost("water-transfers/{waterTransferID}/confirm")]
        [OfferManageFeature]
        public ActionResult<WaterTransferDto> ConfirmTransfer([FromRoute] int waterTransferID, [FromBody] WaterTransferConfirmDto waterTransferConfirmDto)
        {
            var waterTransferDto = WaterTransfer.GetByWaterTransferID(_dbContext, waterTransferID);
            if (waterTransferDto == null)
            {
                return NotFound();
            }

            var validationMessages = WaterTransfer.ValidateConfirmTransfer(waterTransferConfirmDto, waterTransferDto);
            validationMessages.ForEach(vm => {ModelState.AddModelError(vm.Type, vm.Message);});

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            waterTransferDto = WaterTransfer.Confirm(_dbContext, waterTransferID, waterTransferConfirmDto);
            if (waterTransferDto.ConfirmedByReceivingUser && waterTransferDto.ConfirmedByTransferringUser)
            {
                var smtpClient = HttpContext.RequestServices.GetRequiredService<SitkaSmtpClientService>();
                var mailMessages = GenerateConfirmTransferEmail(_rioWebUrl, waterTransferDto, waterTransferConfirmDto.WaterTransferType);
                foreach (var mailMessage in mailMessages)
                {
                    SendEmailMessage(smtpClient, mailMessage);
                }
            }

            return Ok(waterTransferDto);
        }

        private void SendEmailMessage(SitkaSmtpClientService smtpClient, MailMessage mailMessage)
        {
            mailMessage.IsBodyHtml = true;
            mailMessage.From = SitkaSmtpClientService.GetDefaultEmailFrom();
            SitkaSmtpClientService.AddReplyToEmail(mailMessage);
            SitkaSmtpClientService.AddAdminsAsBccRecipientsToEmail(mailMessage, EFModels.Entities.User.ListByRole(_dbContext, RoleEnum.Admin));
            smtpClient.Send(mailMessage);
        }

        private static List<MailMessage> GenerateConfirmTransferEmail(string rioUrl, WaterTransferDto waterTransfer, int waterTransferType)
        {
            var receivingUser = waterTransfer.ReceivingUser;
            var transferringUser = waterTransfer.TransferringUser;
            var mailMessages = new List<MailMessage>();
            var messageBody = $@"The buyer and seller have both confirmed this transaction, and your annual water allocation has been updated.
<ul>
    <li><strong>Buyer:</strong> {receivingUser.FullName} ({receivingUser.Email})</li>
    <li><strong>Seller:</strong> {transferringUser.FullName} ({transferringUser.Email})</li>
    <li><strong>Quantity:</strong> {waterTransfer.AcreFeetTransferred} acre-feet</li>
</ul>
<a href=""{rioUrl}/landowner-dashboard"">View your Landowner Dashboard</a> to see your current water allocation , which has not been updated to reflect this trade.
<br /><br />
{SitkaSmtpClientService.GetDefaultEmailSignature()}";
            var mailTos = new List<UserSimpleDto> { receivingUser, transferringUser };
            foreach (var mailTo in mailTos)
            {
                var mailMessage = new MailMessage
                {
                    Subject = "Water Transfer Confirmed",
                    Body = $"Hello {mailTo.FullName},<br /><br />{messageBody}"
                };
                mailMessage.To.Add(new MailAddress(mailTo.Email, mailTo.FullName));
                mailMessages.Add(mailMessage);
            }
            return mailMessages;
        }
    }
}