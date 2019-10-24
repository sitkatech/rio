using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Rio.Models.DataTransferObjects;
using Rio.Models.DataTransferObjects.Offer;
using Rio.Models.DataTransferObjects.Posting;
using Rio.Models.DataTransferObjects.WaterTransfer;

namespace Rio.EFModels.Entities
{
    public partial class WaterTransfer
    {
        public static WaterTransferDto CreateNew(RioDbContext dbContext, OfferDto offerDto, TradeDto tradeDto, PostingDto postingDto)
        {
            var waterTransfer = new WaterTransfer
            {
                OfferID = offerDto.OfferID,
                AcreFeetTransferred = offerDto.Quantity,
                TransferDate = offerDto.OfferDate
            };

            var waterTransferRegistrationBuyer = new WaterTransferRegistration()
            {
                WaterTransfer = waterTransfer,
                WaterTransferTypeID = (int) WaterTransferTypeEnum.Buying,
                StatusDate = DateTime.Now,
                WaterTransferRegistrationStatusID = (int)WaterTransferRegistrationStatusEnum.Pending
            };
            var waterTransferRegistrationSeller = new WaterTransferRegistration()
            {
                WaterTransfer = waterTransfer,
                WaterTransferTypeID = (int) WaterTransferTypeEnum.Selling,
                StatusDate = DateTime.Now,
                WaterTransferRegistrationStatusID = (int) WaterTransferRegistrationStatusEnum.Pending
            };

            if (postingDto.PostingType.PostingTypeID == (int) PostingTypeEnum.OfferToSell)
            {
                waterTransferRegistrationSeller.UserID = postingDto.CreateUser.UserID;
                waterTransferRegistrationBuyer.UserID = tradeDto.CreateUser.UserID;
            }
            else
            {
                waterTransferRegistrationSeller.UserID = tradeDto.CreateUser.UserID;
                waterTransferRegistrationBuyer.UserID = postingDto.CreateUser.UserID;
            }

            dbContext.WaterTransfer.Add(waterTransfer);
            dbContext.WaterTransferRegistration.Add(waterTransferRegistrationBuyer);
            dbContext.WaterTransferRegistration.Add(waterTransferRegistrationSeller);
            dbContext.SaveChanges();
            dbContext.Entry(waterTransfer).Reload();

            return GetByWaterTransferID(dbContext, waterTransfer.WaterTransferID);
        }

        public static IEnumerable<WaterTransferDto> ListByUserID(RioDbContext dbContext, int userID)
        {
            var waterTransfers = GetWaterTransfersImpl(dbContext)
                .Where(x => x.WaterTransferRegistration.Any(y => y.UserID == userID))
                .OrderByDescending(x => x.TransferDate)
                .Select(x => x.AsDto())
                .AsEnumerable();

            return waterTransfers;
        }

        private static IQueryable<WaterTransfer> GetWaterTransfersImpl(RioDbContext dbContext)
        {
            return dbContext.WaterTransfer
                .Include(x => x.WaterTransferRegistration).ThenInclude(x => x.User)
                .Include(x => x.Offer).ThenInclude(x => x.Trade)
                .AsNoTracking();
        }

        public static WaterTransferDto GetByWaterTransferID(RioDbContext dbContext, int waterTransferID)
        {
            var waterTransfer = GetWaterTransfersImpl(dbContext).SingleOrDefault(x => x.WaterTransferID == waterTransferID);
            return waterTransfer?.AsDto();
        }

        public static List<ErrorMessage> ValidateConfirmTransfer(WaterTransferRegistrationDto waterTransferRegistrationDto, WaterTransferDto waterTransferDto)
        {
            var result = new List<ErrorMessage>();

            if(waterTransferRegistrationDto.WaterTransferTypeID == (int) WaterTransferTypeEnum.Selling && waterTransferRegistrationDto.UserID != waterTransferDto.SellerRegistration.User.UserID)
            {
                result.Add(new ErrorMessage() { Message = "Confirming user does not match seller." });
            }

            if (waterTransferRegistrationDto.WaterTransferTypeID == (int) WaterTransferTypeEnum.Buying && waterTransferRegistrationDto.UserID != waterTransferDto.BuyerRegistration.User.UserID)
            {
                result.Add(new ErrorMessage() { Message = "Confirming user does not match buyer." });
            }

            return result;
        }

        public static WaterTransferDto ChangeWaterRegistrationStatus(RioDbContext dbContext, int waterTransferID,
            WaterTransferRegistrationDto waterTransferRegistrationDto,
            WaterTransferRegistrationStatusEnum waterTransferRegistrationStatusEnum)
        {
            var waterTransferRegistration = dbContext.WaterTransferRegistration
                .Single(x =>
                    x.WaterTransferID == waterTransferID &&
                    x.WaterTransferTypeID == waterTransferRegistrationDto.WaterTransferTypeID);
            waterTransferRegistration.WaterTransferRegistrationStatusID = (int) waterTransferRegistrationStatusEnum;
            waterTransferRegistration.StatusDate = DateTime.Now;
            dbContext.SaveChanges();
            dbContext.Entry(waterTransferRegistration).Reload();
            return GetByWaterTransferID(dbContext, waterTransferID);
        }

        public static List<ErrorMessage> ValidateCancelTransfer(WaterTransferRegistrationDto waterTransferRegistrationDto, WaterTransferDto waterTransferDto)
        {
            var result = new List<ErrorMessage>();

            if(waterTransferDto.BuyerRegistration.IsRegistered || waterTransferDto.SellerRegistration.IsRegistered)
            {
                result.Add(new ErrorMessage() { Message = "Cannot cancel transfer because one of the parties has already registered this transfer." });
            }

            if(waterTransferRegistrationDto.WaterTransferTypeID == (int) WaterTransferTypeEnum.Selling && waterTransferRegistrationDto.UserID != waterTransferDto.SellerRegistration.User.UserID)
            {
                result.Add(new ErrorMessage() { Message = "Canceling user does not match seller." });
            }

            if (waterTransferRegistrationDto.WaterTransferTypeID == (int) WaterTransferTypeEnum.Buying && waterTransferRegistrationDto.UserID != waterTransferDto.BuyerRegistration.User.UserID)
            {
                result.Add(new ErrorMessage() { Message = "Canceling user does not match buyer." });
            }

            return result;
        }

        public static WaterTransferDto GetMostRecentRegistered(RioDbContext dbContext)
        {
            var waterTransfer = GetWaterTransfersImpl(dbContext).Where(x => x.WaterTransferRegistration.All(y => y.WaterTransferRegistrationStatusID == (int) WaterTransferRegistrationStatusEnum.Registered)).OrderByDescending(x => x.WaterTransferRegistration.Max(y => y.StatusDate)).FirstOrDefault();
            return waterTransfer?.AsDto();
        }
    }
}