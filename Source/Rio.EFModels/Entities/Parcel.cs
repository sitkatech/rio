using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Rio.Models.DataTransferObjects;
using Rio.Models.DataTransferObjects.Parcel;
using Rio.Models.DataTransferObjects.User;

namespace Rio.EFModels.Entities
{
    public partial class Parcel
    {
        public static IEnumerable<ParcelWithWaterUsageDto> ListWithWaterUsage(RioDbContext dbContext)
        {
            // right now we are assuming a parcel can only be associated to one user
            var parcels = dbContext.ParcelWithAnnualWaterUsages.OrderBy(x => x.ParcelNumber).ToList()
                .Select(parcel =>
                {
                    var parcelWithWaterUsageDto = new ParcelWithWaterUsageDto()
                    {
                        ParcelID = parcel.ParcelID,
                        ParcelNumber = parcel.ParcelNumber,
                        ParcelAreaInAcres = parcel.ParcelAreaInAcres,
                        WaterUsageFor2016 = parcel.WaterUsageFor2016,
                        WaterUsageFor2017 = parcel.WaterUsageFor2017,
                        WaterUsageFor2018 = parcel.WaterUsageFor2018
                    };
                    if (parcel.UserID.HasValue)
                    {
                        parcelWithWaterUsageDto.LandOwner = new UserSimpleDto()
                        {
                            FirstName = parcel.FirstName, LastName = parcel.LastName, Email = parcel.Email,
                            UserID = parcel.UserID.Value
                        };
                    }

                    return parcelWithWaterUsageDto;
                }).ToList();
            return parcels;
        }

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

            return new BoundingBoxDto(parcels.Select(x => x.ParcelGeometry));
        }
    }
}