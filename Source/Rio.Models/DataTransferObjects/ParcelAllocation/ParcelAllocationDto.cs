namespace Rio.Models.DataTransferObjects.ParcelAllocation
{
    public class ParcelAllocationDto
    {
        public int ParcelID { get; set; }
        public int WaterYear { get; set; }
        public int ParcelAllocationTypeID { get; set; }
        public decimal AcreFeetAllocated { get; set; }
    }
}