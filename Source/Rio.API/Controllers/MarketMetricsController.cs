using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Rio.API.Services;
using Rio.API.Services.Authorization;
using Rio.EFModels.Entities;
using Rio.Models.DataTransferObjects;
using Rio.Models.DataTransferObjects.Offer;
using Rio.Models.DataTransferObjects.Posting;

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
            var mostRecentPostingToBuy = Posting.GetMostRecentOfferOfType(_dbContext, PostingTypeEnum.OfferToBuy);
            var mostRecentPostingToSell = Posting.GetMostRecentOfferOfType(_dbContext, PostingTypeEnum.OfferToSell);
            SetMostRecentOfferOfType(mostRecentOfferToBuy, mostRecentPostingToBuy, marketMetricsDto, x =>  x.MostRecentOfferToBuyQuantity, x =>  x.MostRecentOfferToBuyPrice);
            SetMostRecentOfferOfType(mostRecentOfferToSell, mostRecentPostingToSell, marketMetricsDto, x =>  x.MostRecentOfferToSellQuantity, x =>  x.MostRecentOfferToSellPrice);
            var postings = Posting.List(_dbContext).ToList();
            marketMetricsDto.TotalBuyVolume = postings.Where(x => x.PostingType.PostingTypeID == (int) PostingTypeEnum.OfferToBuy).Sum(x => x.Quantity);
            marketMetricsDto.TotalSellVolume = postings.Where(x => x.PostingType.PostingTypeID == (int) PostingTypeEnum.OfferToSell).Sum(x => x.Quantity);
            var waterTransfer = WaterTransfer.GetMostRecentRegistered(_dbContext);
            marketMetricsDto.MostRecentWaterTransfer = waterTransfer;
            return Ok(marketMetricsDto);
        }

        private static void SetMostRecentOfferOfType(OfferDto mostRecentOffer, PostingDto mostRecentPosting,
            MarketMetricsDto marketMetricsDto, Expression<Func<MarketMetricsDto, int?>> quantityFunc, Expression<Func<MarketMetricsDto, decimal?>> priceFunc)
        {
            var quantityExpression = (MemberExpression) quantityFunc.Body;
            var quantityProperty = (PropertyInfo) quantityExpression.Member;
            var priceExpression = (MemberExpression) priceFunc.Body;
            var priceProperty = (PropertyInfo) priceExpression.Member;
            if (mostRecentOffer == null && mostRecentPosting == null)
            {
                quantityProperty.SetValue(marketMetricsDto, null);
                priceProperty.SetValue(marketMetricsDto, null);
            }
            else
            {
                var mostRecentOfferDate = mostRecentOffer?.OfferDate;
                var mostRecentPostingDate = mostRecentPosting?.PostingDate;
                if (mostRecentPostingDate != null && mostRecentPostingDate > mostRecentOfferDate)
                {
                    quantityProperty.SetValue(marketMetricsDto, mostRecentPosting.Quantity);
                    priceProperty.SetValue(marketMetricsDto, mostRecentPosting.Price);
                }
                else
                {
                    quantityProperty.SetValue(marketMetricsDto, mostRecentOffer?.Quantity);
                    priceProperty.SetValue(marketMetricsDto, mostRecentOffer?.Price);
                }
            }
        }
    }
}