using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Rio.Models.DataTransferObjects.Offer;

namespace Rio.EFModels.Entities
{
    public partial class TradeStatus
    {
        public static IEnumerable<TradeStatusDto> List(RioDbContext dbContext)
        {
            var tradeStatusDtos = dbContext.TradeStatuses
                .AsNoTracking()
                .Select(x => x.AsDto());

            return tradeStatusDtos;
        }

        public static TradeStatusDto GetByTradeStatusID(RioDbContext dbContext, int tradeStatusID)
        {
            var tradeStatus = dbContext.TradeStatuses
                .AsNoTracking()
                .SingleOrDefault(x => x.TradeStatusID == tradeStatusID);

            return tradeStatus?.AsDto();
        }
    }

    public enum TradeStatusEnum
    {
        Countered = 1,
        Accepted = 2,
        Rejected = 3,
        Rescinded = 4
    }
}