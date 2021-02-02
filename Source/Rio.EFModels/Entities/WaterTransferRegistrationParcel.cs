using Microsoft.EntityFrameworkCore;
using Rio.Models.DataTransferObjects;
using Rio.Models.DataTransferObjects.WaterTransfer;
using System.Collections.Generic;
using System.Linq;

namespace Rio.EFModels.Entities
{
    public partial class WaterTransferRegistrationParcel
    {
        public static IEnumerable<WaterTransferRegistrationParcelDto> SaveParcels(RioDbContext dbContext, int waterTransferID, WaterTransferRegistrationDto waterTransferRegistrationDto)
        {
            // get the registration record
            var waterTransferRegistration = dbContext.WaterTransferRegistration.Single(x =>
                x.WaterTransferID == waterTransferID && x.WaterTransferTypeID == waterTransferRegistrationDto.WaterTransferTypeID);

            // delete existing parcels registered
            var existingWaterTransferRegistrationParcels = dbContext.WaterTransferRegistrationParcel.Where(x => x.WaterTransferRegistrationID == waterTransferRegistration.WaterTransferRegistrationID);
            if (existingWaterTransferRegistrationParcels.Any())
            {
                dbContext.WaterTransferRegistrationParcel.RemoveRange(existingWaterTransferRegistrationParcels);
                dbContext.SaveChanges();
            }

            foreach (var waterTransferParcelDto in waterTransferRegistrationDto.WaterTransferRegistrationParcels)
            {
                var waterTransferRegistrationParcel = new WaterTransferRegistrationParcel
                {
                    WaterTransferRegistrationID = waterTransferRegistration.WaterTransferRegistrationID,
                    ParcelID = waterTransferParcelDto.ParcelID,
                    AcreFeetTransferred = waterTransferParcelDto.AcreFeetTransferred
                };
                dbContext.WaterTransferRegistrationParcel.Add(waterTransferRegistrationParcel);
            }

            dbContext.SaveChanges();
            return ListByWaterTransferRegistrationID(dbContext, waterTransferRegistration.WaterTransferRegistrationID);
        }

        public static IEnumerable<WaterTransferRegistrationParcelDto> ListByWaterTransferRegistrationID(
            RioDbContext dbContext, int waterTransferRegistrationID)
        {
            var waterTransferParcelDtos = GetWaterTransferRegistrationParcelsImpl(dbContext)
                .Where(x => x.WaterTransferRegistrationID == waterTransferRegistrationID)
                .OrderBy(x => x.Parcel.ParcelNumber)
                .Select(x => x.AsDto())
                .AsEnumerable();

            return waterTransferParcelDtos;
        }

        public static IEnumerable<WaterTransferRegistrationParcelDto> ListByWaterTransferIDAndAccountID(
            RioDbContext dbContext, int waterTransferID, int accountID)
        {
            var waterTransferParcelDtos = GetWaterTransferRegistrationParcelsImpl(dbContext)
                .Where(x => x.WaterTransferRegistration.WaterTransferID == waterTransferID && x.WaterTransferRegistration.AccountID == accountID)
                .OrderBy(x => x.Parcel.ParcelNumber)
                .Select(x => x.AsDto())
                .AsEnumerable();

            return waterTransferParcelDtos;
        }

        private static IQueryable<WaterTransferRegistrationParcel> GetWaterTransferRegistrationParcelsImpl(RioDbContext dbContext)
        {
            return dbContext.WaterTransferRegistrationParcel
                .Include(x => x.WaterTransferRegistration)
                .Include(x => x.Parcel)
                .AsNoTracking();
        }

        public static List<ErrorMessage> ValidateParcels(List<WaterTransferRegistrationParcelDto> waterTransferParcelDtos, WaterTransferDto waterTransferDto)
        {
            var result = new List<ErrorMessage>();

            //if(waterTransferParcelDto.WaterTransferTypeID == (int) WaterTransferTypeEnum.Selling && waterTransferParcelDto.ConfirmingUserID != waterTransferDto.TransferringUser.UserID)
            //{
            //    result.Add(new ErrorMessage() { Message = "Confirming user does not match transferring user." });
            //}

            //if (waterTransferParcelDto.WaterTransferTypeID == (int) WaterTransferTypeEnum.Buying && waterTransferParcelDto.ConfirmingUserID != waterTransferDto.ReceivingUser.UserID)
            //{
            //    result.Add(new ErrorMessage() { Message = "Confirming user does not match receiving user." });
            //}

            return result;
        }

        public static void DeleteAll(RioDbContext dbContext)
        {
            dbContext.WaterTransferRegistrationParcel.RemoveRange(dbContext.WaterTransferRegistrationParcel);
            dbContext.SaveChanges();
        }
    }
}