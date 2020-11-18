﻿using System.Collections.Generic;
using Rio.Models.DataTransferObjects.Account;
using System.Linq;
using Rio.Models.DataTransferObjects.User;

namespace Rio.EFModels.Entities
{
    public static class AccountExtensionMethods
    {
        public static AccountSimpleDto AsSimpleDto(this Account account)
        {
            return new AccountSimpleDto()
            {
                AccountID = account.AccountID,
                AccountNumber = account.AccountNumber,
                AccountName = account.AccountName,
                AccountVerificationKey = account.AccountVerificationKey,
                Notes = account.Notes,
                AccountDisplayName = $"Account #{account.AccountNumber} ({account.AccountName})",
                ShortAccountDisplayName = $"#{account.AccountNumber} ({account.AccountName})"
            };
        }
        public static AccountDto AsDto(this Account account)
        {
            var userSimpleDtos = account.AccountUser.Select(x=>x.User.AsSimpleDto()).ToList();
            return new AccountDto()
            {
                AccountID = account.AccountID,
                AccountNumber = account.AccountNumber,
                AccountName = account.AccountName,
                Notes = account.Notes,
                AccountVerificationKey = account.AccountVerificationKey,
                Users = userSimpleDtos,
                NumberOfUsers = userSimpleDtos.Count,
                AccountStatus = account.AccountStatus.AsDto(),
                AccountDisplayName = $"Account #{account.AccountNumber} ({account.AccountName})",
                ShortAccountDisplayName = $"#{account.AccountNumber} ({account.AccountName})",
                NumberOfParcels = account.AccountParcel.Count
            };
        }

        public static AccountIncludeParcelsDto AsAccountWithParcelsDto(this Account account)
        {
            return new AccountIncludeParcelsDto()
            {
                Account = account.AsDto(),
                Parcels = account.AccountParcel.Select(x => x.Parcel.AsSimpleDto()).ToList()
            };
        }
    }
}