//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[Account]

using Rio.Models.DataTransferObjects;

namespace Rio.EFModels.Entities
{
    public static partial class AccountExtensionMethods
    {
        public static AccountDto AsDto(this Account account)
        {
            var accountDto = new AccountDto()
            {
                AccountID = account.AccountID,
                AccountNumber = account.AccountNumber,
                AccountName = account.AccountName,
                AccountStatus = account.AccountStatus.AsDto(),
                Notes = account.Notes,
                UpdateDate = account.UpdateDate,
                AccountVerificationKey = account.AccountVerificationKey,
                AccountVerificationKeyLastUseDate = account.AccountVerificationKeyLastUseDate,
                CreateDate = account.CreateDate,
                InactivateDate = account.InactivateDate
            };
            DoCustomMappings(account, accountDto);
            return accountDto;
        }

        static partial void DoCustomMappings(Account account, AccountDto accountDto);

        public static AccountSimpleDto AsSimpleDto(this Account account)
        {
            var accountSimpleDto = new AccountSimpleDto()
            {
                AccountID = account.AccountID,
                AccountNumber = account.AccountNumber,
                AccountName = account.AccountName,
                AccountStatusID = account.AccountStatusID,
                Notes = account.Notes,
                UpdateDate = account.UpdateDate,
                AccountVerificationKey = account.AccountVerificationKey,
                AccountVerificationKeyLastUseDate = account.AccountVerificationKeyLastUseDate,
                CreateDate = account.CreateDate,
                InactivateDate = account.InactivateDate
            };
            DoCustomSimpleDtoMappings(account, accountSimpleDto);
            return accountSimpleDto;
        }

        static partial void DoCustomSimpleDtoMappings(Account account, AccountSimpleDto accountSimpleDto);
    }
}