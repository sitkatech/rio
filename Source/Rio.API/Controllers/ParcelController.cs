using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Rio.API.Services;
using Rio.API.Services.Authorization;
using Rio.EFModels.Entities;
using Rio.Models.DataTransferObjects.Parcel;

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
            var userDtos = Rio.EFModels.Entities.Parcel.List(_dbContext);
            return Ok(userDtos);
        }

        [HttpGet("parcels/{parcelID}")]
        [ParcelManageFeature]
        public ActionResult<ParcelDto> GetByParcelID([FromRoute] int parcelID)
        {
            var userDto = Rio.EFModels.Entities.Parcel.GetByParcelID(_dbContext, parcelID);
            if (userDto == null)
            {
                return NotFound();
            }

            return Ok(userDto);
        }
    }
}