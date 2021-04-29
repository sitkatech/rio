using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Rio.API.Services;
using Rio.API.Services.Authorization;
using Rio.EFModels.Entities;
using Rio.Models.DataTransferObjects.Offer;
using Rio.Models.DataTransferObjects.Posting;
using System.Collections.Generic;
using Microsoft.Extensions.Options;

namespace Rio.API.Controllers
{
    [ApiController]
    public class PostingController : SitkaController<PostingController>
    {
        private readonly bool _allowTrading;

        public PostingController(RioDbContext dbContext, ILogger<PostingController> logger, KeystoneService keystoneService, IOptions<RioConfiguration> rioConfiguration) : base(dbContext, logger, keystoneService, rioConfiguration)
        {
            _allowTrading = _rioConfiguration.ALLOW_TRADING;
        }

        [HttpPost("/postings/new")]
        [PostingManageFeature]
        public IActionResult New([FromBody] PostingUpsertDto postingUpsertDto)
        {
            if (!_allowTrading)
            {
                return BadRequest();
            }

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
            return RequireNotNullThrowNotFound(postingDto, "Posting", postingID);
        }

        [HttpPut("postings/{postingID}/close")]
        [PostingManageFeature]
        public ActionResult<PostingDto> ClosePosting([FromRoute] int postingID, [FromBody] PostingUpdateStatusDto postingUpdateStatusDto)
        {
            if (!_allowTrading)
            {
                return BadRequest();
            }

            var postingDto = Posting.GetByPostingID(_dbContext, postingID);
            if (ThrowNotFound(postingDto, "Posting", postingID, out var actionResult))
            {
                return actionResult;
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
            if (!_allowTrading)
            {
                return BadRequest();
            }

            var postingDto = Posting.GetByPostingID(_dbContext, postingID);
            if (ThrowNotFound(postingDto, "Posting", postingID, out var actionResult))
            {
                return actionResult;
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Posting.Delete(_dbContext, postingID);
            return Ok();
        }

        [HttpDelete("trades/delete")]
        [TradeDeleteAllFeature]
        public ActionResult DeleteAllTradeActivity()
        {
            WaterTransferRegistrationParcel.DeleteAll(_dbContext);
            WaterTransferRegistration.DeleteAll(_dbContext);
            WaterTransfer.DeleteAll(_dbContext);
            Offer.DeleteAll(_dbContext);
            Trade.DeleteAll(_dbContext);
            Posting.DeleteAll(_dbContext);
            return Ok();
        }

        [HttpGet("postings/{postingID}/trades")]
        [PostingManageFeature]
        public ActionResult<TradeDto> GetTradesByPostingID([FromRoute] int postingID)
        {
            var tradeDto = Trade.GetTradesByPostingID(_dbContext, postingID);
            return Ok(tradeDto);
        }

        [HttpGet("postings-activity/{year}")]
        [PostingManageFeature]
        public ActionResult<IEnumerable<PostingDetailedDto>> ListDetailedByYear([FromRoute] int year)
        {
            var postingDetailedDtos = Posting.ListDetailedByYear(_dbContext, year);
            return Ok(postingDetailedDtos);
        }
    }
}