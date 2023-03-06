using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Qanat.EFModels.Entities;
using Rio.API.Models;

namespace Rio.EFModels.Entities;

public static class ParcelUsages
{
    private const double MillimetersToFeetConversionFactor = 304.8;

    public static ParcelUsageCsvResponseDto CreateStagingRecordsFromCSV(RioDbContext dbContext, List<ParcelTransactionCSV> records, DateTime effectiveDate)
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
                    (decimal)ConvertMillimetersToAcreFeet(reportedValue, parcelAreaDictionary[recordGroup.Key].ParcelAreaInAcres)
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
}