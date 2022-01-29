using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Rio.API.Services;
using Rio.API.Services.Authorization;
using Rio.EFModels.Entities;
using Rio.API.Util;
using Rio.Models.DataTransferObjects;

namespace Rio.API.Controllers
{
    [ApiController]
    public class WaterTypeController : SitkaController<WaterTypeController>
    {
        public WaterTypeController(RioDbContext dbContext, ILogger<WaterTypeController> logger, KeystoneService keystoneService, IOptions<RioConfiguration> rioConfiguration) : base(dbContext, logger, keystoneService, rioConfiguration)
        {
        }

        [HttpGet("/water-types/")]
        [LoggedInUnclassifiedFeature]
        public ActionResult<List<WaterTypeDto>> GetWaterTypes()
        {
            var waterTypeDtos = WaterType.GetWaterTypes(_dbContext);
            return Ok(waterTypeDtos);
        }

        [HttpPut("/water-types/")]
        [ManagerDashboardFeature]
        public IActionResult MergeWaterTypes([FromBody] List<WaterTypeDto> waterTypeDtos)
        {
            var updatedWaterTypes = waterTypeDtos.Select(x => new WaterType()
            {
                WaterTypeName = x.WaterTypeName,
                IsAppliedProportionally = x.IsAppliedProportionally,
                IsSourcedFromApi = x.IsSourcedFromApi,
                WaterTypeID = x.WaterTypeID,
                WaterTypeDefinition = x.WaterTypeDefinition,
                SortOrder = x.SortOrder
            }).ToList();

            // add new PATs before the merge.
            var newWaterTypes = updatedWaterTypes.Where(x => x.WaterTypeID == 0);
            _dbContext.WaterTypes.AddRange(newWaterTypes);
            _dbContext.SaveChanges();
            
            var existingWaterTypes = _dbContext.WaterTypes.ToList();
            
            // blast ledger records for deleted water types
            var deletedWaterTypeIDs = existingWaterTypes.Select(x => x.WaterTypeID).Where(x =>
                !waterTypeDtos.Select(y => y.WaterTypeID).Contains(x)).ToList();
            _dbContext.ParcelLedgers.RemoveRange(_dbContext.ParcelLedgers.Where(x=> x.WaterTypeID.HasValue && deletedWaterTypeIDs.Contains(x.WaterTypeID.Value)));
            
            var allInDatabase = _dbContext.WaterTypes;

            existingWaterTypes.Merge(updatedWaterTypes, allInDatabase,
                (x, y) => x.WaterTypeID == y.WaterTypeID,
                (x, y) =>
                {
                    x.WaterTypeName = y.WaterTypeName;
                    x.IsAppliedProportionally = y.IsAppliedProportionally;
                    x.WaterTypeDefinition = y.WaterTypeDefinition;
                    x.IsSourcedFromApi = y.IsSourcedFromApi;
                    x.SortOrder = y.SortOrder;
                });

            _dbContext.SaveChanges();

            return Ok();
        }
    }

}
