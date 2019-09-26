using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Rio.Models.DataTransferObjects;
using Rio.Models.DataTransferObjects.WaterTransfer;

namespace Rio.EFModels.Entities
{
    public partial class WaterTransferRegistrationParcel
    {
        public static IEnumerable<WaterTransferRegistrationParcelDto> SaveParcels(RioDbContext dbContext, int waterTransferID, WaterTransferRegistrationDto waterTransferRegistrationDto)
        {
            var waterTransferRegistration = dbContext.WaterTransferRegistration.Single(x =>
                x.WaterTransferID == waterTransferID && x.WaterTransferTypeID == waterTransferRegistrationDto.WaterTransferTypeID);


            foreach (var waterTransferParcelDto in waterTransferRegistrationDto.WaterTransferRegistrationParcels)
            {
                var waterTransferRegistrationParcel = dbContext.WaterTransferRegistrationParcel
                    .SingleOrDefault(x => x.WaterTransferRegistrationID == waterTransferID && x.ParcelID == waterTransferParcelDto.ParcelID);

                if (waterTransferRegistrationParcel == null)
                {
                    waterTransferRegistrationParcel = new WaterTransferRegistrationParcel
                    {
                        WaterTransferRegistrationID = waterTransferRegistration.WaterTransferRegistrationID,
                        ParcelID = waterTransferParcelDto.ParcelID
                    };
                    dbContext.WaterTransferRegistrationParcel.Add(waterTransferRegistrationParcel);
                }
                waterTransferRegistrationParcel.AcreFeetTransferred = waterTransferParcelDto.AcreFeetTransferred;
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

        public static IEnumerable<WaterTransferRegistrationParcelDto> ListByWaterTransferIDAndUserID(
            RioDbContext dbContext, int waterTransferID, int userID)
        {
            var waterTransferParcelDtos = GetWaterTransferRegistrationParcelsImpl(dbContext)
                .Where(x => x.WaterTransferRegistration.WaterTransferID == waterTransferID && x.WaterTransferRegistration.UserID == userID)
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
    }
}