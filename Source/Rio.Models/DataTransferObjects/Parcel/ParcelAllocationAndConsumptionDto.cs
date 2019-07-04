using System.Collections.Generic;
using Rio.Models.DataTransferObjects.User;

namespace Rio.Models.DataTransferObjects.Parcel
{
    public class ParcelAllocationAndConsumptionDto
    {
        public int WaterYear { get; set; }
        public decimal AcreFeetAllocated { get; set; }

        public List<ParcelMonthlyUsageDto> MonthlyConsumption { get; set; }
    }
}