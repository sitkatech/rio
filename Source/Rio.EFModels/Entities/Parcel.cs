﻿using Microsoft.EntityFrameworkCore;
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
            var parcels = vParcelOwnershipsByYear(dbContext, year).Select(x => x.AsParcelDto())
                .AsEnumerable();

            return parcels;
        }

        public static IQueryable<vParcelOwnership> vParcelOwnershipsByYear(RioDbContext dbContext, int year)
        {
            var accountParcelIDs = dbContext.vParcelOwnership.AsNoTracking()
                .Where(x => x.EffectiveYear == null || x.EffectiveYear <= year).ToList()
                .GroupBy(x => x.ParcelID)
                .Select(x => x.OrderBy(y => y.RowNumber).First().AccountParcelID);

            return dbContext.vParcelOwnership.Include(x => x.Parcel).Include(x => x.Account).AsNoTracking()
                .Where(x => accountParcelIDs.Contains(x.AccountParcelID));
        }

        public static IEnumerable<ParcelDto> ListByAccountID(RioDbContext dbContext, int accountID, int year)
        {
            // get all the parcelIDs Account(accountID) has ever owned
            var parcelIDsEverOwned = dbContext.vParcelOwnership.AsNoTracking().Where(x => x.AccountID == accountID).Select(x => x.ParcelID).Distinct().ToList();

            // get all of their parcel ownership records as of (year)
            var parcelDtos = dbContext.vParcelOwnership.Include(x => x.Parcel).Include(x => x.Account)
                .AsNoTracking()
                .Where(x => parcelIDsEverOwned.Contains(x.ParcelID) &&
                            (x.EffectiveYear == null ||
                             x.EffectiveYear <=
                             year)).ToList()
                // get the lowest row numbered of those
                .GroupBy(x => x.ParcelID).Select(x => x.OrderBy(y => y.RowNumber).First())
                // throw out anything where Record.UserID != userID
                .Where(x => x.AccountID == accountID)
                .Select(x => x.AsParcelDto()).AsEnumerable();

            return parcelDtos;

        }

        public static IEnumerable<ParcelDto> ListByAccountIDs(RioDbContext dbContext, List<int?> accountIDs, int year)
        {
            // get all the parcelIDs Account(accountID) has ever owned
            var parcelIDsEverOwned = dbContext.vParcelOwnership.AsNoTracking().Where(x => accountIDs.Contains(x.AccountID)).Select(x => x.ParcelID).Distinct().ToList();

            // get all of their parcel ownership records as of (year)
            var parcelDtos = dbContext.vParcelOwnership.Include(x => x.Parcel).Include(x => x.Account)
                .AsNoTracking()
                .Where(x => parcelIDsEverOwned.Contains(x.ParcelID) &&
                            (x.EffectiveYear == null ||
                             x.EffectiveYear <=
                             year)).ToList()
                // get the lowest row numbered of those
                .GroupBy(x => x.ParcelID).Select(x => x.OrderBy(y => y.RowNumber).First())
                // throw out anything where Record.UserID != userID
                .Where(x => accountIDs.Contains(x.AccountID))
                .Select(x => x.AsParcelDto()).AsEnumerable();

            return parcelDtos;

        }

        public static IEnumerable<ParcelDto> ListByUserID(RioDbContext dbContext, int userID, int year)
        {
            var user = dbContext.User.Include(x => x.AccountUser).Single(x => x.UserID == userID);
            var accountIDs = user.AccountUser.Select(x => (int?) x.AccountID).ToList();
            
            return ListByAccountIDs(dbContext, accountIDs, year);
        }

        public static ParcelDto GetByParcelID(RioDbContext dbContext, int parcelID)
        {
            var parcel = dbContext.Parcel
                .Include(x => x.AccountParcel).ThenInclude(x => x.Account)
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
            return dbContext.vParcelOwnership.Include(x=>x.Account).AsNoTracking().Where(x=>x.ParcelID == parcelID).Select( x=>x.AsParcelOwnershipDto());
        }

        public static IEnumerable<ErrorMessage> ValidateChangeOwner(RioDbContext dbContext,
            ParcelChangeOwnerDto parcelChangeOwnerDto, ParcelDto parcelDto)
        {
            var mostRecentSaleDate = dbContext.vParcelOwnership.AsNoTracking().Where(x=>x.ParcelID == parcelDto.ParcelID).Max(x=>x.SaleDate);

            if (mostRecentSaleDate.HasValue && parcelChangeOwnerDto.SaleDate < mostRecentSaleDate.Value)
            {
                yield return new ErrorMessage
                {
                    Message =
                        $"Please enter a sale date after the previous sale date for this parcel ({mostRecentSaleDate.Value.ToShortDateString()})",
                    Type = "Error"
                };
            }
        }

        public static bool IsValidParcelNumber(string regexPattern,  string parcelNumber)
        {
            return Regex.IsMatch(parcelNumber, regexPattern);
        }
    }
}