using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
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
            return dbContext.Posting
                .Include(x => x.PostingType)
                .Include(x => x.PostingStatus)
                .Include(x => x.CreateUser)
                .AsNoTracking();
        }

        public static IEnumerable<PostingDto> ListByUserID(RioDbContext dbContext, int userID)
        {
            var postings = GetPostingImpl(dbContext)
                .Where(x => x.CreateUserID == userID)
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
            var acceptedTrades = Rio.EFModels.Entities.Trade.GetTradeWithOfferDetailsImpl(dbContext)
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
            dbContext.SaveChanges();
        }

        public static IEnumerable<PostingDetailedDto> ListDetailedByYear(RioDbContext dbContext, int year)
        {
            // right now we are assuming a parcel can only be associated to one user
            var parcels = dbContext.vPostingDetailed.Where(x => x.PostingDate.Year == year).OrderByDescending(x => x.PostingDate).ToList()
                .Select(posting =>
                {
                    var userDetailedDto = new PostingDetailedDto()
                    {
                        PostingID = posting.PostingID,
                        PostingDate = posting.PostingDate,
                        PostingTypeID = posting.PostingTypeID,
                        PostingTypeDisplayName = posting.PostingTypeDisplayName,
                        PostingStatusID = posting.PostingStatusID,
                        PostingStatusDisplayName = posting.PostingStatusDisplayName,
                        PostedByUserID = posting.PostedByUserID,
                        PostedByFirstName = posting.PostedByFirstName,
                        PostedByLastName = posting.PostedByLastName,
                        PostedByEmail = posting.PostedByEmail,
                        Price = posting.Price,
                        Quantity = posting.Quantity,
                        AvailableQuantity = posting.AvailableQuantity,
                        NumberOfOffers = posting.NumberOfOffers,
                    };
                    return userDetailedDto;
                }).ToList();
            return parcels;
        }

        public static PostingDto GetMostRecentOfferOfType(RioDbContext dbContext, PostingTypeEnum postingTypeEnum)
        {
            var offer = GetPostingImpl(dbContext).Where(x => x.PostingTypeID == (int)postingTypeEnum).OrderByDescending(x => x.PostingDate).FirstOrDefault();
            return offer?.AsDto();
        }
    }
}