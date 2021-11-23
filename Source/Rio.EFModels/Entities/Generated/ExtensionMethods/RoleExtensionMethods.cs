//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[Role]

using Rio.Models.DataTransferObjects;

namespace Rio.EFModels.Entities
{
    public static partial class RoleExtensionMethods
    {
        public static RoleDto AsDto(this Role role)
        {
            var roleDto = new RoleDto()
            {
                RoleID = role.RoleID,
                RoleName = role.RoleName,
                RoleDisplayName = role.RoleDisplayName,
                RoleDescription = role.RoleDescription,
                SortOrder = role.SortOrder
            };
            DoCustomMappings(role, roleDto);
            return roleDto;
        }

        static partial void DoCustomMappings(Role role, RoleDto roleDto);

        public static RoleSimpleDto AsSimpleDto(this Role role)
        {
            var roleSimpleDto = new RoleSimpleDto()
            {
                RoleID = role.RoleID,
                RoleName = role.RoleName,
                RoleDisplayName = role.RoleDisplayName,
                RoleDescription = role.RoleDescription,
                SortOrder = role.SortOrder
            };
            DoCustomSimpleDtoMappings(role, roleSimpleDto);
            return roleSimpleDto;
        }

        static partial void DoCustomSimpleDtoMappings(Role role, RoleSimpleDto roleSimpleDto);
    }
}