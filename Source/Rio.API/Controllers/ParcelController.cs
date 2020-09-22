﻿using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Rio.API.Services;
using Rio.API.Services.Authorization;
using Rio.EFModels.Entities;
using Rio.Models.DataTransferObjects;
using Rio.Models.DataTransferObjects.Parcel;
using Rio.Models.DataTransferObjects.ParcelAllocation;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;
using Rio.API.Util;
using Rio.Models.DataTransferObjects.BulkSetAllocationCSV;
using MissingFieldException = CsvHelper.MissingFieldException;

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
        [ManagerDashboardFeature]
        public ActionResult<IEnumerable<ParcelAllocationAndUsageDto>> GetParcelsWithAllocationAndUsage([FromRoute] int year)
        {
            var parcelDtos = ParcelAllocationAndUsage.GetByYear(_dbContext, year);
            var parcelAllocationBreakdownForYear = ParcelAllocation.GetParcelAllocationBreakdownForYear(_dbContext, year);
            var parcelDtosWithAllocation = parcelDtos.Join(parcelAllocationBreakdownForYear, x => x.ParcelID, y => y.ParcelID, (x, y) =>
            {
                x.Allocations = y.Allocations;
                return x;
            });
            return Ok(parcelDtosWithAllocation);
        }

        [HttpGet("parcels/{parcelID}")]
        [ManagerDashboardFeature]
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
        [ManagerDashboardFeature]
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

        [HttpGet("getWaterYears/{includeCurrentYear}")]
        [ParcelViewFeature]
        public ActionResult<List<int>> GetWaterYears(bool includeCurrentYear)
        {
            return Ok(DateUtilities.GetWaterYears(includeCurrentYear));
        }

        [HttpGet("getDefaultWaterYearToDisplay")]
        [ParcelViewFeature]
        public ActionResult<int> GetDefaultWaterYearToDisplay()
        {
            return Ok(DateUtilities.GetDefaultWaterYearToDisplay(_dbContext));
        }

        [HttpGet("parcels/{parcelID}/getWaterUsage")]
        [ManagerDashboardFeature]
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
        
        [HttpPost("parcels/{parcelID}/mergeParcelAllocations")]
        [ParcelManageFeature]
        public IActionResult MergeParcelAllocations([FromRoute] int parcelID,
            [FromBody] List<ParcelAllocationDto> parcelAllocationDtos)
        {
            var parcel = _dbContext.Parcel.Include(x => x.ParcelAllocation)
                .SingleOrDefault(x => x.ParcelID == parcelID);

            if (parcel == null)
            {
                return NotFound($"Did not find Parcel with ID {parcelID}");
            }

            var updatedParcelAllocations = parcelAllocationDtos.Select(x => new ParcelAllocation()
            {
                ParcelID = x.ParcelID,
                ParcelAllocationTypeID = x.ParcelAllocationTypeID,
                WaterYear = x.WaterYear,
                AcreFeetAllocated = x.AcreFeetAllocated,
                ParcelAllocationID = x.ParcelAllocationID
            }).ToList();

            // add new PAs before the merge.
            var newParcelAllocations = updatedParcelAllocations.Where(x => x.ParcelAllocationID == 0);
            _dbContext.ParcelAllocation.AddRange(newParcelAllocations);

            var existingParcelAllocations = parcel.ParcelAllocation;
            var allInDatabase = _dbContext.ParcelAllocation;

            existingParcelAllocations.Merge(updatedParcelAllocations, allInDatabase,
                (x, y) => x.ParcelAllocationTypeID == y.ParcelAllocationTypeID && x.ParcelID == y.ParcelID && x.WaterYear == y.WaterYear,
                (x, y) => x.AcreFeetAllocated = y.AcreFeetAllocated
                );

            _dbContext.SaveChanges();

            return Ok();

        }

        [HttpPost("parcels/{userID}/bulkSetAnnualParcelAllocation")]
        [ParcelManageFeature]
        public ActionResult BulkSetAnnualParcelAllocation([FromRoute] int userID, [FromBody] ParcelAllocationUpsertDto parcelAllocationUpsertDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var numberOfParcels = ParcelAllocation.BulkSetAllocation(_dbContext, parcelAllocationUpsertDto);
            ParcelAllocationHistory.CreateParcelAllocationHistoryEntity(_dbContext, userID, parcelAllocationUpsertDto, null);

            return Ok(numberOfParcels);
        }

        [HttpPost("parcels/{waterYear}/{parcelAllocationTypeID}/bulkSetAnnualParcelAllocationFileUpload")]
        public async Task<ActionResult> BulkSetAnnualParcelAllocationFileUpload([FromRoute] int waterYear,
            [FromRoute] int parcelAllocationTypeID)
        {
            var fileResource = await HttpUtilities.MakeFileResourceFromHttpRequest(Request, _dbContext, HttpContext);
            var parcelAllocationTypeDisplayName =
                _dbContext.ParcelAllocationType.Single(x => x.ParcelAllocationTypeID == parcelAllocationTypeID).ParcelAllocationTypeName;

            if (!ParseBulkSetAllocationUpload(fileResource, parcelAllocationTypeDisplayName, out var records, out var badRequestFromUpload))
            {
                return badRequestFromUpload;
            }

            if (!ValidateBulkSetAllocationUpload(records, parcelAllocationTypeDisplayName, out var badRequestFromValidation))
            {
                return badRequestFromValidation;
            }

            _dbContext.FileResource.Add(fileResource);
            _dbContext.SaveChanges();

            ParcelAllocation.BulkSetAllocation(_dbContext, records, waterYear, parcelAllocationTypeID);

            ParcelAllocationHistory.CreateParcelAllocationHistoryEntity(_dbContext,
                UserContext.GetUserFromHttpContext(_dbContext, HttpContext).UserID, fileResource.FileResourceID, waterYear,
                parcelAllocationTypeID, null);

            return Ok();
        }

        [HttpGet("parcels/getParcelAllocationHistory")]
        [ManagerDashboardFeature]
        public ActionResult<List<ParcelAllocationHistoryDto>> GetParcelAllocationHistory()
        {
            return Ok(ParcelAllocationHistory.GetParcelAllocationHistoryDtos(_dbContext).ToList().OrderByDescending(x => x.Date));
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


        [HttpGet("parcels/getParcelsWithLandOwners/{year}")]
        [ManagerDashboardFeature]
        public ActionResult<IEnumerable<ParcelDto>> GetParcelsWithLandOwners([FromRoute] int year)
        {
            var parcelDtos = Parcel.ListParcelsWithLandOwners(_dbContext, year);
            return Ok(parcelDtos);
        }

        [HttpGet("parcels/{parcelID}/getOwnershipHistory")]
        [ManagerDashboardFeature]
        public ActionResult<IEnumerable<ParcelOwnershipDto>> GetOwnershipHistory([FromRoute] int parcelID)
        {
            var parcelOwnershipDtos = Parcel.GetOwnershipHistory(_dbContext, parcelID).ToList().OrderByDescending(x => x.SaleDate);

            return Ok(parcelOwnershipDtos);
        }

        [HttpPost("parcels/{parcelID}/changeOwner")]
        [ParcelManageFeature]
        public ActionResult<IEnumerable<ParcelOwnershipDto>> ChangeOwner([FromRoute] int parcelID, [FromBody] ParcelChangeOwnerDto parcelChangeOwnerDto)
        {
            var parcelDto = Parcel.GetByParcelID(_dbContext, parcelID);
            if (parcelDto == null)
            {
                return NotFound();
            }

            var errorMessages = Parcel.ValidateChangeOwner(_dbContext, parcelChangeOwnerDto, parcelDto).ToList();
            errorMessages.ForEach(vm => { ModelState.AddModelError(vm.Type, vm.Message); });

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            UserParcel.ChangeParcelOwner(_dbContext, parcelID, parcelChangeOwnerDto);

            return Ok();
        }

        private bool ParseBulkSetAllocationUpload(FileResource fileResource, string parcelTypeDisplayName, out List<BulkSetAllocationCSV> records, out ActionResult badRequest)
        {
            try
            {
                using var memoryStream = new MemoryStream(fileResource.FileResourceData);
                using var reader = new StreamReader(memoryStream);
                using var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture);
                csvReader.Configuration.RegisterClassMap(new BulkSetAllocationCSVMap(_dbContext,
                    parcelTypeDisplayName));
                csvReader.Read();
                csvReader.ReadHeader();
                var headerNamesDuplicated =
                    csvReader.Context.HeaderRecord.GroupBy(x => x).Where(x => x.Count() > 1).ToList();
                if (headerNamesDuplicated.Any())
                {
                    badRequest = BadRequest(new
                    {
                        validationMessage =
                            $"The following header names appear more than once: {string.Join(", ", headerNamesDuplicated.OrderBy(x => x.Key).Select(x => x.Key))}"
                    });
                    records = null;
                    return false;
                }

                records = csvReader.GetRecords<BulkSetAllocationCSV>().ToList();
            }
            catch (HeaderValidationException e)
            {
                var headerMessage = e.Message.Split('.')[0];
                badRequest = BadRequest(new
                {
                    validationMessage =
                        $"{headerMessage}. Please check that the column name is not missing or misspelled."
                });
                records = null;
                return false;
            }
            catch (MissingFieldException e)
            {
                var headerMessage = e.Message.Split('.')[0];
                badRequest = BadRequest(new
                {
                    validationMessage =
                        $"{headerMessage}. Please check that the column name is not missing or misspelled."
                });
                records = null;
                return false;
            }
            catch
            {
                badRequest = BadRequest(new
                {
                    validationMessage =
                       "There was an error parsing the CSV. Please ensure the file was formatted correctly."
                });
                records = null;
                return false;
            }

            badRequest = null;
            return true;
        }

        private bool ValidateBulkSetAllocationUpload(List<BulkSetAllocationCSV> records, string parcelAllocationTypeDisplayName, out ActionResult badRequest)
        {
            // no null allocation volumes
            var nullAllocationVolumes = records.Where(x => x.AllocationVolume == null).ToList();
            if (nullAllocationVolumes.Any())
            {
                badRequest = BadRequest(new
                {
                    validationMessage =
                        $"The following Account Numbers had no {parcelAllocationTypeDisplayName} Volume entered: " +
                        string.Join(", ", nullAllocationVolumes.Select(x => x.AccountNumber))
                });
                return false;
            }

            // no duplicate account numbers permitted
            var duplicateAccountNumbers = records.GroupBy(x => x.AccountNumber).Where(x => x.Count() > 1)
                .Select(x => x.Key).ToList();

            if (duplicateAccountNumbers.Any())
            {
                badRequest = BadRequest(new
                {
                    validationMessage =
                        "The upload contained multiples rows with these account numbers: " +
                        string.Join(", ", duplicateAccountNumbers)
                });
                return false;
            }

            // all account numbers must match
            var allAccountNumbers = _dbContext.Account.Select(y => y.AccountNumber);
            var unmatchedRecords = records.Where(x => !allAccountNumbers.Contains(x.AccountNumber)).ToList();

            if (unmatchedRecords.Any())
            {
                badRequest = BadRequest(new
                {
                    validationMessage =
                        "The upload contained these account numbers which did not match any record in the system: " +
                        string.Join(", ", unmatchedRecords.Select(x => x.AccountNumber))
                });
                return false;
            }

            badRequest = null;
            return true;
        }
    }

    public sealed class BulkSetAllocationCSVMap : ClassMap<BulkSetAllocationCSV>
    {
        public BulkSetAllocationCSVMap()
        {
            Map(m => m.AccountNumber).Name("Account Number");
            Map(m => m.AllocationVolume).Name("Allocation Volume");
        }

        public BulkSetAllocationCSVMap(RioDbContext dbContext, string parcelAllocationTypeDisplayName)
        {
            Map(m => m.AccountNumber).Name("Account Number");
            Map(m => m.AllocationVolume).Name(parcelAllocationTypeDisplayName + " Volume");
        }
    }
}
