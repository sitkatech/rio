using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rio.Models.DataTransferObjects.Parcel;

namespace Rio.EFModels.Entities
{
    public partial class ParcelStatus
    {
        public static ParcelStatusDto GetByID(RioDbContext dbContext, int parcelStatusId)
        {
            return dbContext.ParcelStatuses.Single(x => x.ParcelStatusID == parcelStatusId).AsDto();
        }
    }
}
