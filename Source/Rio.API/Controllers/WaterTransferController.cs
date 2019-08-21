using Microsoft.AspNetCore.Mvc;
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
    }
}