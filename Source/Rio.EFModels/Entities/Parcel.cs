using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Rio.Models.DataTransferObjects;
using Rio.Models.DataTransferObjects.Parcel;

namespace Rio.EFModels.Entities
{
    public partial class Parcel
    {
        public static IEnumerable<ParcelDto> ListParcelsWithLandOwners(RioDbContext dbContext)
        {
            var parcels = dbContext.UserParcel.Include(x => x.Parcel)
                .AsNoTracking()
                .OrderBy(x => x.Parcel.ParcelNumber)
                .Select(x => x.Parcel.AsDto())
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
                .Include(x => x.UserParcel).ThenInclude(x => x.User)
                .AsNoTracking()
                .SingleOrDefault(x => x.ParcelID == parcelID);

            return parcel?.AsDto();
        }

        public static BoundingBoxDto GetBoundingBoxByParcelIDs(RioDbContext dbContext, List<int> parcelIDs)
        {
            var parcels = dbContext.Parcel
                .AsNoTracking()
                .Where(x => parcelIDs.Contains(x.ParcelID));

            var geometries = parcels.Select(x => x.ParcelGeometry).ToList();
            return new BoundingBoxDto(geometries);
        }
    }
}