using Rio.Models.DataTransferObjects.Parcel;

namespace Rio.EFModels.Entities
{
    public static class vParcelOwnershipExtensionMethods
    {
        public static ParcelOwnershipDto AsParcelOwnershipDto(this vParcelOwnership vParcelOwnership)
        {
            return new ParcelOwnershipDto()
            {
                Account = vParcelOwnership.AccountID.HasValue ? vParcelOwnership.Account.AsSimpleDto() : null,
                WaterYear = vParcelOwnership.WaterYear.AsDto()
            };
        }
    }
}