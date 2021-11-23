//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[AccountReconciliation]

using Rio.Models.DataTransferObjects;

namespace Rio.EFModels.Entities
{
    public static partial class AccountReconciliationExtensionMethods
    {
        public static AccountReconciliationDto AsDto(this AccountReconciliation accountReconciliation)
        {
            var accountReconciliationDto = new AccountReconciliationDto()
            {
                AccountReconciliationID = accountReconciliation.AccountReconciliationID,
                Parcel = accountReconciliation.Parcel.AsDto(),
                Account = accountReconciliation.Account.AsDto()
            };
            DoCustomMappings(accountReconciliation, accountReconciliationDto);
            return accountReconciliationDto;
        }

        static partial void DoCustomMappings(AccountReconciliation accountReconciliation, AccountReconciliationDto accountReconciliationDto);

        public static AccountReconciliationSimpleDto AsSimpleDto(this AccountReconciliation accountReconciliation)
        {
            var accountReconciliationSimpleDto = new AccountReconciliationSimpleDto()
            {
                AccountReconciliationID = accountReconciliation.AccountReconciliationID,
                ParcelID = accountReconciliation.ParcelID,
                AccountID = accountReconciliation.AccountID
            };
            DoCustomSimpleDtoMappings(accountReconciliation, accountReconciliationSimpleDto);
            return accountReconciliationSimpleDto;
        }

        static partial void DoCustomSimpleDtoMappings(AccountReconciliation accountReconciliation, AccountReconciliationSimpleDto accountReconciliationSimpleDto);
    }
}