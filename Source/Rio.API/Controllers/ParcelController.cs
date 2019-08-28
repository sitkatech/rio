using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Rio.API.Services;
using Rio.API.Services.Authorization;
using Rio.API.Services.Filter;
using Rio.EFModels.Entities;
using Rio.Models.DataTransferObjects;
using Rio.Models.DataTransferObjects.Parcel;
using Rio.Models.DataTransferObjects.ParcelAllocation;
using System.Collections.Generic;

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
            var parcelDtos = Parcel.List(_dbContext);
            return Ok(parcelDtos);
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
            var waterYears = DateUtilities.GetWaterYears();
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
        [ParcelViewFeature]
        [RequiresValidJSONBodyFilter("Could not parse a valid Parcel ID List JSON object from the Request Body.")]
        public ActionResult<BoundingBoxDto> GetBoundingBoxByParcelIDs([FromBody] ParcelIDListDto parcelIDListDto)
        {
            var boundingBoxDto = Parcel.GetBoundingBoxByParcelIDs(_dbContext, parcelIDListDto.ParcelIDs);
            return Ok(boundingBoxDto);
        }


        [HttpGet("parcels/getParcelsWithLandOwners")]
        [ParcelManageFeature]
        public ActionResult<IEnumerable<ParcelDto>> GetParcelsWithLandOwners()
        {
            var parcelDtos = Parcel.ListParcelsWithLandOwners(_dbContext);
            return Ok(parcelDtos);
        }
    }
}