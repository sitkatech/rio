using System.Linq;
using Rio.Models.DataTransferObjects.Account;

namespace Rio.EFModels.Entities
{
    public static class AccountExtensionMethods
    {
        public static AccountDto AsDto(this Account account)
        {
            return new AccountDto()
            {
                AccountNumber = account.AccountNumber,
                AccountName = account.AccountName,
                Notes = account.Notes,
                Users = account.AccountUser.Select(x=>UserExtensionMethods.AsSimpleDto(x.User)).ToList()
            };
        }
    }
}