using System.Linq;
using Rio.Models.DataTransferObjects;

namespace Rio.EFModels.Entities;

public static partial class ParcelUsageStagingExtensionMethods
{
    static partial void DoCustomSimpleDtoMappings(ParcelUsageStaging parcelUsageStaging, ParcelUsageStagingSimpleDto parcelUsageStagingSimpleDto)
    {
        var usagesForWaterMonth = parcelUsageStaging.Parcel?.ParcelLedgers
            .Where(x => x.TransactionTypeID == (int)TransactionTypeEnum.Usage && x.WaterYear == parcelUsageStaging.ReportedDate.Year && 
                        x.WaterMonth == parcelUsageStaging.ReportedDate.Month).ToList();

        var existingMonthlyUsageAmount = usagesForWaterMonth?.Sum(x => x.TransactionAmount);

        parcelUsageStagingSimpleDto.ExistingMonthlyUsageAmount = existingMonthlyUsageAmount ?? 0;
        parcelUsageStagingSimpleDto.UpdatedMonthlyUsageAmount = existingMonthlyUsageAmount + parcelUsageStaging.ReportedValueInAcreFeet ?? 0;

    }
}