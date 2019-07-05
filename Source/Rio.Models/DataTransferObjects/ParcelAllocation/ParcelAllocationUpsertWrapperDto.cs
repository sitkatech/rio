using System.Collections.Generic;

namespace Rio.Models.DataTransferObjects.ParcelAllocation
{
    public class ParcelAllocationUpsertWrapperDto
    {
        public List<ParcelAllocationUpsertDto> ParcelAllocations { get; set; }
    }
}