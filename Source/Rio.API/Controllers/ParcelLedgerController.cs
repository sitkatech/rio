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
            var parcelDto = Parcel.GetByParcelNumber(_dbContext, parcelLedgerCreateDto.ParcelNumber);
            if (ThrowNotFound(parcelDto, "Parcel", parcelLedgerCreateDto.ParcelNumber, out var actionResult))
            {
                return BadRequest(new { validationMessage = $"{parcelLedgerCreateDto.ParcelNumber} is not a valid Parcel APN." });
            }

            parcelLedgerCreateDto.ParcelID = parcelDto.ParcelID;

            if (!ValidateNewParcelLedger(parcelLedgerCreateDto, out var badRequestFromValidation))
            {
                return badRequestFromValidation;
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userDto = UserContext.GetUserFromHttpContext(_dbContext, HttpContext);
            var posting = ParcelLedger.CreateNew(_dbContext, parcelLedgerCreateDto, userDto.UserID);
            return Ok(posting);
        }

        private bool ValidateNewParcelLedger(ParcelLedgerCreateDto parcelLedgerCreateDto, out ActionResult badRequest)
        {
            if (parcelLedgerCreateDto.TransactionTypeID == (int) TransactionTypeEnum.ManualAdjustment)
            {
                if (parcelLedgerCreateDto.EffectiveDate > DateTime.UtcNow)
                {
                    badRequest = BadRequest(new { validationMessage = "Transactions to adjust usage for future dates are not allowed." });
                    return false;
                }

                if (!parcelLedgerCreateDto.IsWithdrawal)
                {
                   var monthlyUsageSum = ParcelLedger.getUsageSumForMonthAndParcelID(_dbContext, parcelLedgerCreateDto.EffectiveDate.Year, parcelLedgerCreateDto.EffectiveDate.Month, parcelLedgerCreateDto.ParcelID);
                   if (parcelLedgerCreateDto.TransactionAmount + monthlyUsageSum > 0)
                   {
                       badRequest = BadRequest(new
                       {
                           validationMessage =
                               $"Parcel usage for {parcelLedgerCreateDto.EffectiveDate.Month}/{parcelLedgerCreateDto.EffectiveDate.Year} is currently {Math.Round(monthlyUsageSum, 2)}. Please update quantity for correction so usage is not less than 0."
                       });

                       return false;
                   }
                }
            }

            badRequest = null;
            return true;
        }
    }
}