using System;
using System.Collections.Generic;
using System.Text;
using Rio.Models.DataTransferObjects.User;

namespace Rio.Models.DataTransferObjects.Account
{
    public class AccountDto
    {
        public int? AccountNumber { get; set; }
        public string AccountName { get; set; }
        public string Notes { get; set; }
        public List<UserSimpleDto> Users { get; set; }
    }
}
