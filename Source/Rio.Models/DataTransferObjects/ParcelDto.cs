using System.Collections.Generic;

namespace Rio.Models.DataTransferObjects
{
    public partial class ParcelDto
    {
        public AccountDto LandOwner { get; set; }
        public List<TagDto>? Tags { get; set; }
    }
}