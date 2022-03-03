using System.Collections.Generic;

namespace Rio.Models.DataTransferObjects
{
    public partial class AccountDto
    {
        public List<UserSimpleDto> Users { get; set; }
        public string AccountDisplayName { get; set; }
        public string ShortAccountDisplayName { get; set; }
        public int NumberOfParcels { get; set; }
        public int NumberOfUsers { get; set; }
    }
}