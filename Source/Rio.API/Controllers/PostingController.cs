using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Rio.API.Services;
using Rio.API.Services.Authorization;
using Rio.API.Services.Filter;
using Rio.EFModels.Entities;
using Rio.Models.DataTransferObjects.Posting;

namespace Rio.API.Controllers
{
    public class PostingController : Controller
    {
        private readonly RioDbContext _dbContext;
        private readonly ILogger<PostingController> _logger;
        private readonly KeystoneService _keystoneService;

        public PostingController(RioDbContext dbContext, ILogger<PostingController> logger, KeystoneService keystoneService)
        {
            _dbContext = dbContext;
            _logger = logger;
            _keystoneService = keystoneService;
        }

        [HttpPost("/postings/new")]
        [PostingManageFeature]
        [RequiresValidJSONBodyFilter("Could not parse a valid Posting New JSON object from the Request Body.")]
        public IActionResult New([FromBody] PostingUpsertDto postingUpsertDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var posting = Posting.CreateNew(_dbContext, postingUpsertDto);
            return Ok(posting);
        }

        [HttpGet("postings")]
        [PostingManageFeature]
        public ActionResult<IEnumerable<PostingDto>> List()
        {
            var postingDtos = Posting.List(_dbContext);
            return Ok(postingDtos);
        }

        [HttpGet("postings/{postingID}")]
        [PostingManageFeature]
        public ActionResult<PostingDto> GetByPostingID([FromRoute] int postingID)
        {
            var postingDto = Posting.GetByPostingID(_dbContext, postingID);
            if (postingDto == null)
            {
                return NotFound();
            }

            return Ok(postingDto);
        }

        [HttpPut("postings/{postingID}/update")]
        [PostingManageFeature]
        [RequiresValidJSONBodyFilter("Could not parse a valid Posting Upsert JSON object from the Request Body.")]
        public ActionResult<PostingDto> UpdatePosting([FromRoute] int postingID, [FromBody] PostingUpsertDto postingUpsertDto)
        {
            var postingDto = Posting.GetByPostingID(_dbContext, postingID);
            if (postingDto == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var role = PostingType.GetByPostingTypeID(_dbContext, postingUpsertDto.PostingTypeID);
            if (role == null)
            {
                return NotFound($"Could not find a Posting Type with the ID {postingUpsertDto.PostingTypeID}");
            }

            var updatedPostingDto = Posting.Update(_dbContext, postingID, postingUpsertDto);
            return Ok(updatedPostingDto);
        }

        [HttpDelete("postings/{postingID}/delete")]
        [PostingManageFeature]
        public ActionResult DeletePosting([FromRoute] int postingID)
        {
            var postingDto = Rio.EFModels.Entities.Posting.GetByPostingID(_dbContext, postingID);
            if (postingDto == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Posting.Delete(_dbContext, postingID);
            return Ok();
        }
    }
}