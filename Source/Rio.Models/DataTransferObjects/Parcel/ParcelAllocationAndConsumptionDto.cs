using System.Collections.Generic;
using Rio.Models.DataTransferObjects.ParcelAllocation;

namespace Rio.Models.DataTransferObjects.Parcel
{
    public class ParcelAllocationAndConsumptionDto
    {
        public int ParcelID { get; set; }
        public string ParcelNumber { get; set; }
        public double ParcelAreaInAcres { get; set; }
        public List<ParcelAllocationDto> Allocations { get; set; }
        public List<ParcelMonthlyEvapotranspirationDto> MonthlyEvapotranspiration { get; set; }
    }
}