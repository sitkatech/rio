using System.Collections.Generic;
using System.Linq;
using Rio.Models.DataTransferObjects.Parcel;
using Rio.Models.DataTransferObjects.ParcelAllocation;

namespace Rio.EFModels.Entities
{
    public static class ParcelExtensionMethods
    {
        public static ParcelDto AsDto(this Parcel parcel)
        {
            return new ParcelDto()
            {
                ParcelID = parcel.ParcelID,
                ParcelNumber = parcel.ParcelNumber,
                ParcelAreaInAcres = parcel.ParcelAreaInAcres,
                LandOwner = parcel.UserParcel.SingleOrDefault()?.User.AsSimpleDto()
            };
        }

        public static List<ParcelAllocationAndConsumptionDto> CreateParcelAllocationAndConsumptionDtos(List<int> waterYears, IEnumerable<ParcelDto> parcelDtos,
            List<ParcelAllocationDto> parcelAllocationDtos, List<ParcelMonthlyEvapotranspirationDto> parcelMonthlyEvapotranspirationDtos)
        {
            var parcelAllocationAndConsumptionDtos = new List<ParcelAllocationAndConsumptionDto>();
            foreach (var parcelDto in parcelDtos)
            {
                foreach (var waterYear in waterYears)
                {
                    var parcelAllocationAndConsumptionDto = new ParcelAllocationAndConsumptionDto()
                    {
                        ParcelID = parcelDto.ParcelID,
                        ParcelNumber = parcelDto.ParcelNumber,
                        ParcelAreaInAcres = parcelDto.ParcelAreaInAcres,
                        WaterYear = waterYear
                    };
                    var parcelAllocationDtoForThisYear =
                        parcelAllocationDtos.SingleOrDefault(x => x.ParcelID == parcelDto.ParcelID && x.WaterYear == waterYear);
                    if (parcelAllocationDtoForThisYear != null)
                    {
                        parcelAllocationAndConsumptionDto.AcreFeetAllocated =
                            parcelAllocationDtoForThisYear.AcreFeetAllocated;
                    }

                    parcelAllocationAndConsumptionDto.MonthlyEvapotranspiration = parcelMonthlyEvapotranspirationDtos
                        .Where(x => x.ParcelID == parcelDto.ParcelID && x.WaterYear == waterYear).ToList();
                    parcelAllocationAndConsumptionDtos.Add(parcelAllocationAndConsumptionDto);
                }
            }
            return parcelAllocationAndConsumptionDtos;
        }
    }
}