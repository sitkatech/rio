using Rio.Models.DataTransferObjects.User;
using System.Collections.Generic;

namespace Rio.Models.DataTransferObjects.Account
{
    public class AccountDto
    {
        public int AccountID { get; set; }
        public int? AccountNumber { get; set; }
        public string AccountName { get; set; }
        public string Notes { get; set; }
        public List<UserSimpleDto> Users { get; set; }
        public AccountStatusDto AccountStatus { get; set; }
        public string AccountDisplayName { get; set; }
    }
}