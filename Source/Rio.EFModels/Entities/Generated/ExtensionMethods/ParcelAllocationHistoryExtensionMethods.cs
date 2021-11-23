//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ParcelAllocationHistory]

using Rio.Models.DataTransferObjects;

namespace Rio.EFModels.Entities
{
    public static partial class ParcelAllocationHistoryExtensionMethods
    {
        public static ParcelAllocationHistoryDto AsDto(this ParcelAllocationHistory parcelAllocationHistory)
        {
            var parcelAllocationHistoryDto = new ParcelAllocationHistoryDto()
            {
                ParcelAllocationHistoryID = parcelAllocationHistory.ParcelAllocationHistoryID,
                ParcelAllocationHistoryDate = parcelAllocationHistory.ParcelAllocationHistoryDate,
                ParcelAllocationHistoryWaterYear = parcelAllocationHistory.ParcelAllocationHistoryWaterYear,
                WaterType = parcelAllocationHistory.WaterType.AsDto(),
                User = parcelAllocationHistory.User.AsDto(),
                FileResource = parcelAllocationHistory.FileResource?.AsDto(),
                ParcelAllocationHistoryValue = parcelAllocationHistory.ParcelAllocationHistoryValue
            };
            DoCustomMappings(parcelAllocationHistory, parcelAllocationHistoryDto);
            return parcelAllocationHistoryDto;
        }

        static partial void DoCustomMappings(ParcelAllocationHistory parcelAllocationHistory, ParcelAllocationHistoryDto parcelAllocationHistoryDto);

        public static ParcelAllocationHistorySimpleDto AsSimpleDto(this ParcelAllocationHistory parcelAllocationHistory)
        {
            var parcelAllocationHistorySimpleDto = new ParcelAllocationHistorySimpleDto()
            {
                ParcelAllocationHistoryID = parcelAllocationHistory.ParcelAllocationHistoryID,
                ParcelAllocationHistoryDate = parcelAllocationHistory.ParcelAllocationHistoryDate,
                ParcelAllocationHistoryWaterYear = parcelAllocationHistory.ParcelAllocationHistoryWaterYear,
                WaterTypeID = parcelAllocationHistory.WaterTypeID,
                UserID = parcelAllocationHistory.UserID,
                FileResourceID = parcelAllocationHistory.FileResourceID,
                ParcelAllocationHistoryValue = parcelAllocationHistory.ParcelAllocationHistoryValue
            };
            DoCustomSimpleDtoMappings(parcelAllocationHistory, parcelAllocationHistorySimpleDto);
            return parcelAllocationHistorySimpleDto;
        }

        static partial void DoCustomSimpleDtoMappings(ParcelAllocationHistory parcelAllocationHistory, ParcelAllocationHistorySimpleDto parcelAllocationHistorySimpleDto);
    }
}