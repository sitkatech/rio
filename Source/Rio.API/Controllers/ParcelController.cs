﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Rio.API.Services;
using Rio.API.Services.Authorization;
using Rio.EFModels.Entities;
using Rio.Models.DataTransferObjects;
using Rio.Models.DataTransferObjects.Parcel;
using Rio.Models.DataTransferObjects.ParcelAllocation;
using System.Collections.Generic;

namespace Rio.API.Controllers
{
    [ApiController]
    public class ParcelController : ControllerBase
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

        [HttpGet("parcels/getParcelsWithAllocationAndUsage/{year}")]
        [ParcelManageFeature]
        public ActionResult<IEnumerable<ParcelDto>> GetParcelsWithAllocationAndUsage([FromRoute] int year)
        {
            var parcelDtos = ParcelAllocationAndUsage.GetByYear(_dbContext, year);
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

        [HttpGet("parcels/{parcelID}/getAllocations")]
        [ParcelManageFeature]
        public ActionResult<List<ParcelAllocationDto>> GetAllocations([FromRoute] int parcelID)
        {
            var parcelDto = Parcel.GetByParcelID(_dbContext, parcelID);
            if (parcelDto == null)
            {
                return NotFound();
            }

            var parcelAllocationDtos = ParcelAllocation.ListByParcelID(_dbContext, parcelID);
            return Ok(parcelAllocationDtos);
        }

        [HttpGet("getWaterYears")]
        [ParcelViewFeature]
        public ActionResult<List<int>> GetWaterYears()
        {
            return Ok(DateUtilities.GetWaterYears());
        }

        [HttpGet("parcels/{parcelID}/getWaterUsage")]
        [ParcelManageFeature]
        public ActionResult<ParcelAllocationAndConsumptionDto> GetAllocationAndConsumption([FromRoute] int parcelID)
        {
            var parcelDto = Parcel.GetByParcelID(_dbContext, parcelID);
            if (parcelDto == null)
            {
                return NotFound();
            }

            var parcelMonthlyEvapotranspirationDtos = ParcelMonthlyEvapotranspiration.ListByParcelID(_dbContext, parcelID);
            return Ok(parcelMonthlyEvapotranspirationDtos);
        }

        [HttpPost("parcels/{parcelID}/updateAnnualAllocations")]
        [ParcelManageFeature]
        public ActionResult<List<ParcelAllocationDto>> UpdateParcelAllocation([FromRoute] int parcelID, [FromBody] ParcelAllocationUpsertWrapperDto parcelAllocationUpsertWrapperDto)
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

        [HttpPost("parcels/bulkSetAnnualParcelAllocation")]
        [ParcelManageFeature]
        public ActionResult BulkSetAnnualParcelAllocation([FromBody] ParcelAllocationUpsertDto parcelAllocationUpsertDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var numberOfParcels = ParcelAllocation.BulkSetAllocation(_dbContext, parcelAllocationUpsertDto);
            return Ok(numberOfParcels);
        }


        [HttpPost("parcels/getBoundingBox")]
        [ParcelViewFeature]
        public ActionResult<BoundingBoxDto> GetBoundingBoxByParcelIDs([FromBody] ParcelIDListDto parcelIDListDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

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