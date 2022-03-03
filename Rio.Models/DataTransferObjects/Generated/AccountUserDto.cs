//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[AccountUser]
using System;


namespace Rio.Models.DataTransferObjects
{
    public partial class AccountUserDto
    {
        public int AccountUserID { get; set; }
        public UserDto User { get; set; }
        public AccountDto Account { get; set; }
    }

    public partial class AccountUserSimpleDto
    {
        public int AccountUserID { get; set; }
        public int UserID { get; set; }
        public int AccountID { get; set; }
    }

}