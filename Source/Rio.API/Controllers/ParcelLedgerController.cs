using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Rio.API.Services;
using Rio.API.Services.Authorization;
using Rio.EFModels.Entities;
using Rio.Models.DataTransferObjects.Parcel;
using Rio.Models.DataTransferObjects.ParcelAllocation;

namespace Rio.API.Controllers
{
    [ApiController]
    public class ParcelLedgerController : SitkaController<ParcelLedgerController>
    {
        public ParcelLedgerController(RioDbContext dbContext, ILogger<ParcelLedgerController> logger, KeystoneService keystoneService, IOptions<RioConfiguration> rioConfiguration) : base(dbContext, logger, keystoneService, rioConfiguration)
        {
        }

        [HttpGet("parcels/{parcelID}/getLedgerEntries")]
        public ActionResult<IEnumerable<ParcelLedgerDto>> GetAllLedgerEntriesByParcelID([FromRoute] int parcelID)
        {
            var parcelLedgerDtos = ParcelLedger.ListLedgerEntriesByParcelID(_dbContext, parcelID);
            return Ok(parcelLedgerDtos);
        }


    }
}
