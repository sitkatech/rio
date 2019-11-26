using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Rio.Models.DataTransferObjects;
using Rio.Models.DataTransferObjects.Parcel;

namespace Rio.EFModels.Entities
{
    public partial class Parcel
    {
        public static IEnumerable<ParcelDto> ListParcelsWithLandOwners(RioDbContext dbContext, int year)
        {
            var parcels = dbContext.vParcelOwnership.Include(x => x.Parcel).Include(x => x.User).AsNoTracking().Where(
                    x =>
                        x.RowNumber == 1 &&
                        (x.EffectiveYear == null ||
                         x.EffectiveYear <=
                         year) &&
                        (x.SaleDate == null || x.SaleDate <= new DateTime(year, 12, 31))).Select(x => x.AsParcelDto())
                .AsEnumerable();

            return parcels;
        }

        public static IEnumerable<ParcelDto> ListByUserID(RioDbContext dbContext, int userID, int year)
        {
            // get all the parcelIDs User(userID) has ever owned
            var parcelIDsEverOwned = dbContext.vParcelOwnership.AsNoTracking().Where(x => x.UserID == userID).Select(x => x.ParcelID).Distinct().ToList();

            // get all of their parcel ownership records as of (year)
            var parcelDtos = dbContext.vParcelOwnership.Include(x => x.Parcel).Include(x => x.User)
                .AsNoTracking()
                .Where(x => parcelIDsEverOwned.Contains(x.ParcelID) &&
                            (x.EffectiveYear == null ||
                             x.EffectiveYear <=
                             year) &&
                            (x.SaleDate == null || x.SaleDate <= new DateTime(year, 12, 31))).ToList()
                // get the lowest row numbered of those
                .GroupBy(x => x.ParcelID).Select(x => x.OrderBy(y => y.RowNumber).First())
                // throw out anything where Record.UserID != userID
                .Where(x => x.UserID == userID)
                .Select(x => x.AsParcelDto()).AsEnumerable();

            return parcelDtos;

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

        public static IQueryable<ParcelOwnershipDto> GetOwnershipHistory(RioDbContext dbContext, int parcelID)
        {
            return dbContext.vParcelOwnership.Include(x=>x.User).AsNoTracking().Where(x=>x.ParcelID == parcelID).Select( x=>x.AsParcelOwnershipDto());
        }
    }
}