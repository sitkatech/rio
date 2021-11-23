//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[AccountParcelWaterYear]

using Rio.Models.DataTransferObjects;

namespace Rio.EFModels.Entities
{
    public static partial class AccountParcelWaterYearExtensionMethods
    {
        public static AccountParcelWaterYearDto AsDto(this AccountParcelWaterYear accountParcelWaterYear)
        {
            var accountParcelWaterYearDto = new AccountParcelWaterYearDto()
            {
                AccountParcelWaterYearID = accountParcelWaterYear.AccountParcelWaterYearID,
                Account = accountParcelWaterYear.Account.AsDto(),
                Parcel = accountParcelWaterYear.Parcel.AsDto(),
                WaterYear = accountParcelWaterYear.WaterYear.AsDto()
            };
            DoCustomMappings(accountParcelWaterYear, accountParcelWaterYearDto);
            return accountParcelWaterYearDto;
        }

        static partial void DoCustomMappings(AccountParcelWaterYear accountParcelWaterYear, AccountParcelWaterYearDto accountParcelWaterYearDto);

        public static AccountParcelWaterYearSimpleDto AsSimpleDto(this AccountParcelWaterYear accountParcelWaterYear)
        {
            var accountParcelWaterYearSimpleDto = new AccountParcelWaterYearSimpleDto()
            {
                AccountParcelWaterYearID = accountParcelWaterYear.AccountParcelWaterYearID,
                AccountID = accountParcelWaterYear.AccountID,
                ParcelID = accountParcelWaterYear.ParcelID,
                WaterYearID = accountParcelWaterYear.WaterYearID
            };
            DoCustomSimpleDtoMappings(accountParcelWaterYear, accountParcelWaterYearSimpleDto);
            return accountParcelWaterYearSimpleDto;
        }

        static partial void DoCustomSimpleDtoMappings(AccountParcelWaterYear accountParcelWaterYear, AccountParcelWaterYearSimpleDto accountParcelWaterYearSimpleDto);
    }
}