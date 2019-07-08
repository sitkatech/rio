using System.Collections.Generic;
using Rio.Models.DataTransferObjects.User;

namespace Rio.Models.DataTransferObjects.Parcel
{
    public class ParcelAllocationAndConsumptionDto
    {
        public int ParcelID { get; set; }
        public string ParcelNumber { get; set; }
        public double ParcelAreaInAcres { get; set; }
        public int WaterYear { get; set; }
        public decimal? AcreFeetAllocated { get; set; }

        public List<ParcelMonthlyEvapotranspirationDto> MonthlyEvapotranspiration { get; set; }
    }
}