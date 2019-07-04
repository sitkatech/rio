using System.ComponentModel.DataAnnotations;

namespace Rio.Models.DataTransferObjects.ParcelAllocation
{
    public class ParcelAllocationUpsertDto
    {
        [Required]
        public decimal AcreFeetAllocated { get; set; }
    }
}