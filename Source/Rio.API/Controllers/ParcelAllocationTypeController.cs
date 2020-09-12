
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Rio.API.Services;
using Rio.EFModels.Entities;
using Rio.API.Util;
using Rio.Models.DataTransferObjects;

namespace Rio.API.Controllers
{
    [ApiController]
    public class ParcelAllocationController : ControllerBase
    {
        private readonly RioDbContext _dbContext;
        private readonly ILogger<ParcelAllocationController> _logger;
        private readonly KeystoneService _keystoneService;

        public ParcelAllocationController(RioDbContext dbContext, ILogger<ParcelAllocationController> logger,
            KeystoneService keystoneService)
        {
            _dbContext = dbContext;
            _logger = logger;
            _keystoneService = keystoneService;
        }

        [HttpGet("/parcel-allocation-types/")]
        public ActionResult<List<ParcelAllocationTypeDto>> GetParcelAllocationTypes()
        {
            var parcelAllocationTypeDtos = ParcelAllocationType.GetParcelAllocationTypes(_dbContext);
            return Ok(parcelAllocationTypeDtos);
        }

        [HttpPut("/parcel-allocation-types/")]
        public IActionResult MergeParcelAllocationTypes([FromBody] List<ParcelAllocationTypeDto> parcelAllocationTypeDtos)
        {
            var updatedParcelAllocationTypes = parcelAllocationTypeDtos.Select(x => new ParcelAllocationType()
            {
                ParcelAllocationTypeName = x.ParcelAllocationTypeName,
                ParcelAllocationTypeID = x.ParcelAllocationTypeID
            }).ToList();

            var existingParcelAllocationTypes = _dbContext.ParcelAllocationType.ToList();
            
            var allInDatabase = _dbContext.ParcelAllocationType;

            existingParcelAllocationTypes.Merge(updatedParcelAllocationTypes, allInDatabase,
                (x, y) => x.ParcelAllocationTypeID == y.ParcelAllocationTypeID,
                (x, y) =>
                {
                    x.ParcelAllocationTypeName = y.ParcelAllocationTypeName;
                });

            _dbContext.SaveChanges();

            return Ok();
        }
    }

}
