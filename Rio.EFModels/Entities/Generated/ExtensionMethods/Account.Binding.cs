//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[Account]
namespace Rio.EFModels.Entities
{
    public partial class Account
    {
        public AccountStatus AccountStatus => AccountStatus.AllLookupDictionary[AccountStatusID];
    }
}