using Rio.Models.DataTransferObjects.Parcel;
using System;
using System.Data;
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
                LandOwner = parcel.AccountParcelWaterYears.SingleOrDefault(x => x.WaterYear.Year == currentYear)?.Account?.AsSimpleDto(),
                ParcelStatusID = parcel.ParcelStatusID,
                InactivateDate = parcel.InactivateDate
            };
        }

        public static ParcelSimpleDto AsSimpleDto(this Parcel parcel)
        {
            return new ParcelSimpleDto()
            {
                ParcelID = parcel.ParcelID,
                ParcelNumber =  parcel.ParcelNumber
            };
        }
    }
}