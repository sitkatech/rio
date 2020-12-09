using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Rio.API.Services;
using Rio.API.Services.Authorization;
using Rio.EFModels.Entities;
using Rio.Models.DataTransferObjects;

namespace Rio.API.Controllers
{
    [ApiController]
    public class WaterYearController : ControllerBase
    {
        private readonly RioDbContext _dbContext;
        private readonly ILogger<WaterYearController> _logger;
        private readonly KeystoneService _keystoneService;

        public WaterYearController(RioDbContext dbContext, ILogger<WaterYearController> logger, KeystoneService keystoneService)
        {
            _dbContext = dbContext;
            _logger = logger;
            _keystoneService = keystoneService;
        }

        [HttpGet("water-years")]
        [ParcelViewFeature]
        public ActionResult<List<WaterYearDto>> GetWaterYears()
        {
            var waterYears = WaterYear.List(_dbContext);
            return Ok(waterYears);
        }

        [HttpGet("water-years/non-finalized")]
        [ParcelViewFeature]
        public ActionResult<List<WaterYearDto>> GetNonFinalizedWaterYears()
        {
            var nonFinalizedWaterYears = WaterYear.ListNonFinalized(_dbContext);
            return Ok(nonFinalizedWaterYears);
        }

        [HttpGet("water-years/default")]
        [ParcelViewFeature]
        public ActionResult<int> GetDefaultWaterYearToDisplay()
        {
            var waterYearToDisplay = WaterYear.GetDefaultYearToDisplay(_dbContext);
            return Ok(waterYearToDisplay);
        }

        [HttpPut("water-year/finalize")]
        [ContentManageFeature]
        public ActionResult<WaterYearDto> FinalizeWaterYear([FromBody] int waterYearID)
        {
            var waterYearDto = WaterYear.GetByWaterYearID(_dbContext, waterYearID);
            if (waterYearDto == null)
            {
                return NotFound();
            }

            var finalizedWaterYearDto = WaterYear.Finalize(_dbContext, waterYearID);
            return Ok(finalizedWaterYearDto);
        }

        [HttpGet("water-years/abbreviated-open-et-sync-history")]
        [ManagerDashboardFeature]
        public ActionResult<List<OpenETSyncHistoryDto>> GetAbbreviatedSyncHistoryForWaterYears()
        {
            var waterYears = WaterYear.List(_dbContext);

            if (waterYears == null || waterYears.Count == 0)
            {
                //We just have no water years
                return Ok(null);
            }

            var waterYearQuickOpenETHistoryDtos =
                waterYears.Select(x => OpenETSyncHistory.GetQuickHistoryForWaterYear(_dbContext, x.WaterYearID)).ToList();

            return Ok(waterYearQuickOpenETHistoryDtos);
        }
    }
}
