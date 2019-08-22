using System.Net.Mail;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Rio.API.Services;
using Rio.API.Services.Authorization;
using Rio.EFModels.Entities;
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

            WaterTransfer.Confirm(_dbContext, waterTransferID, waterTransferConfirmDto);

            var mailMessage = GenerateConfirmTransferEmail(waterTransferDto, waterTransferConfirmDto.WaterTransferType);
            var smtpClient = HttpContext.RequestServices.GetRequiredService<SitkaSmtpClientService>();
            SendEmailMessage(smtpClient, mailMessage);
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

        private static MailMessage GenerateConfirmTransferEmail(WaterTransferDto waterTransfer, int waterTransferType)
        {
            var receivingUser = waterTransfer.ReceivingUser;
            var transferringUser = waterTransfer.TransferringUser;
            var mailTo = waterTransferType == (int)WaterTransferTypeEnum.Receiving ? receivingUser : transferringUser;
            var messageBody = $@"Hello {mailTo.FullName},<br /><br />
You have confirmed the following transaction:
<ul>
    <li><strong>To:</strong> {receivingUser.FullName} ({receivingUser.Email})</li>
    <li><strong>From:</strong> {transferringUser.FullName} ({transferringUser.Email})</li>
    <li><strong>Quantity:</strong> {waterTransfer.AcreFeetTransferred} acre-feet</li>
</ul>
<br /><br />
{SitkaSmtpClientService.GetDefaultEmailSignature()}";
            var mailMessage = new MailMessage
            {
                Subject = "Water Transfer Confirmed",
                Body = messageBody
            };
            mailMessage.To.Add(new MailAddress(mailTo.Email, mailTo.FullName));
            return mailMessage;
        }
    }
}