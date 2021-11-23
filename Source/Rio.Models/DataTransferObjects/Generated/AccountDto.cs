//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[Account]
using System;


namespace Rio.Models.DataTransferObjects
{
    public partial class AccountDto
    {
        public int AccountID { get; set; }
        public int AccountNumber { get; set; }
        public string AccountName { get; set; }
        public AccountStatusDto AccountStatus { get; set; }
        public string Notes { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string AccountVerificationKey { get; set; }
        public DateTime? AccountVerificationKeyLastUseDate { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? InactivateDate { get; set; }
    }

    public partial class AccountSimpleDto
    {
        public int AccountID { get; set; }
        public int AccountNumber { get; set; }
        public string AccountName { get; set; }
        public int AccountStatusID { get; set; }
        public string Notes { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string AccountVerificationKey { get; set; }
        public DateTime? AccountVerificationKeyLastUseDate { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? InactivateDate { get; set; }
    }

}