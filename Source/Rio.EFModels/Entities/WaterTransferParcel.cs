using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Rio.Models.DataTransferObjects;
using Rio.Models.DataTransferObjects.WaterTransfer;

namespace Rio.EFModels.Entities
{
    public partial class WaterTransferParcel
    {
        public static IEnumerable<WaterTransferParcelDto> SaveParcels(RioDbContext dbContext, int waterTransferID, List<WaterTransferParcelDto> waterTransferParcelDtos)
        {
            foreach (var waterTransferParcelDto in waterTransferParcelDtos)
            {
                var waterTransferParcel = dbContext.WaterTransferParcel
                    .SingleOrDefault(x => x.WaterTransferID == waterTransferID && x.ParcelID == waterTransferParcelDto.ParcelID);

                if (waterTransferParcel == null)
                {
                    waterTransferParcel = new WaterTransferParcel
                    {
                        WaterTransferID = waterTransferID,
                        ParcelID = waterTransferParcelDto.ParcelID,
                        WaterTransferTypeID = waterTransferParcelDto.WaterTransferTypeID,
                    };
                    dbContext.WaterTransferParcel.Add(waterTransferParcel);
                }
                waterTransferParcel.AcreFeetTransferred = waterTransferParcelDto.AcreFeetTransferred;
            }
            dbContext.SaveChanges();

            return ListByWaterTransferID(dbContext, waterTransferID);
        }

        public static IEnumerable<WaterTransferParcelDto> ListByWaterTransferID(RioDbContext dbContext, int waterTransferID)
        {
            var waterTransferParcelDtos = GetWaterTransferParcelsImpl(dbContext)
                .Where(x => x.WaterTransferID == waterTransferID)
                .OrderBy(x => x.Parcel.ParcelNumber)
                .Select(x => x.AsDto())
                .AsEnumerable();

            return waterTransferParcelDtos;
        }

        private static IQueryable<WaterTransferParcel> GetWaterTransferParcelsImpl(RioDbContext dbContext)
        {
            return dbContext.WaterTransferParcel
                .Include(x => x.Parcel)
                .AsNoTracking();
        }

        public static List<ErrorMessage> ValidateParcels(List<WaterTransferParcelDto> waterTransferParcelDtos, WaterTransferDto waterTransferDto)
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