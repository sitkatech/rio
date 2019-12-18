using System;
using System.Text;

namespace Rio.Models.DataTransferObjects.Account
{
    public class AccountSimpleDto
    {
        public int AccountID { get; set; }
        public int? AccountNumber { get; set; }
        public string AccountName { get; set; }
        public string Notes { get; set; }
    }
}
