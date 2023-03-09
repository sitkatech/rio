using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Qanat.EFModels.Entities;
using Rio.API.Models;
using Rio.Models.DataTransferObjects;

namespace Rio.EFModels.Entities;

public static class ParcelUsages
{
    private const double MillimetersToFeetConversionFactor = 304.8;

    public static IEnumerable<ParcelUsageStaging> ListByUserID(RioDbContext dbContext, int userID)
    {
        return dbContext.ParcelUsageStagings.AsNoTracking()
            .Include(x => x.Parcel)
            .ThenInclude(x => x.ParcelLedgers)
            .Where(x => x.UserID == userID);
    }

    public static ParcelUsageCsvResponseDto CreateStagingRecordsFromCsv(RioDbContext dbContext, List<ParcelTransactionCSV> records, DateTime effectiveDate, string uploadedFileName, int userID)
    {
        var transactionDate = DateTime.UtcNow;
        effectiveDate = effectiveDate.AddHours(8); // todo: convert effective date to utc date
        var parcelUsages = new List<ParcelUsageStaging>();
        var unmatchedParcelNumbers = new List<string>();

        var parcelAreaDictionary = dbContext.Parcels.AsNoTracking()
            .ToDictionary(x => x.ParcelNumber, y => new { ParcelID = y.ParcelID, ParcelAreaInAcres = y.ParcelAreaInAcres }
            );

        var recordGroups = records.GroupBy(x => x.APN);
        foreach (var recordGroup in recordGroups)
        {
            if (!parcelAreaDictionary.ContainsKey(recordGroup.Key))
            {
                unmatchedParcelNumbers.Add(recordGroup.Key);
                continue;
            }

            var reportedValue = recordGroup.Sum(x => x.Quantity.Value);
            var parcelUsage = new ParcelUsageStaging()
            {
                ParcelID = parcelAreaDictionary[recordGroup.Key].ParcelID,
                ParcelNumber = recordGroup.Key,
                LastUpdateDate = transactionDate,
                ReportedDate = effectiveDate,
                ReportedValue = (decimal)reportedValue,
                ReportedValueInAcreFeet = 
                    (decimal)ConvertMillimetersToAcreFeet(reportedValue, parcelAreaDictionary[recordGroup.Key].ParcelAreaInAcres),
                UploadedFileName = uploadedFileName,
                UserID = userID
            };

            parcelUsages.Add(parcelUsage);
        }

        dbContext.ParcelUsageStagings.AddRange(parcelUsages);
        dbContext.SaveChanges();

        return new ParcelUsageCsvResponseDto(parcelUsages.Count, unmatchedParcelNumbers);
    }

    private static double ConvertMillimetersToAcreFeet(double reportedValue, double parcelAreaInAcres)
    {
        return (reportedValue / MillimetersToFeetConversionFactor) * parcelAreaInAcres;
    }

    public static int PublishStagingToParcelLedgerByUserID(RioDbContext dbContext, int userID)
    {
        var transactionDate = DateTime.UtcNow;
        var stagedParcelUsages = ListByUserID(dbContext, userID);

        var parcelLedgers = stagedParcelUsages.Select(stagedParcelUsage => new ParcelLedger()
        {
            ParcelID = stagedParcelUsage.ParcelID,
            TransactionDate = transactionDate,
            EffectiveDate = stagedParcelUsage.ReportedDate,
            TransactionTypeID = (int)TransactionTypeEnum.Usage,
            ParcelLedgerEntrySourceTypeID = (int)ParcelLedgerEntrySourceTypeEnum.Manual,
            TransactionAmount = stagedParcelUsage.ReportedValueInAcreFeet,
            TransactionDescription = $"Transaction recorded via spreadsheet upload: {stagedParcelUsage.UploadedFileName}",
            UserID = userID,
            UploadedFileName = stagedParcelUsage.UploadedFileName
        }).ToList();

        dbContext.ParcelLedgers.AddRange(parcelLedgers);
        dbContext.SaveChanges();

        DeleteFromStagingByUserID(dbContext, userID);

        return parcelLedgers.Count;
    }

    public static void DeleteFromStagingByUserID(RioDbContext dbContext, int userID)
    {
        var stagedParcelUsages = ListByUserID(dbContext, userID);
        
        dbContext.ParcelUsageStagings.RemoveRange(stagedParcelUsages);
        dbContext.SaveChanges();
    }
}