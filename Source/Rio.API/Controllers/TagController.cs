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
        public ActionResult<List<TagDto>> GetByID([FromRoute] int tagID)
        {
            var tagDto = Tags.GetByIDAsDto(_dbContext, tagID);
            return Ok(tagDto);
        }

        [HttpGet("tags/listByParcelID/{parcelID}")]
        [ParcelViewFeature]
        public ActionResult<List<TagDto>> ListByParcelID([FromRoute] int parcelID)
        {
            var tagDtos = Tags.ListByParcelIDAsDto(_dbContext, parcelID);
            return Ok(tagDtos);
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

        [HttpPost("tags/tagParcel/{parcelID}")]
        [ManagerDashboardFeature]
        public ActionResult TagParcelByParcelID([FromRoute] int parcelID, [FromBody] TagDto tagDto)
        {
            if (string.IsNullOrWhiteSpace(tagDto.TagName))
            {
                ModelState.AddModelError("Tag", "Whitespace cannot be used as a tag.");
                return BadRequest(ModelState);
            }

            var parcel = _dbContext.Parcels.SingleOrDefault(x => x.ParcelID == parcelID);
            if (parcel == null)
            {
                return BadRequest();
            }

            var tag = _dbContext.Tags.SingleOrDefault(x => x.TagName == tagDto.TagName);
            if (tag == null)
            {
                tag = Tags.Create(_dbContext, tagDto);
            }
            else
            {
                var existingParcelTag = _dbContext.ParcelTags.Where(x => x.ParcelID == parcelID && x.TagID == tag.TagID);
                if (existingParcelTag.Any())
                {
                    ModelState.AddModelError("Tag", $"{tag.TagName} tag has already been applied to this parcel.");
                    return BadRequest(ModelState);
                }
            }

            Tags.TagParcelByIDAndParcelID(_dbContext, tag.TagID, parcelID);

            return Ok();
        }

        [HttpDelete("tags/{tagID}/removeTagFromParcel/{parcelID}")]
        [ManagerDashboardFeature]
        public ActionResult RemoveTagFromParcel([FromRoute] int tagID, [FromRoute] int parcelID)
        {
            var tag = _dbContext.Tags.SingleOrDefault(x => x.TagID == tagID);
            if (tag == null)
            {
                return BadRequest();
            }

            var parcelTagToDelete = _dbContext.ParcelTags.FirstOrDefault(x => x.ParcelID == parcelID && x.TagID == tagID);
            if (parcelTagToDelete == null)
            {
                return BadRequest();
            }

            _dbContext.ParcelTags.Remove(parcelTagToDelete);
            _dbContext.SaveChanges();

            return Ok();
        }
    }
}