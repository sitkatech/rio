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
                TradeStatusID = (int) TradeStatusEnum.Open
            };

            dbContext.Trade.Add(trade);
            dbContext.SaveChanges();
            dbContext.Entry(trade).Reload();

            return GetByTradeID(dbContext, trade.TradeID);
        }

        public static IEnumerable<TradeWithMostRecentOfferDto> GetActiveTradesForUserID(RioDbContext dbContext, int userID)
        {
            var offers = dbContext.Trade
                .Include(x => x.Offer).ThenInclude(x => x.OfferStatus)
                .Include(x => x.TradeStatus)
                .Include(x => x.CreateUser)
                .Include(x => x.Posting).ThenInclude(x => x.CreateUser)
                .Include(x => x.Posting).ThenInclude(x => x.PostingType)
                .Include(x => x.Posting).ThenInclude(x => x.PostingStatus)
                .AsNoTracking()
                .Where(x => x.CreateUserID == userID || x.Posting.CreateUserID == userID)
                .OrderByDescending(x => x.TradeDate)
                .Select(x => x.AsTradeWithMostRecentOfferDto())
                .AsEnumerable();

            return offers;
        }
        public static IEnumerable<TradeWithMostRecentOfferDto> GetPendingTradesForPostingID(RioDbContext dbContext, int postingID)
        {
            var offers = dbContext.Trade
                .Include(x => x.Offer).ThenInclude(x => x.OfferStatus)
                .Include(x => x.TradeStatus)
                .Include(x => x.CreateUser)
                .Include(x => x.Posting).ThenInclude(x => x.CreateUser)
                .Include(x => x.Posting).ThenInclude(x => x.PostingType)
                .Include(x => x.Posting).ThenInclude(x => x.PostingStatus)
                .AsNoTracking()
                .Where(x => x.TradeStatusID == (int) TradeStatusEnum.Open && x.PostingID == postingID)
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

            var tradeDto = trade?.AsDto();
            return tradeDto;
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

        public static void Delete(RioDbContext dbContext, int tradeID)
        {
            var trade = dbContext.Trade
                .Single(x => x.TradeID == tradeID);
            dbContext.Trade.Remove(trade);
        }
    }
}