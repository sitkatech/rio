using System;
using Rio.Models.DataTransferObjects.Account;
using System.Linq;
using Rio.Models.DataTransferObjects;

namespace Rio.EFModels.Entities
{
    public static partial class AccountExtensionMethods
    {
        static partial void DoCustomMappings(Account account, AccountDto accountDto)
        {
            var userSimpleDtos = account.AccountUsers.Select(x => x.User.AsSimpleDto()).ToList();
            accountDto.Users = userSimpleDtos;
            accountDto.NumberOfUsers = userSimpleDtos.Count;
            accountDto.AccountDisplayName = $"{account.AccountName} (Account #{account.AccountNumber})";
            accountDto.ShortAccountDisplayName = $"{account.AccountName} (#{account.AccountNumber})";
            accountDto.NumberOfParcels = account.AccountParcelWaterYears.Count(x => x.WaterYear.Year == DateTime.Now.Year);
        }

        static partial void DoCustomSimpleDtoMappings(Account account, AccountSimpleDto accountSimpleDto)
        {
            accountSimpleDto.AccountDisplayName = $"{account.AccountName} (Account #{account.AccountNumber})";
            accountSimpleDto.ShortAccountDisplayName = $"#{account.AccountName} ({account.AccountNumber})";
        }

        public static AccountIncludeParcelsDto AsAccountWithParcelsDto(this Account account)
        {
            return new AccountIncludeParcelsDto()
            {
                Account = account.AsDto(),
                Parcels = account.AccountParcelWaterYears.Where(x => x.WaterYear.Year == DateTime.Now.Year).Select(x => x.Parcel.AsSimpleDto()).ToList()
            };
        }
    }
}