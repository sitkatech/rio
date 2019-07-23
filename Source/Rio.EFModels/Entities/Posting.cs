using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Rio.Models.DataTransferObjects.Offer;
using Rio.Models.DataTransferObjects.Posting;

namespace Rio.EFModels.Entities
{
    public partial class Posting
    {
        public static PostingDto CreateNew(RioDbContext dbContext, PostingUpsertDto postingUpsertDto)
        {
            var posting = new Posting
            {
                PostingTypeID = postingUpsertDto.PostingTypeID, 
                PostingDescription = postingUpsertDto.PostingDescription,
                CreateUserID = postingUpsertDto.CreateUserID,
                PostingDate = DateTime.UtcNow,
                Price = postingUpsertDto.Price,
                Quantity = postingUpsertDto.Quantity,
                AvailableQuantity = postingUpsertDto.Quantity,
                PostingStatusID = (int) PostingStatusEnum.Open
            };

            dbContext.Posting.Add(posting);
            dbContext.SaveChanges();
            dbContext.Entry(posting).Reload();

            return GetByPostingID(dbContext, posting.PostingID);
        }

        public static IEnumerable<PostingDto> List(RioDbContext dbContext)
        {
            var postings = dbContext.Posting
                .Include(x => x.PostingType)
                .Include(x => x.PostingStatus)
                .Include(x => x.CreateUser)
                .AsNoTracking()
                .OrderByDescending(x => x.PostingDate)
                .Select(x => x.AsDto())
                .AsEnumerable();

            return postings;
        }

        public static IEnumerable<PostingDto> ListActive(RioDbContext dbContext)
        {
            var postings = dbContext.Posting
                .Include(x => x.PostingType)
                .Include(x => x.PostingStatus)
                .Include(x => x.CreateUser)
                .AsNoTracking()
                .Where(x => x.PostingStatusID == (int) PostingStatusEnum.Open)
                .OrderByDescending(x => x.PostingDate)
                .Select(x => x.AsDto())
                .AsEnumerable();

            return postings;
        }
        public static IEnumerable<PostingDto> ListByUserID(RioDbContext dbContext, int userID)
        {
            var postings = dbContext.Posting
                .Include(x => x.PostingType)
                .Include(x => x.PostingStatus)
                .Include(x => x.CreateUser)
                .AsNoTracking()
                .Where(x => x.CreateUserID == userID)
                .OrderByDescending(x => x.PostingDate)
                .Select(x => x.AsDto())
                .AsEnumerable();

            return postings;
        }

        public static PostingDto GetByPostingID(RioDbContext dbContext, int postingID)
        {
            var posting = dbContext.Posting
                .Include(x => x.PostingType)
                .Include(x => x.PostingStatus)
                .Include(x => x.CreateUser)
                .AsNoTracking()
                .SingleOrDefault(x => x.PostingID == postingID);

            var postingDto = posting?.AsDto();
            return postingDto;
        }

        public static PostingDto Update(RioDbContext dbContext, int postingID, PostingUpsertDto postingUpsertDto)
        {
            var posting = dbContext.Posting
                .Single(x => x.PostingID == postingID);

            posting.PostingTypeID = postingUpsertDto.PostingTypeID;
            posting.Quantity = postingUpsertDto.Quantity;
            posting.Price = postingUpsertDto.Price;
            posting.PostingDescription = postingUpsertDto.PostingDescription;

            dbContext.SaveChanges();
            dbContext.Entry(posting).Reload();
            return GetByPostingID(dbContext, postingID);
        }

        public static PostingDto UpdateStatus(RioDbContext dbContext, int postingID,
            PostingUpdateStatusDto postingUpdateStatusDto, int? availableQuantity)
        {
            var posting = dbContext.Posting
                .Single(x => x.PostingID == postingID);

            posting.PostingStatusID = postingUpdateStatusDto.PostingStatusID;
            if (availableQuantity.HasValue)
            {
                posting.AvailableQuantity = availableQuantity.Value;
            }
            dbContext.SaveChanges();
            dbContext.Entry(posting).Reload();
            return GetByPostingID(dbContext, postingID);
        }
        public static int CalculateAcreFeetOfAcceptedTrades(RioDbContext dbContext, int postingID)
        {
            var acceptedTrades = dbContext.Trade
                .Include(x => x.Offer).ThenInclude(x => x.OfferStatus)
                .Include(x => x.TradeStatus)
                .Include(x => x.CreateUser)
                .Include(x => x.Posting)
                .AsNoTracking()
                .Where(x => x.PostingID == postingID && x.TradeStatusID == (int) TradeStatusEnum.Accepted)
                .OrderByDescending(x => x.TradeDate)
                .Select(x => x.AsTradeWithMostRecentOfferDto())
                .AsEnumerable();
            return acceptedTrades.Sum(x => x.Quantity);
        }

        public static void Delete(RioDbContext dbContext, int postingID)
        {
            var posting = dbContext.Posting
                .Single(x => x.PostingID == postingID);
            dbContext.Posting.Remove(posting);
        }
    }
}