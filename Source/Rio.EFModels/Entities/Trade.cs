using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Rio.Models.DataTransferObjects.Offer;

namespace Rio.EFModels.Entities
{
    public partial class Trade
    {
        public static TradeDto CreateNew(RioDbContext dbContext, int postingID, int userID)
        {
            var trade = new Trade
            {
                PostingID = postingID,
                CreateUserID = userID,
                TradeDate = DateTime.UtcNow,
                TradeStatusID = (int) TradeStatusEnum.Countered
            };

            dbContext.Trade.Add(trade);
            dbContext.SaveChanges();
            dbContext.Entry(trade).Reload();

            return GetByTradeID(dbContext, trade.TradeID);
        }

        public static IEnumerable<TradeWithMostRecentOfferDto> GetAllTrades(RioDbContext dbContext)
        {
            var offers = GetTradeWithOfferDetailsImpl(dbContext)
                .OrderByDescending(x => x.TradeDate)
                .Select(x => x.AsTradeWithMostRecentOfferDto())
                .AsEnumerable();

            return offers;
        }

        public static IEnumerable<TradeWithMostRecentOfferDto> GetTradesForUserID(RioDbContext dbContext, int userID)
        {
            var offers = GetTradeWithOfferDetailsImpl(dbContext)
                .Where(x => x.CreateUserID == userID || x.Posting.CreateUserID == userID)
                .OrderByDescending(x => x.TradeDate)
                .Select(x => x.AsTradeWithMostRecentOfferDto())
                .AsEnumerable();

            return offers;
        }

        public static IQueryable<Trade> GetTradeWithOfferDetailsImpl(RioDbContext dbContext)
        {
            return dbContext.Trade
                .Include(x => x.Offer).ThenInclude(x => x.OfferStatus)
                .Include(x => x.Offer).ThenInclude(x => x.WaterTransfer)
                .Include(x => x.Offer).ThenInclude(x => x.CreateUser)
                .Include(x => x.TradeStatus)
                .Include(x => x.CreateUser)
                .Include(x => x.Posting).ThenInclude(x => x.CreateUser)
                .AsNoTracking();
        }

        public static IEnumerable<TradeWithMostRecentOfferDto> GetPendingTradesForPostingID(RioDbContext dbContext, int postingID)
        {
            var offers = GetTradeWithOfferDetailsImpl(dbContext)
                .Where(x => x.TradeStatusID == (int) TradeStatusEnum.Countered && x.PostingID == postingID)
                .OrderByDescending(x => x.TradeDate)
                .Select(x => x.AsTradeWithMostRecentOfferDto())
                .AsEnumerable();

            return offers;
        }

        public static TradeDto GetByTradeID(RioDbContext dbContext, int tradeID)
        {
            var trade = dbContext.Trade
                .Include(x => x.TradeStatus)
                .Include(x => x.CreateUser)
                .Include(x => x.Posting).ThenInclude(x => x.CreateUser)
                .Include(x => x.Posting).ThenInclude(x => x.PostingType)
                .Include(x => x.Posting).ThenInclude(x => x.PostingStatus)
                .AsNoTracking()
                .SingleOrDefault(x => x.TradeID == tradeID);

            return trade?.AsDto();
        }


        public static IEnumerable<TradeDto> GetTradesByPostingID(RioDbContext dbContext, int postingID)
        {
            var trades = dbContext.Trade
                .Include(x => x.TradeStatus)
                .Include(x => x.CreateUser)
                .Include(x => x.Posting).ThenInclude(x => x.CreateUser)
                .Include(x => x.Posting).ThenInclude(x => x.PostingType)
                .Include(x => x.Posting).ThenInclude(x => x.PostingStatus)
                .AsNoTracking()
                .Where(x => x.PostingID == postingID).Select(x => x.AsDto());

            return trades;
        }

        public static TradeDto Update(RioDbContext dbContext, int tradeID, TradeStatusEnum tradeStatusEnum)
        {
            var trade = dbContext.Trade
                .Single(x => x.TradeID == tradeID);

            trade.TradeStatusID = (int) tradeStatusEnum;

            dbContext.SaveChanges();
            dbContext.Entry(trade).Reload();
            return GetByTradeID(dbContext, tradeID);
        }
    }
}