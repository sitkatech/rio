//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[AccountUser]

using Rio.Models.DataTransferObjects;

namespace Rio.EFModels.Entities
{
    public static partial class AccountUserExtensionMethods
    {
        public static AccountUserDto AsDto(this AccountUser accountUser)
        {
            var accountUserDto = new AccountUserDto()
            {
                AccountUserID = accountUser.AccountUserID,
                User = accountUser.User.AsDto(),
                Account = accountUser.Account.AsDto()
            };
            DoCustomMappings(accountUser, accountUserDto);
            return accountUserDto;
        }

        static partial void DoCustomMappings(AccountUser accountUser, AccountUserDto accountUserDto);

        public static AccountUserSimpleDto AsSimpleDto(this AccountUser accountUser)
        {
            var accountUserSimpleDto = new AccountUserSimpleDto()
            {
                AccountUserID = accountUser.AccountUserID,
                UserID = accountUser.UserID,
                AccountID = accountUser.AccountID
            };
            DoCustomSimpleDtoMappings(accountUser, accountUserSimpleDto);
            return accountUserSimpleDto;
        }

        static partial void DoCustomSimpleDtoMappings(AccountUser accountUser, AccountUserSimpleDto accountUserSimpleDto);
    }
}