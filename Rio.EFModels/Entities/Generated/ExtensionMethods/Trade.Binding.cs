//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[Trade]
namespace Rio.EFModels.Entities
{
    public partial class Trade
    {
        public TradeStatus TradeStatus => TradeStatus.AllLookupDictionary[TradeStatusID];
    }
}