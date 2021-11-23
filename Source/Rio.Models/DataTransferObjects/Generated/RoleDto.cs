//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[Role]
using System;


namespace Rio.Models.DataTransferObjects
{
    public partial class RoleDto
    {
        public int RoleID { get; set; }
        public string RoleName { get; set; }
        public string RoleDisplayName { get; set; }
        public string RoleDescription { get; set; }
        public int SortOrder { get; set; }
    }

    public partial class RoleSimpleDto
    {
        public int RoleID { get; set; }
        public string RoleName { get; set; }
        public string RoleDisplayName { get; set; }
        public string RoleDescription { get; set; }
        public int SortOrder { get; set; }
    }

}