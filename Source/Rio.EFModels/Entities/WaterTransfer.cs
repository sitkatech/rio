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
                TransferDate = offerDto.OfferDate,
                ConfirmedByReceivingUser = false,
                ConfirmedByTransferringUser = false
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
            var waterTransfers = GetWaterTransfersImpl(dbContext)
                .Where(x => x.ReceivingUserID == userID || x.TransferringUserID == userID)
                .OrderByDescending(x => x.TransferDate)
                .Select(x => x.AsDto())
                .AsEnumerable();

            return waterTransfers;
        }

        private static IQueryable<WaterTransfer> GetWaterTransfersImpl(RioDbContext dbContext)
        {
            return dbContext.WaterTransfer
                .Include(x => x.TransferringUser)
                .Include(x => x.ReceivingUser)
                .Include(x => x.Offer).ThenInclude(x => x.Trade)
                .AsNoTracking();
        }

        public static WaterTransferDto GetByWaterTransferID(RioDbContext dbContext, int waterTransferID)
        {
            var waterTransfer = GetWaterTransfersImpl(dbContext).SingleOrDefault(x => x.WaterTransferID == waterTransferID);
            return waterTransfer?.AsDto();
        }

        public static List<ErrorMessage> ValidateConfirmTransfer(WaterTransferConfirmDto waterTransferConfirmDto, WaterTransferDto waterTransferDto)
        {
            var result = new List<ErrorMessage>();

            if(waterTransferConfirmDto.WaterTransferType == (int) WaterTransferTypeEnum.Transferring && waterTransferConfirmDto.ConfirmingUserID != waterTransferDto.TransferringUser.UserID)
            {
                result.Add(new ErrorMessage() { Message = "Confirming user does not match transferring user." });
            }

            if (waterTransferConfirmDto.WaterTransferType == (int) WaterTransferTypeEnum.Receiving && waterTransferConfirmDto.ConfirmingUserID != waterTransferDto.ReceivingUser.UserID)
            {
                result.Add(new ErrorMessage() { Message = "Confirming user does not match receiving user." });
            }

            return result;
        }

        public static WaterTransferDto Confirm(RioDbContext dbContext, int waterTransferID, WaterTransferConfirmDto waterTransferConfirmDto)
        {
            var waterTransfer = dbContext.WaterTransfer
                .Single(x => x.WaterTransferID == waterTransferID);

            if (waterTransferConfirmDto.WaterTransferType == (int) WaterTransferTypeEnum.Receiving)
            {
                waterTransfer.ConfirmedByReceivingUser = true;
                waterTransfer.DateConfirmedByReceivingUser = DateTime.Now;
            }
            if (waterTransferConfirmDto.WaterTransferType == (int) WaterTransferTypeEnum.Transferring)
            {
                waterTransfer.ConfirmedByTransferringUser = true;
                waterTransfer.DateConfirmedByTransferringUser = DateTime.Now;
            }

            dbContext.SaveChanges();
            dbContext.Entry(waterTransfer).Reload();
            return GetByWaterTransferID(dbContext, waterTransferID);
        }
    }
}