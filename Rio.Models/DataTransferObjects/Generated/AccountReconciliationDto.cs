//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[AccountReconciliation]
using System;


namespace Rio.Models.DataTransferObjects
{
    public partial class AccountReconciliationDto
    {
        public int AccountReconciliationID { get; set; }
        public ParcelDto Parcel { get; set; }
        public AccountDto Account { get; set; }
    }

    public partial class AccountReconciliationSimpleDto
    {
        public int AccountReconciliationID { get; set; }
        public int ParcelID { get; set; }
        public int AccountID { get; set; }
    }

}