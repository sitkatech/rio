using System.Collections.Generic;

namespace Rio.EFModels.Entities
{
    public partial class ParcelWithAnnualWaterUsage
    {
        public ParcelWithAnnualWaterUsage()
        {
        }

        public int ParcelID { get; set; }
        public string ParcelNumber { get; set; }
        public double ParcelAreaInAcres { get; set; }
        public int? UserID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public decimal? AllocationFor2016 { get; set; }
        public decimal? AllocationFor2017 { get; set; }
        public decimal? AllocationFor2018 { get; set; }
        public decimal? WaterUsageFor2016 { get; set; }
        public decimal? WaterUsageFor2017 { get; set; }
        public decimal? WaterUsageFor2018 { get; set; }
    }
}