using Microsoft.EntityFrameworkCore;
using Rio.Models.DataTransferObjects;
using Rio.Models.DataTransferObjects.WaterTransfer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rio.EFModels.Entities
{
    public partial class WaterTransfer
    {
        public static WaterTransferDetailedDto CreateNew(RioDbContext dbContext, OfferDto offerDto, TradeDto tradeDto, PostingDto postingDto)
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
                WaterTransferTypeID = (int)WaterTransferTypeEnum.Buying,
                StatusDate = DateTime.Now,
                WaterTransferRegistrationStatusID = (int)WaterTransferRegistrationStatusEnum.Pending
            };
            var waterTransferRegistrationSeller = new WaterTransferRegistration()
            {
                WaterTransfer = waterTransfer,
                WaterTransferTypeID = (int)WaterTransferTypeEnum.Selling,
                StatusDate = DateTime.Now,
                WaterTransferRegistrationStatusID = (int)WaterTransferRegistrationStatusEnum.Pending
            };

            if (postingDto.PostingType.PostingTypeID == (int)PostingTypeEnum.OfferToSell)
            {
                waterTransferRegistrationSeller.AccountID = postingDto.CreateAccount.AccountID;
                waterTransferRegistrationBuyer.AccountID = tradeDto.CreateAccount.AccountID;
            }
            else
            {
                waterTransferRegistrationSeller.AccountID = tradeDto.CreateAccount.AccountID;
                waterTransferRegistrationBuyer.AccountID = postingDto.CreateAccount.AccountID;
            }

            dbContext.WaterTransfers.Add(waterTransfer);
            dbContext.WaterTransferRegistrations.Add(waterTransferRegistrationBuyer);
            dbContext.WaterTransferRegistrations.Add(waterTransferRegistrationSeller);
            dbContext.SaveChanges();
            dbContext.Entry(waterTransfer).Reload();

            return GetByWaterTransferIDAsWaterTransferDetailedDto(dbContext, waterTransfer.WaterTransferID);
        }

        public static IEnumerable<WaterTransferDto> ListByAccountID(RioDbContext dbContext, int accountID)
        {
            var waterTransfers = GetWaterTransfersImpl(dbContext)
                .Where(x => x.WaterTransferRegistrations.Any(y => y.AccountID == accountID))
                .OrderByDescending(x => x.TransferDate)
                .Select(x => x.AsDto())
                .AsEnumerable();

            return waterTransfers;
        }

        private static IQueryable<WaterTransfer> GetWaterTransfersImpl(RioDbContext dbContext)
        {
            return dbContext.WaterTransfers
                .Include(x => x.WaterTransferRegistrations).ThenInclude(x => x.Account).ThenInclude(x => x.AccountUsers).ThenInclude(x => x.User)
                .Include(x => x.Offer).ThenInclude(x => x.Trade)
                .AsNoTracking();
        }

        public static WaterTransferDetailedDto GetByWaterTransferIDAsWaterTransferDetailedDto(RioDbContext dbContext, int waterTransferID)
        {
            var waterTransfer = GetWaterTransfersImpl(dbContext).SingleOrDefault(x => x.WaterTransferID == waterTransferID);
            return waterTransfer?.AsDetailedDto();
        }

        public static List<ErrorMessage> ValidateConfirmTransfer(
            WaterTransferRegistrationUpsertDto waterTransferRegistrationUpsertDto, WaterTransferDetailedDto waterTransferDto,
            UserDto currentUser)
        {
            var result = new List<ErrorMessage>();

            if (currentUser.Role.RoleID != (int)RoleEnum.Admin)
            {
                if (waterTransferRegistrationUpsertDto.WaterTransferTypeID == (int)WaterTransferTypeEnum.Selling &&
                    !waterTransferDto.SellerRegistration.Account.Users.Select(x => (int?)x.UserID)
                        .Contains(waterTransferRegistrationUpsertDto.UserID))
                {
                    result.Add(new ErrorMessage() { Message = "Confirming user does not match seller." });
                }

                if (waterTransferRegistrationUpsertDto.WaterTransferTypeID == (int)WaterTransferTypeEnum.Buying &&
                    !waterTransferDto.BuyerRegistration.Account.Users.Select(x => (int?)x.UserID)
                        .Contains(waterTransferRegistrationUpsertDto.UserID))
                {
                    result.Add(new ErrorMessage() { Message = "Confirming user does not match buyer." });
                }
            }

            return result;
        }

        public static WaterTransferDetailedDto ChangeWaterRegistrationStatus(RioDbContext dbContext, int waterTransferID,
            WaterTransferRegistrationUpsertDto waterTransferRegistrationDto,
            WaterTransferRegistrationStatusEnum waterTransferRegistrationStatusEnum)
        {
            var waterTransferRegistration = dbContext.WaterTransferRegistrations
                .Single(x =>
                    x.WaterTransferID == waterTransferID &&
                    x.WaterTransferTypeID == waterTransferRegistrationDto.WaterTransferTypeID);
            waterTransferRegistration.WaterTransferRegistrationStatusID = (int)waterTransferRegistrationStatusEnum;
            waterTransferRegistration.StatusDate = DateTime.Now;
            dbContext.SaveChanges();
            dbContext.Entry(waterTransferRegistration).Reload();
            return GetByWaterTransferIDAsWaterTransferDetailedDto(dbContext, waterTransferID);
        }

        public static List<ErrorMessage> ValidateCancelTransfer(
            WaterTransferRegistrationUpsertDto waterTransferRegistrationUpsertDto, WaterTransferDetailedDto waterTransferDetailedDto,
            UserDto currentUser)
        {
            var result = new List<ErrorMessage>();

            if (waterTransferDetailedDto.BuyerRegistration.IsRegistered || waterTransferDetailedDto.SellerRegistration.IsRegistered)
            {
                result.Add(new ErrorMessage() { Message = "Cannot cancel transfer because one of the parties has already registered this transfer." });
            }

            if (currentUser.Role.RoleID != (int)RoleEnum.Admin)
            {
                if (waterTransferRegistrationUpsertDto.WaterTransferTypeID == (int)WaterTransferTypeEnum.Selling && !
                        waterTransferDetailedDto.SellerRegistration.Account.Users.Select(x => (int?)x.UserID)
                            .Contains(waterTransferRegistrationUpsertDto.UserID))
                {
                    result.Add(new ErrorMessage() { Message = "Canceling user does not match seller." });
                }

                if (waterTransferRegistrationUpsertDto.WaterTransferTypeID == (int)WaterTransferTypeEnum.Buying && !
                        waterTransferDetailedDto.BuyerRegistration.Account.Users.Select(x => (int?)x.UserID)
                            .Contains(waterTransferRegistrationUpsertDto.UserID))
                {
                    result.Add(new ErrorMessage() { Message = "Canceling user does not match buyer." });
                }
            }

            return result;
        }

        public static WaterTransferDto GetMostRecentRegistered(RioDbContext dbContext)
        {
            var waterTransfer = GetWaterTransfersImpl(dbContext).Where(x => x.WaterTransferRegistrations.All(y => y.WaterTransferRegistrationStatusID == (int)WaterTransferRegistrationStatusEnum.Registered)).OrderByDescending(x => x.WaterTransferRegistrations.Max(y => y.StatusDate)).FirstOrDefault();
            return waterTransfer?.AsDto();
        }

        public static void DeleteAll(RioDbContext dbContext)
        {
            dbContext.WaterTransfers.RemoveRange(dbContext.WaterTransfers);
            dbContext.SaveChanges();
        }
    }
}