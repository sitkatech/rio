using System.Collections.Generic;
using System.Linq;
using Rio.Models.DataTransferObjects;

namespace Rio.EFModels.Entities
{
    public static class TradeStatuses
    {
        public static IEnumerable<TradeStatusDto> List(RioDbContext dbContext)
        {
            return TradeStatus.All.Select(x => x.AsDto());
        }

        public static TradeStatusDto GetByTradeStatusID(RioDbContext dbContext, int tradeStatusID)
        {
            return TradeStatus.AllLookupDictionary[tradeStatusID]?.AsDto();
        }
    }
}