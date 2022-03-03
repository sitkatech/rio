using Rio.Models.DataTransferObjects.Parcel;

namespace Rio.EFModels.Entities
{
    public static partial class AccountParcelWaterYearExtensionMethods
    {
        public static ParcelOwnershipDto AsParcelOwnershipDto(this AccountParcelWaterYear accountParcelWaterYear)
        {
            return new ParcelOwnershipDto()
            {
                Account = accountParcelWaterYear.Account.AsSimpleDto(),
                WaterYear = accountParcelWaterYear.WaterYear.AsDto()
            };
        }
    }
}