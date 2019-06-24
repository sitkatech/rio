using Rio.Models.DataTransferObjects.Role;

namespace Rio.EFModels.Entities
{
    public static class RoleExtensionMethods
    {
        public static RoleDto AsDto(this Role role)
        {
            return new RoleDto()
            {
                RoleID = role.RoleID,
                RoleName = role.RoleName,
                RoleDisplayName = role.RoleDisplayName
            };
        }
    }
}