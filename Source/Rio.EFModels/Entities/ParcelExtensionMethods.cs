using Rio.Models.DataTransferObjects.Parcel;
using System;
using System.Linq;

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
                LandOwner = parcel.AccountParcel.Where(x=> x.EffectiveYear <= currentYear).OrderByDescending(x => x.SaleDate).FirstOrDefault()?.Account?.AsSimpleDto()
            };
        }
    }
}