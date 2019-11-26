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
            var parcels = dbContext.vParcelOwnership.Include(x => x.Parcel).Include(x => x.User).AsNoTracking().Where(
                    x =>
                        x.UserID == userID &&
                        (x.EffectiveYear == null ||
                         x.EffectiveYear <=
                         year) &&
                        (x.SaleDate == null || x.SaleDate <= new DateTime(year, 12, 31))).ToList()
                .GroupBy(x => x.ParcelID).Select(x => x.OrderBy(y => y.RowNumber).First()).ToList();

            
            var parcelIDsToCheck = parcels.Select(x=>x.ParcelID).ToList();

            // if the same list of parcels from earlier have entries matching the same date criteria with a lower row number when userid is excluded from the where, those entries are parcels where this user is not the owner as of year
            var parcelRowNumbersToCheck = dbContext.vParcelOwnership.Include(x => x.Parcel).Include(x => x.User).AsNoTracking()
                .Where(x => parcelIDsToCheck.Contains(x.ParcelID) &&
                            (x.EffectiveYear == null ||
                             x.EffectiveYear <=
                             year) &&
                            (x.SaleDate == null || x.SaleDate <= new DateTime(year, 12, 31))).ToList()
                .GroupBy(x=>x.ParcelID).Select(x => x.OrderBy(y => y.RowNumber).First())
                .Select(x=> new {x.ParcelID, x.RowNumber}).ToList();


            return parcels.Where(x => parcelRowNumbersToCheck.Contains(new { x.ParcelID, x.RowNumber })).Select(x => x.AsParcelDto()).AsEnumerable();
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