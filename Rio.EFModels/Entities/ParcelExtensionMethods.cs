using System;
using System.Linq;
using Rio.Models.DataTransferObjects;

namespace Rio.EFModels.Entities
{
    public static partial class ParcelExtensionMethods
    {
        static partial void DoCustomMappings(Parcel parcel, ParcelDto parcelDto)
        {
            var currentYear = DateTime.Now.Year;
            parcelDto.LandOwner = parcel.AccountParcelWaterYears.SingleOrDefault(x => x.WaterYear.Year == currentYear)
                ?.Account?.AsDto();
        }
    }
}