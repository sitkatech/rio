using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Rio.Models.DataTransferObjects.Offer;

namespace Rio.EFModels.Entities
{
    public partial class Offer
    {
        public static OfferDto CreateNew(RioDbContext dbContext, int postingID, int userID, OfferUpsertDto offerUpsertDto)
        {
            if (!offerUpsertDto.TradeID.HasValue)
            {
                var trade = Trade.CreateNew(dbContext, postingID, userID);
                offerUpsertDto.TradeID = trade.TradeID;
            }

            var offer = new Offer
            {
                TradeID = offerUpsertDto.TradeID.Value,
                OfferNotes = offerUpsertDto.OfferNotes,
                CreateUserID = userID,
                OfferDate = DateTime.UtcNow,
                Price = offerUpsertDto.Price,
                Quantity = offerUpsertDto.Quantity,
                OfferStatusID = offerUpsertDto.OfferStatusID
            };

            dbContext.Offer.Add(offer);
            dbContext.SaveChanges();
            dbContext.Entry(offer).Reload();

            return GetByOfferID(dbContext, offer.OfferID);
        }

        public static IEnumerable<OfferDto> GetActiveOffersFromPostingIDAndUserID(RioDbContext dbContext, int postingID, int userID)
        {
            var offers = dbContext.Offer
                .Include(x => x.OfferStatus)
                .Include(x => x.Trade)
                .Include(x => x.CreateUser)
                .AsNoTracking()
                .Where(x => x.Trade.PostingID == postingID && x.CreateUserID == userID && x.Trade.TradeStatusID == (int) TradeStatusEnum.Open)
                .OrderByDescending(x => x.OfferDate)
                .Select(x => x.AsDto())
                .AsEnumerable();

            return offers;
        }

        public static IEnumerable<OfferDto> GetByTradeID(RioDbContext dbContext, int tradeID)
        {
            var offers = dbContext.Offer
                .Include(x => x.OfferStatus)
                .Include(x => x.Trade).ThenInclude(x => x.TradeStatus)
                .Include(x => x.Trade).ThenInclude(x => x.CreateUser)
                .Include(x => x.CreateUser)
                .AsNoTracking()
                .Where(x => x.TradeID == tradeID)
                .OrderByDescending(x => x.OfferDate)
                .Select(x => x.AsDto())
                .AsEnumerable();

            return offers;
        }

        public static OfferDto GetByOfferID(RioDbContext dbContext, int offerID)
        {
            var offer = dbContext.Offer
                .Include(x => x.OfferStatus)
                .Include(x => x.Trade).ThenInclude(x => x.TradeStatus)
                .Include(x => x.Trade).ThenInclude(x => x.CreateUser)
                .Include(x => x.CreateUser)
                .AsNoTracking()
                .SingleOrDefault(x => x.OfferID == offerID);

            return offer?.AsDto();
        }
    }
}