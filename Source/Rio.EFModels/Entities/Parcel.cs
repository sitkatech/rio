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
            var parcels = dbContext.AccountParcelWaterYear
                .Include(x => x.Parcel)
                .Include(x => x.Account)
                .Include(x => x.WaterYear)
                .Where(x => x.WaterYear.Year == year)
                .Select(x => x.Parcel.AsDto()).AsEnumerable();

            return parcels;
        }

        public static IQueryable<AccountParcelWaterYear> AccountParcelWaterYearOwnershipsByYear(RioDbContext dbContext, int year)
        {
            return dbContext.AccountParcelWaterYear
                .Include(x => x.Parcel)
                .Include(x => x.Account)
                .Include(x => x.WaterYear)
                .AsNoTracking().Where(x => x.WaterYear.Year == year);
        }

        public static IEnumerable<ParcelDto> ListByAccountIDAndYear(RioDbContext dbContext, int accountID, int year)
        {
            var parcelDtos = dbContext.AccountParcelWaterYear
                .Include(x => x.Parcel)
                .Include(x => x.Account)
                .Include(x => x.WaterYear)
                .Where(x => x.WaterYear.Year == year && x.Account.AccountID == accountID)
                .Select(x => x.Parcel.AsDto()).AsEnumerable();

            return parcelDtos;

        }

        public static IEnumerable<ParcelDto> ListByAccountIDsAndYear(RioDbContext dbContext, List<int> accountIDs, int year)
        {
            var parcelDtos = dbContext.AccountParcelWaterYear
                .Include(x => x.Parcel)
                .Include(x => x.Account)
                .Include(x => x.WaterYear)
                .Where(x => x.WaterYear.Year == year && accountIDs.Contains(x.AccountID))
                .Select(x => x.Parcel.AsDto()).AsEnumerable();

            return parcelDtos;

        }

        public static IEnumerable<ParcelDto> ListByUserID(RioDbContext dbContext, int userID, int year)
        {
            var user = dbContext.User.Include(x => x.AccountUser).Single(x => x.UserID == userID);
            var accountIDs = user.AccountUser.Select(x => x.AccountID).ToList();
            
            return ListByAccountIDsAndYear(dbContext, accountIDs, year);
        }

        public static ParcelDto GetByParcelID(RioDbContext dbContext, int parcelID)
        {
            var parcel = dbContext.Parcel
                .Include(x => x.AccountParcelWaterYear)
                .ThenInclude(x => x.Account)
                .Include(x => x.AccountParcelWaterYear)
                .ThenInclude(x => x.WaterYear)
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
            return dbContext.vParcelOwnership
                .Include(x => x.Account)
                .Include(x => x.WaterYear)
                .AsNoTracking()
                .Where(x => x.ParcelID == parcelID)
                .Select(x => x.AsParcelOwnershipDto());
        }

        //public static IEnumerable<ErrorMessage> ValidateChangeOwner(RioDbContext dbContext,
        //    ParcelChangeOwnerDto parcelChangeOwnerDto, ParcelDto parcelDto)
        //{
        //    var mostRecentSaleDate = dbContext.vParcelOwnership.AsNoTracking().Where(x=>x.ParcelID == parcelDto.ParcelID).Max(x=>x.SaleDate);

        //    if (mostRecentSaleDate.HasValue && parcelChangeOwnerDto.SaleDate < mostRecentSaleDate.Value)
        //    {
        //        yield return new ErrorMessage
        //        {
        //            Message =
        //                $"Please enter a sale date after the previous sale date for this parcel ({mostRecentSaleDate.Value.ToShortDateString()})",
        //            Type = "Error"
        //        };
        //    }
        //}

        public static bool IsValidParcelNumber(string regexPattern,  string parcelNumber)
        {
            return Regex.IsMatch(parcelNumber, regexPattern);
        }
        public static IEnumerable<ParcelDto> GetInactiveParcels(RioDbContext dbContext)
        {
            var currentWaterYear = WaterYear.GetDefaultYearToDisplay(dbContext);

            return dbContext.vParcelOwnership
                .Include(x => x.Parcel)
                .Where(x => !x.AccountID.HasValue && x.WaterYearID == currentWaterYear.WaterYearID)
                .Select(x => x.Parcel.AsDto()).AsEnumerable();
        }
    }
}