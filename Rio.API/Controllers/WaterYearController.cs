using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Rio.API.Models;
using Rio.API.Services;
using Rio.API.Services.Authorization;
using Rio.EFModels.Entities;
using Rio.Models.DataTransferObjects;

namespace Rio.API.Controllers
{
    [ApiController]
    public class WaterYearController : SitkaController<WaterYearController>
    {
        public WaterYearController(RioDbContext dbContext, ILogger<WaterYearController> logger, KeystoneService keystoneService, IOptions<RioConfiguration> rioConfiguration) : base(dbContext, logger, keystoneService, rioConfiguration)
        {
        }


        [HttpGet("water-years")]
        [ParcelViewFeature]
        public ActionResult<List<WaterYearDto>> GetWaterYears()
        {
            var waterYears = WaterYear.List(_dbContext);
            return Ok(waterYears);
        }

        [HttpGet("water-years/current-and-variable-previous/{numYearsBackToInclude}")]
        [ParcelViewFeature]
        public ActionResult<List<WaterYearDto>> GetWaterYearForCurrentYearAndVariableYearsBack([FromRoute] int numYearsBackToInclude)
        {
            var endYear = DateTime.Now.Year;
            var startYear = endYear - numYearsBackToInclude;
            var waterYears = WaterYear.ListBetweenYears(_dbContext, startYear, endYear);
            return Ok(waterYears);
        }

        [HttpGet("water-years/default")]
        [ParcelViewFeature]
        public ActionResult<int> GetDefaultWaterYearToDisplay()
        {
            var waterYearToDisplay = WaterYear.GetDefaultYearToDisplay(_dbContext);
            return Ok(waterYearToDisplay);
        }

        [HttpPut("water-years/{waterYearID}/overconsumption-rate")]
        [ParcelManageFeature]
        public ActionResult UpdateOverconsumptionRate([FromRoute] int waterYearID, [FromBody] OverconsumptionRateUpsertDto overconsumptionRateUpsertDto)
        {
            if (overconsumptionRateUpsertDto.OverconsumptionRate < 0)
            {
                ModelState.AddModelError("OverconsumptionRate", "Overconsumption Rate cannot be negative.");
                return BadRequest(ModelState);
            }

            var waterYear = WaterYear.GetByID(_dbContext, waterYearID);
            if (waterYear == null)
            {
                return NotFound();
            }

            WaterYear.UpdateOverconsumptionRate(_dbContext, waterYear, overconsumptionRateUpsertDto.OverconsumptionRate.Value);

            return Ok();
        }
    }
}
