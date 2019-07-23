using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Rio.API.Services;
using Rio.API.Services.Authorization;
using Rio.API.Services.Filter;
using Rio.EFModels.Entities;
using Rio.Models.DataTransferObjects.Offer;
using Rio.Models.DataTransferObjects.Posting;
using Rio.Models.DataTransferObjects.User;

namespace Rio.API.Controllers
{
    public class OfferController : Controller
    {
        private readonly RioDbContext _dbContext;
        private readonly ILogger<OfferController> _logger;
        private readonly KeystoneService _keystoneService;

        public OfferController(RioDbContext dbContext, ILogger<OfferController> logger, KeystoneService keystoneService)
        {
            _dbContext = dbContext;
            _logger = logger;
            _keystoneService = keystoneService;
        }

        [HttpPost("/offers/new/{postingID}")]
        [OfferManageFeature]
        [RequiresValidJSONBodyFilter("Could not parse a valid Offer New JSON object from the Request Body.")]
        public IActionResult New([FromRoute] int postingID, [FromBody] OfferUpsertDto offerUpsertDto)
        {
            var posting = Posting.GetByPostingID(_dbContext, postingID);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (posting.AvailableQuantity < offerUpsertDto.Quantity)
            {
                ModelState.AddModelError("Quantity", "Exceeds remaining balance in posting");
                return BadRequest(ModelState);
            }

            var userDto = GetCurrentUser();
            var offer = Offer.CreateNew(_dbContext, postingID, userDto.UserID, offerUpsertDto);

            // get current balance of posting
            var acreFeetOfAcceptedTrades = Posting.CalculateAcreFeetOfAcceptedTrades(_dbContext, postingID);
            var postingStatusToUpdateTo = (int) PostingStatusEnum.Open;
            if (posting.Quantity == acreFeetOfAcceptedTrades)
            {
                postingStatusToUpdateTo = (int)PostingStatusEnum.Closed;
                // expire all other outstanding offers
                var postingCreateUserID = posting.CreateUser.UserID;
                var activeTradesForPosting = Trade.GetPendingTradesForPostingID(_dbContext, postingID);
                foreach (var activeTrade in activeTradesForPosting)
                {
                    var offerStatus = activeTrade.OfferCreateUserID == postingCreateUserID
                        ? OfferStatusEnum.Rescinded
                        : OfferStatusEnum.Rejected;
                    var offerUpsertDtoForRescindReject = new OfferUpsertDto();
                    offerUpsertDtoForRescindReject.TradeID = activeTrade.TradeID;
                    offerUpsertDtoForRescindReject.Price = activeTrade.Price;
                    offerUpsertDtoForRescindReject.Quantity = activeTrade.Quantity;
                    offerUpsertDtoForRescindReject.OfferStatusID = (int)offerStatus;
                    offerUpsertDtoForRescindReject.OfferNotes = $"Offer {offerStatus} because original posting is now closed";
                    Offer.CreateNew(_dbContext, postingID, postingCreateUserID, offerUpsertDtoForRescindReject);
                }
            }
            Posting.UpdateStatus(_dbContext, postingID,
                new PostingUpdateStatusDto { PostingStatusID = postingStatusToUpdateTo }, posting.Quantity - acreFeetOfAcceptedTrades);

            return Ok(offer);
        }

        [HttpGet("current-user-active-offers/{postingID}")]
        [OfferManageFeature]
        public ActionResult<IEnumerable<OfferDto>> GetActiveOffersForCurrentUserByPosting([FromRoute] int postingID)
        {
            var userDto = GetCurrentUser();
            var offerDtos = Offer.GetActiveOffersFromPostingIDAndUserID(_dbContext, postingID, userDto.UserID);
            return Ok(offerDtos);
        }

        [HttpGet("trade-activity/{userID}")]
        [UserViewFeature]
        public ActionResult<IEnumerable<TradeWithMostRecentOfferDto>> GetActiveTradesForUser([FromRoute] int userID)
        {
            var tradeWithMostRecentOfferDtos = Trade.GetTradeActivityForUserID(_dbContext, userID);
            return Ok(tradeWithMostRecentOfferDtos);
        }

        [HttpGet("trades/{tradeID}")]
        [OfferManageFeature]
        public ActionResult<TradeDto> GetTradeByTradeID([FromRoute] int tradeID)
        {
            var tradeDto = Trade.GetByTradeID(_dbContext, tradeID);
            return Ok(tradeDto);
        }

        [HttpGet("trades/{tradeID}/offers")]
        [OfferManageFeature]
        public ActionResult<IEnumerable<OfferDto>> GetOffersByTradeID([FromRoute] int tradeID)
        {
            var offerDtos = Offer.GetByTradeID(_dbContext, tradeID);
            return Ok(offerDtos);
        }

        private UserDto GetCurrentUser()
        {
            var userGuid = _keystoneService.GetProfile().Payload.UserGuid;
            var userDto = Rio.EFModels.Entities.User.GetByUserGuid(_dbContext, userGuid);
            return userDto;
        }

        [HttpGet("offers/{offerID}")]
        [OfferManageFeature]
        public ActionResult<OfferDto> GetByOfferID([FromRoute] int offerID)
        {
            var offerDto = Offer.GetByOfferID(_dbContext, offerID);
            if (offerDto == null)
            {
                return NotFound();
            }

            return Ok(offerDto);
        }
    }
}