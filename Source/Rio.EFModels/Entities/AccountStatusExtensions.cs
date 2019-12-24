using Rio.Models.DataTransferObjects.Account;

namespace Rio.EFModels.Entities
{
    public static class AccountStatusExtensions
    {
        public static AccountStatusDto AsDto(this AccountStatus accountStatus)
        {
            return new AccountStatusDto()
            {
                AccountStatusID = accountStatus?.AccountStatusID,
                AccountStatusDisplayName =  accountStatus?.AccountStatusDisplayName
            };
        }
    }
}