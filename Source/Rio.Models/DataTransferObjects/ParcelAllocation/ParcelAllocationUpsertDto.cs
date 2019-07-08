using System.ComponentModel.DataAnnotations;

namespace Rio.Models.DataTransferObjects.ParcelAllocation
{
    public class ParcelAllocationUpsertDto
    {
        [Required]
        public int WaterYear { get; set; }
        public decimal? AcreFeetAllocated { get; set; }
    }
}