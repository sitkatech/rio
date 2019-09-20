using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Rio.API.Services;
using Rio.API.Services.Authorization;
using Rio.API.Services.Filter;
using Rio.EFModels.Entities;
using Rio.Models.DataTransferObjects.Offer;
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
            var postingDtos = Posting.ListActive(_dbContext);
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

        [HttpPut("postings/{postingID}/close")]
        [PostingManageFeature]
        [RequiresValidJSONBodyFilter("Could not parse a valid Posting Update Status JSON object from the Request Body.")]
        public ActionResult<PostingDto> ClosePosting([FromRoute] int postingID, [FromBody] PostingUpdateStatusDto postingUpdateStatusDto)
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

            var updatedPostingDto = Posting.UpdateStatus(_dbContext, postingID, postingUpdateStatusDto, null);
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

        [HttpGet("postings/{postingID}/trades")]
        [PostingManageFeature]
        public ActionResult<TradeDto> GetTradesByPostingID([FromRoute] int postingID)
        {
            var tradeDto = Trade.GetTradesByPostingID(_dbContext, postingID);
            return Ok(tradeDto);
        }

        [HttpGet("postings-activity")]
        [PostingManageFeature]
        public ActionResult<IEnumerable<PostingDetailedDto>> ListDetailed()
        {
            var postingDetailedDtos = Posting.ListDetailed(_dbContext);
            return Ok(postingDetailedDtos);
        }
    }
}