using System;
using System.Linq;
using Rio.Models.DataTransferObjects.Parcel;

namespace Rio.EFModels.Entities
{
    public static class ParcelExtensionMethods
    {
        public static ParcelDto AsDto(this Parcel parcel)
        {
            var currentYear = DateTime.Now.Year;
            return new ParcelDto()
            {
                ParcelID = parcel.ParcelID,
                ParcelNumber = parcel.ParcelNumber,
                ParcelAreaInAcres = parcel.ParcelAreaInAcres,
                LandOwner = parcel.UserParcel.Where(x=> x.EffectiveYear <= currentYear).OrderByDescending(x => x.SaleDate).FirstOrDefault()?.User.AsSimpleDto()
            };
        }
    }
}