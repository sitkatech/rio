using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Rio.API.Services;
using Rio.API.Services.Authorization;
using Rio.EFModels.Entities;
using Rio.Models.DataTransferObjects;

namespace Rio.API.Controllers
{
    [ApiController]
    public class WaterYearMonthController : SitkaController<WaterYearMonthController>
    {
        public WaterYearMonthController(RioDbContext dbContext, ILogger<WaterYearMonthController> logger, KeystoneService keystoneService, IOptions<RioConfiguration> rioConfiguration) : base(dbContext, logger, keystoneService, rioConfiguration)
        {
        }

        [HttpGet("water-year-months")]
        [ParcelViewFeature]
        public ActionResult<List<WaterYearMonthDto>> GetWaterYearMonths()
        {
            var waterYearMonths = WaterYearMonth.List(_dbContext);
            return Ok(waterYearMonths);
        }

        [HttpGet("water-year-months/most-recent-sync-history")]
        [ManagerDashboardFeature]
        public ActionResult<List<OpenETSyncHistoryDto>> GetMostRecentSyncHistoryForWaterYearMonthsThatHaveBeenUpdated()
        {
            return _dbContext.vOpenETMostRecentSyncHistoryForYearAndMonth
                .Include(x => x.OpenETSyncResultType)
                .Include(x => x.WaterYearMonth)
                .ThenInclude(x => x.WaterYear)
                .Select(x => x.AsOpenETSyncHistoryDto()).ToList();
        }

        [HttpPut("water-year-month/finalize")]
        [ContentManageFeature]
        public ActionResult<WaterYearMonthDto> FinalizeWaterYearMonth([FromBody] int waterYearMonthID)
        {
            var waterYearMonthDto = WaterYearMonth.GetByWaterYearMonthID(_dbContext, waterYearMonthID);
            if (ThrowNotFound(waterYearMonthDto, "Water Year Month", waterYearMonthID, out var actionResult))
            {
                return actionResult;
            }

            var finalizedWaterYearDto = WaterYearMonth.Finalize(_dbContext, waterYearMonthID);
            return Ok(finalizedWaterYearDto);
        }
    }
}