using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Rio.Models.DataTransferObjects.Parcel;

namespace Rio.EFModels.Entities
{
    public partial class Parcel
    {
        public static IEnumerable<ParcelDto> List(RioDbContext dbContext)
        {
            var parcels = dbContext.Parcel
                .AsNoTracking()
                .OrderBy(x => x.ParcelNumber)
                .Select(x => x.AsDto())
                .AsEnumerable();

            return parcels;
        }

        public static IEnumerable<ParcelDto> ListByUserID(RioDbContext dbContext, int userID)
        {
            var parcels = dbContext.UserParcel.Include(x => x.Parcel).Where(x => x.UserID == userID)
                .AsNoTracking()
                .OrderBy(x => x.Parcel.ParcelNumber)
                .Select(x => x.Parcel.AsDto())
                .AsEnumerable();

            return parcels;
        }

        public static ParcelDto GetByParcelID(RioDbContext dbContext, int parcelID)
        {
            var parcel = dbContext.Parcel
                .AsNoTracking()
                .SingleOrDefault(x => x.ParcelID == parcelID);

            var parcelDto = parcel?.AsDto();
            return parcelDto;
        }

        public static ParcelDto GetByParcelNumber(RioDbContext dbContext, string parcelNumber)
        {
            var parcel = dbContext.Parcel
                .AsNoTracking()
                .SingleOrDefault(x => x.ParcelNumber == parcelNumber);

            var parcelDto = parcel?.AsDto();
            return parcelDto;
        }
    }
}