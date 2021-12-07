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
            
            if (parcelLedgerCreateDto.ParcelNumbers.Count == 1)
            {
                // manual ParcelNumber entry is only allowed for single transactions, so this check is only needed when ParcelNumbers count is 1
                var parcelDto = Parcel.GetByParcelNumberAsDto(_dbContext, parcelLedgerCreateDto.ParcelNumbers[0]);
                if (parcelDto == null)
                {
                    ModelState.AddModelError("ParcelNumber", $"{parcelLedgerCreateDto.ParcelNumbers[0]} is not a valid Parcel APN.");
                    return BadRequest(ModelState);
                }

                // validates usage adjustments, which are only allowed via single transaction
                ValidateNewParcelLedger(parcelLedgerCreateDto);

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
            }

            var userDto = UserContext.GetUserFromHttpContext(_dbContext, HttpContext);
            var postings = ParcelLedgers.CreateNew(_dbContext, parcelLedgerCreateDto, userDto.UserID);
            return Ok(postings);
        }

        private void ValidateNewParcelLedger(ParcelLedgerCreateDto parcelLedgerCreateDto)
        {
            if (parcelLedgerCreateDto.TransactionTypeID == (int) TransactionTypeEnum.Usage)
            {
                if (parcelLedgerCreateDto.EffectiveDate > DateTime.UtcNow)
                {
                    ModelState.AddModelError("EffectiveDate","Transactions to adjust usage for future dates are not allowed.");
                }

                if (parcelLedgerCreateDto.TransactionAmount > 0)
                {
                   var monthlyUsageSum = ParcelLedgers.GetUsageSumForMonthAndParcelID(_dbContext, parcelLedgerCreateDto.EffectiveDate.Year, parcelLedgerCreateDto.EffectiveDate.Month, parcelLedgerCreateDto.ParcelNumbers[0][0]);
                   if (parcelLedgerCreateDto.TransactionAmount + monthlyUsageSum > 0)
                   {
                       ModelState.AddModelError("TransactionAmount", 
                           $"Parcel usage for {parcelLedgerCreateDto.EffectiveDate.Month}/{parcelLedgerCreateDto.EffectiveDate.Year} is currently {Math.Round(monthlyUsageSum, 2)}. Please update quantity for correction so usage is not less than 0.");
                   }
                }
            }
        }
    }
}