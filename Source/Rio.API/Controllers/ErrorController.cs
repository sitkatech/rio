using System;
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
            if (error == null)
            {
                return Ok();
            }

            var messageBody =
                $"A Front End error has occurred in {_rioConfiguration.PlatformShortName}. \n Error Message:{error.Message} \n Error Stack:{error.Stack}";
            
            _logger.LogError(messageBody);

            return Ok();
        }
    }

    public class ErrorMessage
    {
        public string Message { get; set; }
        public string Stack { get; set; }
    }
}