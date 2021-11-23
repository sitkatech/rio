﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Rio.Models.DataTransferObjects;

namespace Rio.EFModels.Entities
{
    public partial class OfferStatus
    {
        public static IEnumerable<OfferStatusDto> List(RioDbContext dbContext)
        {
            var offerStatusDtos = dbContext.OfferStatuses
                .AsNoTracking()
                .Select(x => x.AsDto());

            return offerStatusDtos;
        }

        public static OfferStatusDto GetByOfferStatusID(RioDbContext dbContext, int offerStatusID)
        {
            var offerStatus = dbContext.OfferStatuses
                .AsNoTracking()
                .SingleOrDefault(x => x.OfferStatusID == offerStatusID);

            return offerStatus?.AsDto();
        }
    }

    public enum OfferStatusEnum
    {
        Pending = 1,
        Rejected = 2,
        Rescinded = 3,
        Accepted = 4
    }
}