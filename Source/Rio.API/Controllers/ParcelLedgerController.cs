using System;
using System.Collections.Generic;
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
            var parcelDto = Parcel.GetByParcelNumberAsDto(_dbContext, parcelLedgerCreateDto.ParcelNumber);
            if (parcelDto == null)
            {
                ModelState.AddModelError("ParcelNumber", $"{parcelLedgerCreateDto.ParcelNumber} is not a valid Parcel APN.");
                return BadRequest(ModelState);
            }

            parcelLedgerCreateDto.ParcelID = parcelDto.ParcelID;
            ValidateNewParcelLedger(parcelLedgerCreateDto);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userDto = UserContext.GetUserFromHttpContext(_dbContext, HttpContext);
            var posting = ParcelLedger.CreateNew(_dbContext, parcelLedgerCreateDto, userDto.UserID);
            return Ok(posting);
        }

        private void ValidateNewParcelLedger(ParcelLedgerCreateDto parcelLedgerCreateDto)
        {
            if (parcelLedgerCreateDto.TransactionTypeID == (int) TransactionTypeEnum.ManualAdjustment)
            {
                if (parcelLedgerCreateDto.EffectiveDate > DateTime.UtcNow)
                {
                    ModelState.AddModelError("EffectiveDate","Transactions to adjust usage for future dates are not allowed.");
                }

                if (!parcelLedgerCreateDto.IsWithdrawal)
                {
                   var monthlyUsageSum = ParcelLedger.GetUsageSumForMonthAndParcelID(_dbContext, parcelLedgerCreateDto.EffectiveDate.Year, parcelLedgerCreateDto.EffectiveDate.Month, parcelLedgerCreateDto.ParcelID);
                   if (parcelLedgerCreateDto.TransactionAmount + monthlyUsageSum > 0)
                   {
                       ModelState.AddModelError("TransactionAmount", $"Parcel usage for {parcelLedgerCreateDto.EffectiveDate.Month}/{parcelLedgerCreateDto.EffectiveDate.Year} is currently {Math.Round(monthlyUsageSum, 2)}. Please update quantity for correction so usage is not less than 0.");
                   }
                }
            }
        }
    }
}