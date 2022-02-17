using System.Collections.Generic;

namespace Rio.Models.DataTransferObjects
{
    public partial class ParcelDto
    {
        public AccountDto LandOwner { get; set; }
        public string? TagsAsCommaSeparatedString { get; set; }
    }
}