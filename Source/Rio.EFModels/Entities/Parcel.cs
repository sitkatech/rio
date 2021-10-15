using Microsoft.EntityFrameworkCore;
using Rio.Models.DataTransferObjects;
using Rio.Models.DataTransferObjects.Parcel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Rio.EFModels.Entities
{
    public partial class Parcel
    {
        public static IEnumerable<ParcelDto> ListParcelsWithLandOwners(RioDbContext dbContext, int year)
        {
            var parcels = dbContext.AccountParcelWaterYears
                .Include(x => x.Parcel)
                .Include(x => x.Account)
                .Include(x => x.WaterYear)
                .Where(x => x.WaterYear.Year == year)
                .Select(x => x.Parcel.AsDto()).AsEnumerable();

            return parcels;
        }

        public static IQueryable<AccountParcelWaterYear> AccountParcelWaterYearOwnershipsByYear(RioDbContext dbContext, int year)
        {
            return dbContext.AccountParcelWaterYears
                .Include(x => x.Parcel)
                .Include(x => x.Account)
                .Include(x => x.WaterYear)
                .AsNoTracking().Where(x => x.WaterYear.Year == year);
        }

        public static IEnumerable<ParcelDto> ListByAccountIDAndYear(RioDbContext dbContext, int accountID, int year)
        {
            var parcelDtos = dbContext.AccountParcelWaterYears
                .Include(x => x.Parcel)
                .Include(x => x.Account)
                .Include(x => x.WaterYear)
                .Where(x => x.WaterYear.Year == year && x.Account.AccountID == accountID)
                .Select(x => x.Parcel.AsDto()).AsEnumerable();

            return parcelDtos;

        }

        public static IEnumerable<ParcelDto> ListByAccountIDsAndYear(RioDbContext dbContext, List<int> accountIDs, int year)
        {
            var parcelDtos = dbContext.AccountParcelWaterYears
                .Include(x => x.Parcel)
                .Include(x => x.Account)
                .Include(x => x.WaterYear)
                .Where(x => x.WaterYear.Year == year && accountIDs.Contains(x.AccountID))
                .Select(x => x.Parcel.AsDto()).AsEnumerable();

            return parcelDtos;

        }

        public static IEnumerable<ParcelDto> ListByUserID(RioDbContext dbContext, int userID, int year)
        {
            var user = dbContext.Users.Include(x => x.AccountUsers).Single(x => x.UserID == userID);
            var accountIDs = user.AccountUsers.Select(x => x.AccountID).ToList();
            
            return ListByAccountIDsAndYear(dbContext, accountIDs, year);
        }

        public static ParcelDto GetByParcelID(RioDbContext dbContext, int parcelID)
        {
            var parcel = dbContext.Parcels
                .Include(x => x.AccountParcelWaterYears)
                .ThenInclude(x => x.Account)
                .Include(x => x.AccountParcelWaterYears)
                .ThenInclude(x => x.WaterYear)
                .AsNoTracking()
                .SingleOrDefault(x => x.ParcelID == parcelID);

            return parcel?.AsDto();
        }

        public static BoundingBoxDto GetBoundingBoxByParcelIDs(RioDbContext dbContext, List<int> parcelIDs)
        {
            var parcels = dbContext.Parcels
                .AsNoTracking()
                .Where(x => parcelIDs.Contains(x.ParcelID));

            var geometries = parcels.Select(x => x.ParcelGeometry).ToList();
            return new BoundingBoxDto(geometries);
        }

        public static IQueryable<ParcelOwnershipDto> GetOwnershipHistory(RioDbContext dbContext, int parcelID)
        {
            return dbContext.vParcelOwnerships
                .Include(x => x.Account)
                .Include(x => x.WaterYear)
                .AsNoTracking()
                .Where(x => x.ParcelID == parcelID)
                .Select(x => x.AsParcelOwnershipDto());
        }

        public static bool IsValidParcelNumber(string regexPattern,  string parcelNumber)
        {
            return Regex.IsMatch(parcelNumber, regexPattern);
        }
        public static IEnumerable<ParcelDto> GetInactiveParcels(RioDbContext dbContext)
        {
            return dbContext.Parcels
                .Where(x => x.ParcelStatusID == (int) ParcelStatusEnum.Inactive)
                .Select(x => x.AsDto());
        }

        public static void UpdateParcelStatus(RioDbContext dbContext, int parcelID, int parcelStatusID)
        {
            var currentParcel = dbContext.Parcels.Single(x => x.ParcelID == parcelID);

            currentParcel.ParcelStatusID = parcelStatusID;
            currentParcel.InactivateDate = parcelStatusID == (int)ParcelStatusEnum.Active ? (DateTime?)null : DateTime.UtcNow;

            dbContext.SaveChanges();
        }

        public static IEnumerable<ParcelDto> ListByAccountID(RioDbContext dbContext, int accountID)
        {
            var parcelDtos = dbContext.AccountParcelWaterYears
                .Include(x => x.Parcel)
                .Include(x => x.Account)
                .Include(x => x.WaterYear)
                .Where(x => x.Account.AccountID == accountID)
                .Select(x => x.Parcel.AsDto()).AsEnumerable();

            return parcelDtos;

        }
    }
}