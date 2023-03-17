using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Qanat.EFModels.Entities;
using Rio.Models.DataTransferObjects;

namespace Rio.EFModels.Entities;

public static class ParcelUsages
{
    private const double MillimetersToFeetConversionFactor = 304.8;

    public static IEnumerable<ParcelUsageStaging> GetByUserID(RioDbContext dbContext, int userID)
    {
        return dbContext.ParcelUsageStagings.AsNoTracking()
            .Include(x => x.Parcel)
            .ThenInclude(x => x.ParcelLedgers)
            .Where(x => x.UserID == userID);
    }

    public static List<string> CreateStagingRecordsFromCsv(RioDbContext dbContext, List<ParcelTransactionCSV> records, DateTime effectiveDate, string uploadedFileName, int userID)
    {
        var transactionDate = DateTime.UtcNow;
        effectiveDate = effectiveDate.AddHours(8); // todo: convert effective date to utc date
        var parcelUsages = new List<ParcelUsageStaging>();
        var unmatchedParcelNumbers = new List<string>();

        var parcelAreaDictionary = dbContext.Parcels.AsNoTracking()
            .ToDictionary(x => x.ParcelNumber, y => new { y.ParcelID, y.ParcelAreaInAcres });

        var recordGroups = records.Where(x => !string.IsNullOrEmpty(x.APN))
            .GroupBy(x => x.APN);
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

        return unmatchedParcelNumbers;
    }

    private static double ConvertMillimetersToAcreFeet(double reportedValue, double parcelAreaInAcres)
    {
        return (reportedValue / MillimetersToFeetConversionFactor) * parcelAreaInAcres;
    }

    public static int PublishStagingToParcelLedgerByUserID(RioDbContext dbContext, int userID)
    {
        var transactionDate = DateTime.UtcNow;
        var parcelUsageStagings = GetByUserID(dbContext, userID).ToList();

        var parcelLedgers = new List<ParcelLedger>();
        foreach (var parcelUsageStaging in parcelUsageStagings)
        {
            parcelLedgers.Add(new ParcelLedger()
            {
                ParcelID = parcelUsageStaging.ParcelID,
                TransactionDate = transactionDate,
                EffectiveDate = parcelUsageStaging.ReportedDate,
                TransactionTypeID = (int)TransactionTypeEnum.Usage,
                ParcelLedgerEntrySourceTypeID = (int)ParcelLedgerEntrySourceTypeEnum.Manual,
                TransactionAmount = parcelUsageStaging.ReportedValueInAcreFeet,
                TransactionDescription = $"Transaction recorded via spreadsheet upload: {parcelUsageStaging.UploadedFileName}",
                UserID = userID,
                UploadedFileName = parcelUsageStaging.UploadedFileName
            });
        }

        dbContext.ParcelLedgers.AddRange(parcelLedgers);
        dbContext.ParcelUsageStagings.RemoveRange(parcelUsageStagings);
        dbContext.SaveChanges();

        if (parcelLedgers.Any())
        {
            AccountOverconsumptionCharges.UpdateByYear(dbContext, parcelLedgers.First().EffectiveDate.Year);
        }

        return parcelLedgers.Count;
    }

    public static void DeleteFromStagingByUserID(RioDbContext dbContext, int userID)
    {
        var parcelUsageStagings = dbContext.ParcelUsageStagings.Where(x => x.UserID == userID);

        dbContext.ParcelUsageStagings.RemoveRange(parcelUsageStagings);
        dbContext.SaveChanges();
    }
}