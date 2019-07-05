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
        public ActionResult<ParcelDto> GetAllocationAndConsumption([FromRoute] int parcelID)
        {
            var parcelDto = Parcel.GetByParcelID(_dbContext, parcelID);
            if (parcelDto == null)
            {
                return NotFound();
            }

            var parcelAllocationDtos = ParcelAllocation.ListByParcelID(_dbContext, parcelID);
            var parcelMonthlyEvapotranspirationDtos = ParcelMonthlyEvapotranspiration.ListByParcelID(_dbContext, parcelID);
            var waterYears = DateUtilities.GetRangeOfYears(DateUtilities.MinimumYear, DateTime.Now.Year);
            var parcelAllocationAndConsumptionDtos = new List<ParcelAllocationAndConsumptionDto>();
            foreach (var waterYear in waterYears)
            {
                var parcelAllocationAndConsumptionDto = new ParcelAllocationAndConsumptionDto() {WaterYear = waterYear};
                var parcelAllocationDtoForThisYear = parcelAllocationDtos.SingleOrDefault(x => x.WaterYear == waterYear);
                if (parcelAllocationDtoForThisYear != null)
                {
                    parcelAllocationAndConsumptionDto.AcreFeetAllocated = parcelAllocationDtoForThisYear.AcreFeetAllocated;
                }
                parcelAllocationAndConsumptionDto.MonthlyEvapotranspiration = parcelMonthlyEvapotranspirationDtos.Where(x => x.WaterYear == waterYear).ToList();
                parcelAllocationAndConsumptionDtos.Add(parcelAllocationAndConsumptionDto);
            }
            return Ok(parcelAllocationAndConsumptionDtos);
        }

        [HttpPost("parcels/{parcelID}/updateAnnualAllocations")]
        [ParcelManageFeature]
        [RequiresValidJSONBodyFilter("Could not parse a valid Parcel Allocation Upsert JSON object from the Request Body.")]
        public ActionResult<ParcelDto> UpdateParcelAllocation([FromRoute] int parcelID, [FromRoute] int waterYear, [FromBody] ParcelAllocationUpsertWrapperDto parcelAllocationUpsertWrapperDto)
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

            var updatedParcelDto = ParcelAllocation.Upsert(_dbContext, parcelID, parcelAllocationUpsertWrapperDto.ParcelAllocations);
            return Ok(updatedParcelDto);
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