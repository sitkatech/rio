//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[AccountOverconsumptionCharge]

using Rio.Models.DataTransferObjects;

namespace Rio.EFModels.Entities
{
    public static partial class AccountOverconsumptionChargeExtensionMethods
    {
        public static AccountOverconsumptionChargeDto AsDto(this AccountOverconsumptionCharge accountOverconsumptionCharge)
        {
            var accountOverconsumptionChargeDto = new AccountOverconsumptionChargeDto()
            {
                AccountOverconsumptionChargeID = accountOverconsumptionCharge.AccountOverconsumptionChargeID,
                Account = accountOverconsumptionCharge.Account.AsDto(),
                WaterYear = accountOverconsumptionCharge.WaterYear.AsDto(),
                OverconsumptionAmount = accountOverconsumptionCharge.OverconsumptionAmount,
                OverconsumptionCharge = accountOverconsumptionCharge.OverconsumptionCharge
            };
            DoCustomMappings(accountOverconsumptionCharge, accountOverconsumptionChargeDto);
            return accountOverconsumptionChargeDto;
        }

        static partial void DoCustomMappings(AccountOverconsumptionCharge accountOverconsumptionCharge, AccountOverconsumptionChargeDto accountOverconsumptionChargeDto);

        public static AccountOverconsumptionChargeSimpleDto AsSimpleDto(this AccountOverconsumptionCharge accountOverconsumptionCharge)
        {
            var accountOverconsumptionChargeSimpleDto = new AccountOverconsumptionChargeSimpleDto()
            {
                AccountOverconsumptionChargeID = accountOverconsumptionCharge.AccountOverconsumptionChargeID,
                AccountID = accountOverconsumptionCharge.AccountID,
                WaterYearID = accountOverconsumptionCharge.WaterYearID,
                OverconsumptionAmount = accountOverconsumptionCharge.OverconsumptionAmount,
                OverconsumptionCharge = accountOverconsumptionCharge.OverconsumptionCharge
            };
            DoCustomSimpleDtoMappings(accountOverconsumptionCharge, accountOverconsumptionChargeSimpleDto);
            return accountOverconsumptionChargeSimpleDto;
        }

        static partial void DoCustomSimpleDtoMappings(AccountOverconsumptionCharge accountOverconsumptionCharge, AccountOverconsumptionChargeSimpleDto accountOverconsumptionChargeSimpleDto);
    }
}