using Microsoft.EntityFrameworkCore;
using Rio.Models.DataTransferObjects.Offer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rio.EFModels.Entities
{
    public partial class Trade
    {
        public static TradeDto CreateNew(RioDbContext dbContext, int postingID, int accountID)
        {
            var trade = new Trade
            {
                PostingID = postingID,
                CreateAccountID = accountID,
                TradeDate = DateTime.UtcNow,
                TradeStatusID = (int) TradeStatusEnum.Countered
            };

            // we need to calculate the trade number
            var existingTradesForTheYearCount = dbContext.Trade.AsNoTracking().Count(x => x.TradeDate.Year == trade.TradeDate.Year);
            trade.TradeNumber = $"{trade.TradeDate.Year}-{existingTradesForTheYearCount + 1:D4}";

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

        public static IEnumerable<TradeWithMostRecentOfferDto> GetTradesForYear(RioDbContext dbContext, int year)
        {
            var offers = GetTradeWithOfferDetailsImpl(dbContext).Where(x => x.TradeDate.Year == year)
                .OrderByDescending(x => x.TradeDate)
                .Select(x => x.AsTradeWithMostRecentOfferDto())
                .AsEnumerable();

            return offers;
        }

        public static IEnumerable<TradeWithMostRecentOfferDto> GetTradesForAccountID(RioDbContext dbContext, int accountID)
        {
            var offers = GetTradeWithOfferDetailsImpl(dbContext)
                .Where(x => x.CreateAccountID == accountID || x.Posting.CreateAccountID == accountID)
                .OrderByDescending(x => x.TradeDate)
                .Select(x => x.AsTradeWithMostRecentOfferDto())
                .AsEnumerable();

            return offers;
        }

        public static IQueryable<Trade> GetTradeWithOfferDetailsImpl(RioDbContext dbContext)
        {
            return dbContext.Trade
                .Include(x => x.Offer).ThenInclude(x => x.OfferStatus)
                .Include(x => x.Offer).ThenInclude(x => x.WaterTransfer).ThenInclude(x => x.WaterTransferRegistration).ThenInclude(x => x.Account)
                .Include(x => x.Offer).ThenInclude(x => x.CreateAccount).ThenInclude(x => x.AccountUser).ThenInclude(x => x.User)
                .Include(x => x.TradeStatus)
                .Include(x => x.CreateAccount)
                .Include(x => x.Posting).ThenInclude(x => x.CreateAccount)
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
            var trade = GetTradeImpl(dbContext).SingleOrDefault(x => x.TradeID == tradeID);
            return trade?.AsDto();
        }

        private static IQueryable<Trade> GetTradeImpl(RioDbContext dbContext)
        {
            return dbContext.Trade
                .Include(x => x.TradeStatus)
                .Include(x => x.CreateAccount).ThenInclude(x=>x.AccountUser).ThenInclude(x=>x.User)
                .Include(x=>x.CreateAccount.AccountStatus)
                .Include(x => x.Posting).ThenInclude(x => x.CreateAccount.AccountStatus)
                .Include(x => x.Posting).ThenInclude(x => x.PostingType)
                .Include(x => x.Posting).ThenInclude(x => x.PostingStatus)
                .AsNoTracking();
        }


        public static TradeDto GetByTradeNumber(RioDbContext dbContext, string tradeNumber)
        {
            var trade = GetTradeImpl(dbContext).SingleOrDefault(x => x.TradeNumber == tradeNumber);
            return trade?.AsDto();
        }


        public static IEnumerable<TradeDto> GetTradesByPostingID(RioDbContext dbContext, int postingID)
        {
            var trades = GetTradeImpl(dbContext).Where(x => x.PostingID == postingID).Select(x => x.AsDto());
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