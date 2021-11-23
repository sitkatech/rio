//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[AccountStatus]

using Rio.Models.DataTransferObjects;

namespace Rio.EFModels.Entities
{
    public static partial class AccountStatusExtensionMethods
    {
        public static AccountStatusDto AsDto(this AccountStatus accountStatus)
        {
            var accountStatusDto = new AccountStatusDto()
            {
                AccountStatusID = accountStatus.AccountStatusID,
                AccountStatusName = accountStatus.AccountStatusName,
                AccountStatusDisplayName = accountStatus.AccountStatusDisplayName
            };
            DoCustomMappings(accountStatus, accountStatusDto);
            return accountStatusDto;
        }

        static partial void DoCustomMappings(AccountStatus accountStatus, AccountStatusDto accountStatusDto);

        public static AccountStatusSimpleDto AsSimpleDto(this AccountStatus accountStatus)
        {
            var accountStatusSimpleDto = new AccountStatusSimpleDto()
            {
                AccountStatusID = accountStatus.AccountStatusID,
                AccountStatusName = accountStatus.AccountStatusName,
                AccountStatusDisplayName = accountStatus.AccountStatusDisplayName
            };
            DoCustomSimpleDtoMappings(accountStatus, accountStatusSimpleDto);
            return accountStatusSimpleDto;
        }

        static partial void DoCustomSimpleDtoMappings(AccountStatus accountStatus, AccountStatusSimpleDto accountStatusSimpleDto);
    }
}