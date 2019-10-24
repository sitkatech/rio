using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NetTopologySuite.Operation.Valid;
using Rio.API.Services;
using Rio.API.Services.Authorization;
using Rio.EFModels.Entities;
using Rio.Models.DataTransferObjects;

namespace Rio.API.Controllers
{
    [ApiController]
    public class MarketMetricsController : ControllerBase
    {
        private readonly RioDbContext _dbContext;
        private readonly ILogger<RoleController> _logger;
        private readonly KeystoneService _keystoneService;

        public MarketMetricsController(RioDbContext dbContext, ILogger<RoleController> logger, KeystoneService keystoneService)
        {
            _dbContext = dbContext;
            _logger = logger;
            _keystoneService = keystoneService;
        }

        [HttpGet("market-metrics")]
        [UserManageFeature]
        public ActionResult<MarketMetricsDto> Get()
        {
            var marketMetricsDto = new MarketMetricsDto();
            var mostRecentOfferToBuy = Offer.GetMostRecentOfferOfType(_dbContext, PostingTypeEnum.OfferToBuy);
            var mostRecentOfferToSell = Offer.GetMostRecentOfferOfType(_dbContext, PostingTypeEnum.OfferToSell);
            marketMetricsDto.MostRecentOfferToBuy = mostRecentOfferToBuy;
            marketMetricsDto.MostRecentOfferToSell = mostRecentOfferToSell;
            var postings = Posting.List(_dbContext).ToList();
            marketMetricsDto.TotalBuyVolume = postings.Where(x => x.PostingType.PostingTypeID == (int) PostingTypeEnum.OfferToBuy).Sum(x => x.Quantity);
            marketMetricsDto.TotalSellVolume = postings.Where(x => x.PostingType.PostingTypeID == (int) PostingTypeEnum.OfferToSell).Sum(x => x.Quantity);
            var waterTransfer = WaterTransfer.GetMostRecentRegistered(_dbContext);
            marketMetricsDto.MostRecentWaterTransfer = waterTransfer;
            return Ok(marketMetricsDto);
        }
    }
}