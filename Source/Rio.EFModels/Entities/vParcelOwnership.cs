using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Rio.Models.DataTransferObjects.Parcel;
using Rio.Models.DataTransferObjects.User;

namespace Rio.EFModels.Entities
{
    public partial class vParcelOwnership
    {
        [ForeignKey(nameof(ParcelID))]
        public virtual Parcel Parcel { get; set; }

        [ForeignKey(nameof(UserID))]
        public virtual User User { get; set; }


    }

    public static class vParcelOwnershipExtensionMethods
    {
        public static ParcelDto AsParcelDto(this vParcelOwnership vParcelOwnership)
        {
            UserSimpleDto landOwner;

            if (vParcelOwnership.User != null)
            {
                landOwner = vParcelOwnership.User.AsSimpleDto();
            }
            else
            {
                landOwner = new UserSimpleDto()
                {
                    FirstName = vParcelOwnership.OwnerName
                };
            }

            var parcelDto = new ParcelDto
            {
                ParcelID = vParcelOwnership.ParcelID,
                ParcelNumber = vParcelOwnership.Parcel.ParcelNumber,
                ParcelAreaInAcres = vParcelOwnership.Parcel.ParcelAreaInAcres
            };



            return parcelDto;
        }
    }
}