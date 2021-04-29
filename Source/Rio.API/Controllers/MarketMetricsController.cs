using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Rio.API.Services;
using Rio.API.Services.Authorization;
using Rio.EFModels.Entities;
using Rio.Models.DataTransferObjects;
using Rio.Models.DataTransferObjects.Offer;
using Rio.Models.DataTransferObjects.Posting;

namespace Rio.API.Controllers
{
    [ApiController]
    public class MarketMetricsController : SitkaController<MarketMetricsController>
    {
        public MarketMetricsController(RioDbContext dbContext, ILogger<MarketMetricsController> logger, KeystoneService keystoneService, IOptions<RioConfiguration> rioConfiguration) : base(dbContext, logger, keystoneService, rioConfiguration)
        {
        }


        [HttpGet("market-metrics")]
        [ManagerDashboardFeature]
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

        [HttpGet("market-metrics/monthly-trade-activity")]
        [ManagerDashboardFeature]
        public ActionResult<List<TradeActivityByMonthDto>> GetLast12MonthsTradeActivity()
        {
            var now = DateTime.Now;
            var date12MonthsBefore = now.AddMonths(-11);
            var beginDate = new DateTime(date12MonthsBefore.Year, date12MonthsBefore.Month, 1);
            var endDate = new DateTime(now.Year, now.Month, 1).AddMonths(1).AddDays(-1);

            var tradeActivityByMonthDtos = Enumerable.Range(0, 12).Select(a => beginDate.AddMonths(a).AddDays(1))
                .TakeWhile(a => a <= endDate)
                .Select(a => new TradeActivityByMonthDto() { GroupingDate = a, NumberOfTrades = 0, TradeVolume = 0}).ToList();

            var acceptedAndNotCanceledTrades = Trade.GetAllTrades(_dbContext).Where(x =>
                x.TradeStatus.TradeStatusID == (int) TradeStatusEnum.Accepted && !x.BuyerRegistration.IsCanceled &&
                !x.SellerRegistration.IsCanceled && x.OfferDate >= beginDate && x.OfferDate <= now).ToList();
            var tradesGroupedByMonth = acceptedAndNotCanceledTrades.GroupBy(x => new DateTime(x.OfferDate.Year, x.OfferDate.Month, 2)).ToList();
            foreach (var tradeActivityByMonthDto in tradeActivityByMonthDtos)
            {
                var currentTradeActivityByMonthDto = tradesGroupedByMonth.SingleOrDefault(x => x.Key == tradeActivityByMonthDto.GroupingDate);
                if (currentTradeActivityByMonthDto != null)
                {
                    tradeActivityByMonthDto.MaximumPrice = Math.Round(currentTradeActivityByMonthDto.Max(x => x.Price), 2);
                    tradeActivityByMonthDto.MinimumPrice = Math.Round(currentTradeActivityByMonthDto.Min(x => x.Price), 2);
                    tradeActivityByMonthDto.AveragePrice = Math.Round(currentTradeActivityByMonthDto.Average(x => x.Price), 2);
                    tradeActivityByMonthDto.TradeVolume = currentTradeActivityByMonthDto.Sum(x => x.Quantity);
                    tradeActivityByMonthDto.TradeVolume = currentTradeActivityByMonthDto.Sum(x => x.Quantity);
                    tradeActivityByMonthDto.NumberOfTrades = currentTradeActivityByMonthDto.Count();
                }
            }
            return Ok(tradeActivityByMonthDtos);
        }
    }
}