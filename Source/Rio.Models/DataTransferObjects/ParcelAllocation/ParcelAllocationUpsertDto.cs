using System.ComponentModel.DataAnnotations;

namespace Rio.Models.DataTransferObjects.ParcelAllocation
{
    public class ParcelAllocationUpsertDto
    {
        [Required]
        public int WaterYear { get; set; }
        [Required]
        public int WaterTypeID { get; set; }
        [Required]
        public decimal AcreFeetAllocated { get; set; }
    }
}