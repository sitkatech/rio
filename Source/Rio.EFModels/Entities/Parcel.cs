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
        public static IEnumerable<ParcelDto> ListForWaterYearAsDto(RioDbContext dbContext, int year)
        {
            var parcels = AccountParcelWaterYearOwnershipsByYear(dbContext, year)
                .Select(x => x.Parcel.AsDto()).AsEnumerable();

            return parcels;
        }

        public static IQueryable<AccountParcelWaterYear> AccountParcelWaterYearOwnerships(RioDbContext dbContext)
        {
            return AccountParcelWaterYearOwnershipsImpl(dbContext);
        }

        public static IQueryable<AccountParcelWaterYear> AccountParcelWaterYearOwnershipsByYear(RioDbContext dbContext, int year)
        {
            return AccountParcelWaterYearOwnershipsImpl(dbContext).Where(x => x.WaterYear.Year == year);
        }

        private static IQueryable<AccountParcelWaterYear> AccountParcelWaterYearOwnershipsImpl(RioDbContext dbContext)
        {
            return dbContext.AccountParcelWaterYears
                .Include(x => x.Parcel).ThenInclude(x => x.ParcelStatus)
                .Include(x => x.Account).ThenInclude(x => x.AccountStatus)
                .Include(x => x.WaterYear)
                .AsNoTracking();
        }

        public static IQueryable<AccountParcelWaterYear> ListByAccountIDsAndYear(RioDbContext dbContext, List<int> accountIDs, int year)
        {
            return AccountParcelWaterYearOwnershipsImpl(dbContext).Where(x => x.WaterYear.Year == year && accountIDs.Contains(x.AccountID));
        }


        public static List<ParcelDto> ListByAccountIDAndYearAsDto(RioDbContext dbContext, int accountID, int year)
        {
            var parcelDtos = ListByAccountIDsAndYear(dbContext, new List<int> { accountID }, year)
                .Select(x => x.Parcel.AsDto()).ToList();
            return parcelDtos;
        }

        public static List<ParcelSimpleDto> ListByAccountIDsAndYearAsSimpleDto(RioDbContext dbContext, List<int> accountIDs, int year)
        {
            var parcelDtos = ListByAccountIDsAndYear(dbContext, accountIDs, year)
                .Select(x => x.Parcel.AsSimpleDto()).ToList();
            return parcelDtos;
        }
        
        public static List<ParcelSimpleDto> ListByUserID(RioDbContext dbContext, int userID, int year)
        {
            var user = dbContext.Users.Include(x => x.AccountUsers).Single(x => x.UserID == userID);
            var accountIDs = user.AccountUsers.Select(x => x.AccountID).ToList();
            
            return ListByAccountIDsAndYearAsSimpleDto(dbContext, accountIDs, year);
        }

        public static ParcelDto GetByIDAsDto(RioDbContext dbContext, int parcelID)
        {
            var parcel = GetParcelImpl(dbContext)
                .SingleOrDefault(x => x.ParcelID == parcelID);

            return parcel?.AsDto();
        }

        public static List<Parcel> ListByIDs(RioDbContext dbContext, List<int> parcelIDs)
        {
            return GetParcelImpl(dbContext)
                .Where(x => parcelIDs.Contains(x.ParcelID))
                .ToList();
        }

        public static List<Parcel> ListByParcelNumbers(RioDbContext dbContext, List<string> parcelNumbers)
        {
            return GetParcelImpl(dbContext)
                .Where(x => parcelNumbers.Contains(x.ParcelNumber))
                .ToList();
        }

        private static IQueryable<Parcel> GetParcelImpl(RioDbContext dbContext)
        {
            return dbContext.Parcels
                .Include(x => x.ParcelStatus)
                .Include(x => x.AccountParcelWaterYears)
                .ThenInclude(x => x.Account)
                .ThenInclude(x => x.AccountStatus)
                .Include(x => x.AccountParcelWaterYears)
                .ThenInclude(x => x.WaterYear)
                .AsNoTracking();
        }

        public static ParcelDto GetByParcelNumberAsDto(RioDbContext dbContext, string parcelNumber)
        {
            var parcel = GetParcelImpl(dbContext)
                .SingleOrDefault(x => x.ParcelNumber == parcelNumber);

            return parcel?.AsDto();
        }

        public static List<ParcelDto> ListByTagIDAsDto(RioDbContext dbContext, int tagID)
        {
            var parcelIDs = dbContext.ParcelTags.Where(x => x.TagID == tagID).Select(x => x.ParcelID).ToList();
            var parcelDtos = GetParcelImpl(dbContext)
                .Where(x => parcelIDs.Contains(x.ParcelID))
                .Select(x => x.AsDto())
                .ToList();

            return parcelDtos;
        }

        public static List<string> SearchParcelNumber(RioDbContext dbContext, string parcelNumber)
        {
            var parcelNumbers = dbContext.Parcels
                .AsNoTracking()
                .Where(x => x.ParcelNumber.Contains(parcelNumber))
                .Select(x => x.ParcelNumber)
                .ToList();

            return parcelNumbers;
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

        public static IEnumerable<ParcelDto> ListInactiveAsDto(RioDbContext dbContext)
        {
            return GetParcelImpl(dbContext)
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
    }
}