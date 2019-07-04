namespace Rio.Models.DataTransferObjects.ParcelAllocation
{
    public class ParcelAllocationDto
    {
        public int ParcelAllocationID { get; set; }
        public int ParcelID { get; set; }
        public int WaterYear { get; set; }
        public decimal AcreFeetAllocated { get; set; }
    }
}