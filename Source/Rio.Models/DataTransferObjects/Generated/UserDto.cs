//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[User]
using System;


namespace Rio.Models.DataTransferObjects
{
    public partial class UserDto
    {
        public int UserID { get; set; }
        public Guid? UserGuid { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public RoleDto Role { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public DateTime? LastActivityDate { get; set; }
        public DateTime? DisclaimerAcknowledgedDate { get; set; }
        public bool ReceiveSupportEmails { get; set; }
        public string LoginName { get; set; }
        public string Company { get; set; }
    }

    public partial class UserSimpleDto
    {
        public int UserID { get; set; }
        public Guid? UserGuid { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public int RoleID { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public DateTime? LastActivityDate { get; set; }
        public DateTime? DisclaimerAcknowledgedDate { get; set; }
        public bool ReceiveSupportEmails { get; set; }
        public string LoginName { get; set; }
        public string Company { get; set; }
    }

}