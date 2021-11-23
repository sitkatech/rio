//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[TradeStatus]

using Rio.Models.DataTransferObjects;

namespace Rio.EFModels.Entities
{
    public static partial class TradeStatusExtensionMethods
    {
        public static TradeStatusDto AsDto(this TradeStatus tradeStatus)
        {
            var tradeStatusDto = new TradeStatusDto()
            {
                TradeStatusID = tradeStatus.TradeStatusID,
                TradeStatusName = tradeStatus.TradeStatusName,
                TradeStatusDisplayName = tradeStatus.TradeStatusDisplayName
            };
            DoCustomMappings(tradeStatus, tradeStatusDto);
            return tradeStatusDto;
        }

        static partial void DoCustomMappings(TradeStatus tradeStatus, TradeStatusDto tradeStatusDto);

        public static TradeStatusSimpleDto AsSimpleDto(this TradeStatus tradeStatus)
        {
            var tradeStatusSimpleDto = new TradeStatusSimpleDto()
            {
                TradeStatusID = tradeStatus.TradeStatusID,
                TradeStatusName = tradeStatus.TradeStatusName,
                TradeStatusDisplayName = tradeStatus.TradeStatusDisplayName
            };
            DoCustomSimpleDtoMappings(tradeStatus, tradeStatusSimpleDto);
            return tradeStatusSimpleDto;
        }

        static partial void DoCustomSimpleDtoMappings(TradeStatus tradeStatus, TradeStatusSimpleDto tradeStatusSimpleDto);
    }
}