using System.Collections.Generic;

namespace Rio.Models.DataTransferObjects
{
    public class TagBulkSetUpsertDto
    {
        public TagDto TagDto { get; set; }
        public List<int> parcelIDs { get; set; }
    }
}