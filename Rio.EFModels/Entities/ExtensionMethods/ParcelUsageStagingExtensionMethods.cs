using System.Linq;
using Rio.Models.DataTransferObjects;

namespace Rio.EFModels.Entities;

public static partial class ParcelUsageStagingExtensionMethods
{
    static partial void DoCustomSimpleDtoMappings(ParcelUsageStaging parcelUsageStaging, ParcelUsageStagingSimpleDto parcelUsageStagingSimpleDto)
    {
        var usageToDate = parcelUsageStaging.Parcel.ParcelLedgers
            .Where(x => x.TransactionTypeID == (int)TransactionTypeEnum.Usage)
            .Sum(x => x.TransactionAmount);

        parcelUsageStagingSimpleDto.ExistingUsageToDate = usageToDate;
        parcelUsageStagingSimpleDto.UpdatedUsageToDate = usageToDate + parcelUsageStaging.ReportedValueInAcreFeet;
    }

}