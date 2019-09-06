using System.Collections.Generic;
using Rio.Models.DataTransferObjects.User;

namespace Rio.Models.DataTransferObjects.Parcel
{
    public class ParcelWithWaterUsageDto : ParcelDto
    {
        public decimal? WaterUsageFor2016 { get; set; }
        public decimal? WaterUsageFor2017 { get; set; }
        public decimal? WaterUsageFor2018 { get; set; }
        public decimal? AllocationFor2016 { get; set; }
        public decimal? AllocationFor2017 { get; set; }
        public decimal? AllocationFor2018 { get; set; }
    }
}