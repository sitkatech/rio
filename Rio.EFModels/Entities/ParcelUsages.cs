using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.IO;
using Qanat.EFModels.Entities;
using Rio.Models.DataTransferObjects;

namespace Rio.EFModels.Entities;

public static class ParcelUsages
{
    private const double MillimetersToFeetConversionFactor = 304.8;

    public static IEnumerable<ParcelUsageStaging> GetByParcelUsageFileUploadID(RioDbContext dbContext, int parcelUsageFileUploadID)
    {
        return dbContext.ParcelUsageStagings.AsNoTracking()
            .Include(x => x.Parcel)
            .ThenInclude(x => x.ParcelLedgers)
            .Where(x => x.ParcelUsageFileUploadID == parcelUsageFileUploadID);
    }

    public static ParcelUsageFileUpload GetParcelUsageFileUploadByID(RioDbContext dbContext, int parcelUsageFileUploadID)
    {
        return dbContext.ParcelUsageFileUploads.SingleOrDefault(x => x.ParcelUsageFileUploadID == parcelUsageFileUploadID);
    }

    public static int CreateStagingRecords(RioDbContext dbContext, List<ParcelTransactionCSV> records, DateTime effectiveDate, string uploadedFileName, int nullParcelNumberCount, int userID)
    {
        var parcelUsageFileUpload = new ParcelUsageFileUpload()
        {
            UserID = userID,
            UploadedFileName = uploadedFileName,
            UploadDate = DateTime.UtcNow,
            MatchedRecordCount = 0,
            UnmatchedParcelNumberCount = 0,
            NullParcelNumberCount = nullParcelNumberCount
        };

        dbContext.ParcelUsageFileUploads.Add(parcelUsageFileUpload);
        dbContext.SaveChanges();

        var parcelAreaDictionary = dbContext.Parcels.AsNoTracking()
            .ToDictionary(x => x.ParcelNumber, y => new { y.ParcelID, y.ParcelAreaInAcres });
        var parcelUsages = new List<ParcelUsageStaging>();
        var unmatchedParcelNumbersCount = 0;

        var recordGroups = records.Where(x => !string.IsNullOrEmpty(x.APN))
            .GroupBy(x => x.APN).ToList();
        foreach (var recordGroup in recordGroups)
        {
            var reportedValue = -1 * recordGroup.Sum(x => x.Quantity.Value); // flipping sign to indicate usage in ledger
            var parcelUsage = new ParcelUsageStaging()
            {
                ParcelNumber = recordGroup.Key,
                ReportedDate = effectiveDate, // todo: convert effective date to utc date
                ReportedValue = (decimal)reportedValue, 
                ReportedValueInAcreFeet = 0,
                ParcelUsageFileUploadID = parcelUsageFileUpload.ParcelUsageFileUploadID,
                UserID = userID
            };

            if (parcelAreaDictionary.ContainsKey(recordGroup.Key))
            {
                parcelUsage.ParcelID = parcelAreaDictionary[recordGroup.Key].ParcelID;
                parcelUsage.ReportedValueInAcreFeet =
                    (decimal)ConvertMillimetersToAcreFeet(reportedValue, parcelAreaDictionary[recordGroup.Key].ParcelAreaInAcres);
            }
            else
            {
                unmatchedParcelNumbersCount++;
            }

            parcelUsages.Add(parcelUsage);
        }

        dbContext.ParcelUsageStagings.AddRange(parcelUsages);

        parcelUsageFileUpload.MatchedRecordCount = parcelUsages.Count;
        parcelUsageFileUpload.UnmatchedParcelNumberCount = unmatchedParcelNumbersCount;

        dbContext.SaveChanges();

        return parcelUsageFileUpload.ParcelUsageFileUploadID;
    }

    private static double ConvertMillimetersToAcreFeet(double reportedValue, double parcelAreaInAcres)
    {
        return (reportedValue / MillimetersToFeetConversionFactor) * parcelAreaInAcres;
    }

    public static int PublishStagingByParcelUsageFileUploadID(RioDbContext dbContext, ParcelUsageFileUpload parcelUsageFileUpload)
    {
        var transactionDate = DateTime.UtcNow;
        var parcelUsageStagings = GetByParcelUsageFileUploadID(dbContext, parcelUsageFileUpload.ParcelUsageFileUploadID).ToList();

        var parcelLedgers = new List<ParcelLedger>();
        foreach (var parcelUsageStaging in parcelUsageStagings)
        {
            if (!parcelUsageStaging.ParcelID.HasValue)
            {
                continue;
            }

            parcelLedgers.Add(new ParcelLedger()
            {
                ParcelID = parcelUsageStaging.ParcelID.Value,
                TransactionDate = transactionDate,
                EffectiveDate = parcelUsageStaging.ReportedDate,
                TransactionTypeID = (int)TransactionTypeEnum.Usage,
                ParcelLedgerEntrySourceTypeID = (int)ParcelLedgerEntrySourceTypeEnum.Manual,
                TransactionAmount = parcelUsageStaging.ReportedValueInAcreFeet,
                TransactionDescription = $"Transaction recorded via spreadsheet upload: {parcelUsageFileUpload.UploadedFileName}",
                UserID = parcelUsageStaging.UserID,
                UploadedFileName = parcelUsageFileUpload.UploadedFileName
            });
        }

        dbContext.ParcelLedgers.AddRange(parcelLedgers);
        parcelUsageFileUpload.PublishDate = DateTime.UtcNow;

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