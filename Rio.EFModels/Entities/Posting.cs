using Microsoft.EntityFrameworkCore;
using Rio.Models.DataTransferObjects.Posting;
using System;
using System.Collections.Generic;
using System.Linq;
using Rio.Models.DataTransferObjects;

namespace Rio.EFModels.Entities
{
    public partial class Posting
    {
        public static PostingDto CreateNew(RioDbContext dbContext, PostingUpsertDto postingUpsertDto)
        {
            var posting = new Posting
            {
                PostingTypeID = postingUpsertDto.PostingTypeID.Value, 
                PostingDescription = postingUpsertDto.PostingDescription,
                CreateAccountID = postingUpsertDto.CreateAccountID.Value,
                CreateUserID = postingUpsertDto.CreateUserID,
                PostingDate = DateTime.UtcNow,
                Price = postingUpsertDto.Price.Value,
                Quantity = postingUpsertDto.Quantity.Value,
                AvailableQuantity = postingUpsertDto.Quantity.Value,
                PostingStatusID = (int) PostingStatusEnum.Open
            };

            dbContext.Postings.Add(posting);
            dbContext.SaveChanges();
            dbContext.Entry(posting).Reload();

            return GetByPostingID(dbContext, posting.PostingID);
        }

        public static IEnumerable<PostingDto> List(RioDbContext dbContext)
        {
            var postings = GetPostingImpl(dbContext)
                .OrderByDescending(x => x.PostingDate)
                .Select(x => x.AsDto())
                .AsEnumerable();

            return postings;
        }

        public static IEnumerable<PostingDto> ListActive(RioDbContext dbContext)
        {
            var postings = GetPostingImpl(dbContext)
                .Where(x => x.PostingStatusID == (int) PostingStatusEnum.Open)
                .OrderByDescending(x => x.PostingDate)
                .Select(x => x.AsDto())
                .AsEnumerable();

            return postings;
        }

        private static IQueryable<Posting> GetPostingImpl(RioDbContext dbContext)
        {
            return dbContext.Postings
                .Include(x => x.CreateAccount)
                .ThenInclude(x => x.AccountUsers)
                .ThenInclude(x => x.User)
                .AsNoTracking();
        }

        public static IEnumerable<PostingDto> ListByAccountID(RioDbContext dbContext, int accountID)
        {
            var postings = GetPostingImpl(dbContext)
                .Where(x => x.CreateAccountID == accountID)
                .OrderByDescending(x => x.PostingDate)
                .Select(x => x.AsDto())
                .AsEnumerable();

            return postings;
        }

        public static PostingDto GetByPostingID(RioDbContext dbContext, int postingID)
        {
            var posting = GetPostingImpl(dbContext).SingleOrDefault(x => x.PostingID == postingID);
            return posting?.AsDto();
        }

        public static PostingDto UpdateStatus(RioDbContext dbContext, int postingID,
            PostingUpdateStatusDto postingUpdateStatusDto, int? availableQuantity)
        {
            var posting = dbContext.Postings
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
            var acceptedTrades = Trade.GetTradeWithOfferDetailsImpl(dbContext)
                .Where(x => x.PostingID == postingID && x.TradeStatusID == (int) TradeStatusEnum.Accepted)
                .OrderByDescending(x => x.TradeDate)
                .Select(x => x.AsTradeWithMostRecentOfferDto())
                .AsEnumerable();
            return acceptedTrades.Sum(x => x.Quantity);
        }

        public static void Delete(RioDbContext dbContext, int postingID)
        {
            var posting = dbContext.Postings
                .Single(x => x.PostingID == postingID);
            dbContext.Postings.Remove(posting);
            dbContext.SaveChanges();
        }

        public static IEnumerable<PostingDetailedDto> ListDetailedByYear(RioDbContext dbContext, int year)
        {
            var postings = dbContext.vPostingDetaileds.Where(x => x.PostingDate.Year == year).OrderByDescending(x => x.PostingDate).ToList()
                .Select(posting =>
                {
                    var postingDetailedDto = new PostingDetailedDto()
                    {
                        PostingID = posting.PostingID,
                        PostingDate = posting.PostingDate,
                        PostingTypeID = posting.PostingTypeID,
                        PostingTypeDisplayName = posting.PostingTypeDisplayName,
                        PostingStatusID = posting.PostingStatusID,
                        PostingStatusDisplayName = posting.PostingStatusDisplayName,
                        PostedByUserID = posting.PostedByUserID,
                        PostedByAccountID = posting.PostedByAccountID,
                        PostedByAccountName = posting.PostedByAccountName,
                        PostedByFirstName = posting.PostedByFirstName,
                        PostedByLastName = posting.PostedByLastName,
                        PostedByEmail = posting.PostedByEmail,
                        Price = posting.Price,
                        Quantity = posting.Quantity,
                        AvailableQuantity = posting.AvailableQuantity,
                        NumberOfOffers = posting.NumberOfOffers,
                    };
                    return postingDetailedDto;
                }).ToList();
            return postings;
        }

        public static Posting GetMostRecentOfferOfType(RioDbContext dbContext, PostingTypeEnum postingTypeEnum)
        {
            var posting = GetPostingImpl(dbContext).Where(x => x.PostingTypeID == (int)postingTypeEnum).OrderByDescending(x => x.PostingDate).FirstOrDefault();
            return posting;
        }

        public static bool HasOpenOfferByAccountID(RioDbContext dbContext, PostingDto posting, int createAccountID)
        {
            return dbContext.Trades.Any(x =>
                x.PostingID == posting.PostingID && 
                x.CreateAccountID == createAccountID &&
                (x.TradeStatusID == (int) TradeStatusEnum.Accepted ||
                x.TradeStatusID == (int) TradeStatusEnum.Countered));
        }

        public static void DeleteAll(RioDbContext dbContext)
        {
            dbContext.Postings.RemoveRange(dbContext.Postings);
            dbContext.SaveChanges();
        }
    }
}
