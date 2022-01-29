//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[Trade]

using Rio.Models.DataTransferObjects;

namespace Rio.EFModels.Entities
{
    public static partial class TradeExtensionMethods
    {
        public static TradeDto AsDto(this Trade trade)
        {
            var tradeDto = new TradeDto()
            {
                TradeID = trade.TradeID,
                Posting = trade.Posting.AsDto(),
                TradeDate = trade.TradeDate,
                TradeStatus = trade.TradeStatus.AsDto(),
                CreateAccount = trade.CreateAccount.AsDto(),
                TradeNumber = trade.TradeNumber
            };
            DoCustomMappings(trade, tradeDto);
            return tradeDto;
        }

        static partial void DoCustomMappings(Trade trade, TradeDto tradeDto);

        public static TradeSimpleDto AsSimpleDto(this Trade trade)
        {
            var tradeSimpleDto = new TradeSimpleDto()
            {
                TradeID = trade.TradeID,
                PostingID = trade.PostingID,
                TradeDate = trade.TradeDate,
                TradeStatusID = trade.TradeStatusID,
                CreateAccountID = trade.CreateAccountID,
                TradeNumber = trade.TradeNumber
            };
            DoCustomSimpleDtoMappings(trade, tradeSimpleDto);
            return tradeSimpleDto;
        }

        static partial void DoCustomSimpleDtoMappings(Trade trade, TradeSimpleDto tradeSimpleDto);
    }
}