using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
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
                TransferDate = offerDto.OfferDate,
                ConfirmedByReceivingUser = false,
                ConfirmedByTransferringUser = false,
                TradeID = tradeDto.TradeID
            };

            if (postingDto.PostingType.PostingTypeID == (int) PostingTypeEnum.OfferToSell)
            {
                waterTransfer.TransferringUserID = postingDto.CreateUser.UserID;
                waterTransfer.ReceivingUserID = tradeDto.CreateUser.UserID;
            }
            else
            {
                waterTransfer.TransferringUserID = tradeDto.CreateUser.UserID;
                waterTransfer.ReceivingUserID = postingDto.CreateUser.UserID;
            }

            dbContext.WaterTransfer.Add(waterTransfer);
            dbContext.SaveChanges();
            dbContext.Entry(waterTransfer).Reload();

            return GetByWaterTransferID(dbContext, waterTransfer.WaterTransferID);
        }

        public static IEnumerable<WaterTransferDto> ListByUserID(RioDbContext dbContext, int userID)
        {
            var waterTransfers = dbContext.WaterTransfer
                .Include(x => x.TransferringUser)
                .Include(x => x.ReceivingUser)
                .AsNoTracking()
                .Where(x => x.ReceivingUserID == userID || x.TransferringUserID == userID)
                .OrderByDescending(x => x.TransferDate)
                .Select(x => x.AsDto())
                .AsEnumerable();

            return waterTransfers;
        }
        public static WaterTransferDto GetByWaterTransferID(RioDbContext dbContext, int waterTransferID)
        {
            var waterTransferDto = dbContext.WaterTransfer
                .Include(x => x.TransferringUser)
                .Include(x => x.ReceivingUser)
                .AsNoTracking()
                .SingleOrDefault(x => x.WaterTransferID == waterTransferID)
                .AsDto();

            return waterTransferDto;
        }
    }
}