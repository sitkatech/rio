using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Rio.API.Services;
using Rio.API.Services.Authorization;
using Rio.API.Services.Filter;
using Rio.EFModels.Entities;
using Rio.Models.DataTransferObjects;
using Rio.Models.DataTransferObjects.Parcel;
using Rio.Models.DataTransferObjects.ParcelAllocation;

namespace Rio.API.Controllers
{
    public class ParcelController : Controller
    {
        private readonly RioDbContext _dbContext;
        private readonly ILogger<ParcelController> _logger;
        private readonly KeystoneService _keystoneService;

        public ParcelController(RioDbContext dbContext, ILogger<ParcelController> logger, KeystoneService keystoneService)
        {
            _dbContext = dbContext;
            _logger = logger;
            _keystoneService = keystoneService;
        }

        [HttpGet("parcels")]
        [ParcelManageFeature]
        public ActionResult<IEnumerable<ParcelDto>> List()
        {
            var userDtos = Parcel.List(_dbContext);
            return Ok(userDtos);
        }

        [HttpGet("parcels/{parcelID}")]
        [ParcelManageFeature]
        public ActionResult<ParcelDto> GetByParcelID([FromRoute] int parcelID)
        {
            var parcelDto = Parcel.GetByParcelID(_dbContext, parcelID);
            if (parcelDto == null)
            {
                return NotFound();
            }

            return Ok(parcelDto);
        }

        [HttpGet("parcels/{parcelID}/getAllocationAndConsumption")]
        [ParcelManageFeature]
        public ActionResult<List<ParcelAllocationAndConsumptionDto>> GetAllocationAndConsumption([FromRoute] int parcelID)
        {
            var parcelDto = Parcel.GetByParcelID(_dbContext, parcelID);
            if (parcelDto == null)
            {
                return NotFound();
            }

            var parcelAllocationDtos = ParcelAllocation.ListByParcelID(_dbContext, parcelID);
            var parcelMonthlyEvapotranspirationDtos = ParcelMonthlyEvapotranspiration.ListByParcelID(_dbContext, parcelID);
            var waterYears = DateUtilities.GetRangeOfYears(DateUtilities.MinimumYear, DateUtilities.GetLatestWaterYear());
            var parcelAllocationAndConsumptionDtos = ParcelExtensionMethods.CreateParcelAllocationAndConsumptionDtos(waterYears, new List<ParcelDto>{parcelDto}, parcelAllocationDtos, parcelMonthlyEvapotranspirationDtos);
            return Ok(parcelAllocationAndConsumptionDtos);
        }

        [HttpPost("parcels/{parcelID}/updateAnnualAllocations")]
        [ParcelManageFeature]
        [RequiresValidJSONBodyFilter("Could not parse a valid Parcel Allocation Upsert JSON object from the Request Body.")]
        public ActionResult<List<ParcelAllocationAndConsumptionDto>> UpdateParcelAllocation([FromRoute] int parcelID, [FromBody] ParcelAllocationUpsertWrapperDto parcelAllocationUpsertWrapperDto)
        {
            var parcelDto = Parcel.GetByParcelID(_dbContext, parcelID);
            if (parcelDto == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updatedParcelAllocationDtos = ParcelAllocation.Upsert(_dbContext, parcelID, parcelAllocationUpsertWrapperDto.ParcelAllocations);
            return Ok(updatedParcelAllocationDtos);
        }


        [HttpPost("parcels/getBoundingBox")]
        [ParcelManageFeature]
        [RequiresValidJSONBodyFilter("Could not parse a valid Parcel ID List JSON object from the Request Body.")]
        public ActionResult<BoundingBoxDto> GetBoundingBoxByParcelIDs([FromBody] ParcelIDListDto parcelIDListDto)
        {
            var boundingBoxDto = Parcel.GetBoundingBoxByParcelIDs(_dbContext, parcelIDListDto.ParcelIDs);
            return Ok(boundingBoxDto);
        }
    }
}