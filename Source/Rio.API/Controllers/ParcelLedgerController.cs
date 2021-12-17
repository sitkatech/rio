using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.IIS.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Org.BouncyCastle.Asn1.Sec;
using Rio.API.Services;
using Rio.API.Services.Authorization;
using Rio.EFModels.Entities;
using Rio.Models.DataTransferObjects.ParcelAllocation;

namespace Rio.API.Controllers
{
    [ApiController]
    public class ParcelLedgerController : SitkaController<ParcelLedgerController>
    {
        public ParcelLedgerController(RioDbContext dbContext, ILogger<ParcelLedgerController> logger,
            KeystoneService keystoneService, IOptions<RioConfiguration> rioConfiguration) : base(dbContext, logger,
            keystoneService, rioConfiguration)
        {
        }

        [HttpPost("parcel-ledgers/new")]
        [ParcelManageFeature]
        public IActionResult New([FromBody] ParcelLedgerCreateDto parcelLedgerCreateDto)
        {
            var parcelDto = Parcel.GetByParcelNumberAsDto(_dbContext, parcelLedgerCreateDto.ParcelNumbers[0]); 
            if (parcelDto == null)
            {
                ModelState.AddModelError("ParcelNumber", $"{parcelLedgerCreateDto.ParcelNumbers[0]} is not a valid Parcel APN.");
                return BadRequest(ModelState);
            }

            ValidateEffectiveDate(parcelLedgerCreateDto);
            if (parcelLedgerCreateDto.TransactionTypeID == (int) TransactionTypeEnum.Usage)
            {
                // flip TransactionAmount sign for usage adjustment; usage is negative in the ledgera user-inputted positive value should increase usage sum (and vice versa)
                parcelLedgerCreateDto.TransactionAmount *= -1;
                ValidateUsageAmount(parcelLedgerCreateDto);
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userDto = UserContext.GetUserFromHttpContext(_dbContext, HttpContext);
            ParcelLedgers.CreateNew(_dbContext, parcelDto, parcelLedgerCreateDto, userDto.UserID);
            return Ok();
        }

        [HttpPost("parcel-ledgers/bulk-new")]
        [ParcelManageFeature]
        public IActionResult BulkNew([FromBody] ParcelLedgerCreateDto parcelLedgerCreateDto)
        {
            ValidateEffectiveDate(parcelLedgerCreateDto);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userDto = UserContext.GetUserFromHttpContext(_dbContext, HttpContext);
            var postingCount = ParcelLedgers.BulkCreateNew(_dbContext, parcelLedgerCreateDto, userDto.UserID);
            return Ok(postingCount);
        }

        private void ValidateEffectiveDate(ParcelLedgerCreateDto parcelLedgerCreateDto)
        {
            var earliestWaterYear = WaterYear.List(_dbContext).OrderBy(x => x.Year).First();
            if (parcelLedgerCreateDto.EffectiveDate.Year < earliestWaterYear.Year)
            {
                ModelState.AddModelError("EffectiveDate", 
                    $"Transactions for dates before 1/1/{earliestWaterYear.Year} are not allowed");
            }

            var currentDate = DateTime.Today.AddDays(1).AddSeconds(-1);
            if (DateTime.Compare(parcelLedgerCreateDto.EffectiveDate, currentDate) > 0)
            {
                ModelState.AddModelError("EffectiveDate", "Transactions for future dates are not allowed.");
            }
        }

        private void ValidateUsageAmount(ParcelLedgerCreateDto parcelLedgerCreateDto)
        { 
            if (parcelLedgerCreateDto.TransactionAmount > 0)
            {
                var monthlyUsageSum = ParcelLedgers.GetUsageSumForMonthAndParcelID(_dbContext, parcelLedgerCreateDto.EffectiveDate.Year, parcelLedgerCreateDto.EffectiveDate.Month, parcelLedgerCreateDto.ParcelNumbers[0][0]);
                if (parcelLedgerCreateDto.TransactionAmount + monthlyUsageSum > 0)
                {
                    ModelState.AddModelError("TransactionAmount", 
                        $"Parcel usage for {parcelLedgerCreateDto.EffectiveDate.Month}/{parcelLedgerCreateDto.EffectiveDate.Year} is currently {Math.Round(monthlyUsageSum, 2)}. Usage correction quantity cannot exceed total usage for month.");
                }
            }
        }
    }
}