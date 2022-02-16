using System.Collections.Generic;
using System.Linq;
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

        [HttpGet("tags/{tagID}")]
        [ParcelViewFeature]
        public ActionResult<List<TagDto>> GetByIDAsDto([FromRoute] int tagID)
        {
            var tagDto = Tags.GetByIDAsDto(_dbContext, tagID);
            return Ok(tagDto);
        }

        [HttpDelete("tags/{tagID}")]
        [ManagerDashboardFeature]
        public ActionResult DeleteByID([FromRoute] int tagID)
        {
            var tag = _dbContext.Tags.SingleOrDefault(x => x.TagID == tagID);
            if (tag == null)
            {
                return BadRequest();
            }

            Tags.Delete(_dbContext, tag);
            return Ok();
        }
    }
}