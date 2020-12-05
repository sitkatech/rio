using System.Net.Mail;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Rio.API.Services;
using Rio.EFModels.Entities;

namespace Rio.API.Controllers
{
    [ApiController]
    public class ErrorController : ControllerBase
    {
        private readonly RioDbContext _dbContext;
        private readonly ILogger<ErrorController> _logger;
        private readonly KeystoneService _keystoneService;
        private readonly RioConfiguration _rioConfiguration;

        public ErrorController(RioDbContext dbContext, ILogger<ErrorController> logger, KeystoneService keystoneService, IOptions<RioConfiguration> rioConfiguration)
        {
            _dbContext = dbContext;
            _logger = logger;
            _keystoneService = keystoneService;
            _rioConfiguration = rioConfiguration.Value;
        }

        [HttpPost("/error/front-end")]
        public ActionResult SendFrontEndErrorEmail([FromBody] ErrorMessage error)
        {
            if (string.IsNullOrEmpty(error.Error))
            {
                return Ok();
            }

            var smtpClient = HttpContext.RequestServices.GetRequiredService<SitkaSmtpClientService>();
            var messageBody =
                $"A Front End error has occurred in {_rioConfiguration.PlatformShortName} <br/><br/> {error.Error.Replace("\n", "<br/>")}";
            var supportMailAddress = new MailAddress(_rioConfiguration.SupportEmailAddress);
            var mailMessage = new MailMessage
            {
                Subject = $"{_rioConfiguration.PlatformShortName} Front End Error",
                Body = messageBody,
                IsBodyHtml = true,
                From = supportMailAddress
            };
            mailMessage.To.Add(supportMailAddress);
            smtpClient.Send(mailMessage);

            return Ok();
        }
    }

    public class ErrorMessage
    {
        public string Error { get; set; }
    }
}