//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[User]
namespace Rio.EFModels.Entities
{
    public partial class User
    {
        public Role Role => Role.AllLookupDictionary[RoleID];
    }
}