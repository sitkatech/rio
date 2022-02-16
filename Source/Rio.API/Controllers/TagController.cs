using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Rio.API.Services;
using Rio.API.Services.Authorization;
using Rio.EFModels.Entities;
using Rio.Models.DataTransferObjects;

namespace Rio.API.Controllers
{
    [ApiController]
    public class TagController : SitkaController<TagController>
    {
        public TagController(RioDbContext dbContext, ILogger<TagController> logger, KeystoneService keystoneService, IOptions<RioConfiguration> rioConfiguration) : base(dbContext, logger,
            keystoneService, rioConfiguration)
        {
        }

        [HttpGet("tags")]
        [ParcelViewFeature]
        public ActionResult<List<TagDto>> List()
        {
            var tagDtos = Tags.ListAsDto(_dbContext);
            return Ok(tagDtos);
        }
    }
}