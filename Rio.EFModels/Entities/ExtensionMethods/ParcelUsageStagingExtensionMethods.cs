using System.Linq;
using Rio.Models.DataTransferObjects;

namespace Rio.EFModels.Entities;

public static partial class ParcelUsageStagingExtensionMethods
{
    static partial void DoCustomSimpleDtoMappings(ParcelUsageStaging parcelUsageStaging, ParcelUsageStagingSimpleDto parcelUsageStagingSimpleDto)
    {
        var usagesForWaterYear = parcelUsageStaging.Parcel.ParcelLedgers
            .Where(x => x.TransactionTypeID == (int)TransactionTypeEnum.Usage && x.WaterYear == parcelUsageStaging.ReportedDate.Year).ToList();
        var usagesForWaterMonth = usagesForWaterYear.Where(x => x.WaterMonth == parcelUsageStaging.ReportedDate.Month);

        var existingAnnualUsageAmount = usagesForWaterYear.Sum(x => x.TransactionAmount);
        var existingMonthlyUsageAmount = usagesForWaterMonth.Sum(x => x.TransactionAmount);

        parcelUsageStagingSimpleDto.ExistingAnnualUsageAmount = existingAnnualUsageAmount;
        parcelUsageStagingSimpleDto.ExistingMonthlyUsageAmount = existingMonthlyUsageAmount;

        parcelUsageStagingSimpleDto.UpdatedAnnualUsageAmount = existingAnnualUsageAmount + parcelUsageStaging.ReportedValueInAcreFeet;
        parcelUsageStagingSimpleDto.UpdatedMonthlyUsageAmount = existingMonthlyUsageAmount + parcelUsageStaging.ReportedValueInAcreFeet;

    }
}