using System;
using Rio.Models.DataTransferObjects.Role;

namespace Rio.Models.DataTransferObjects.User
{
    public class UserDto
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
        public bool IsActive { get; set; }
        public int OrganizationId { get; set; }
        public bool ReceiveSupportEmails { get; set; }
        public string LoginName { get; set; }

        public string FullName => $"{FirstName} {LastName}";

        public static string GetListRoute => "users";

        public static string GetSingleRouteByUserID(int userID)
        {
            return $"users/{userID}";
        }

        public static string GetSingleRouteByGlobalID(string globalID)
        {
            return $"user-claims/{globalID}";
        }

        public static string GetSingleRouteByGlobalID(Guid globalID)
        {
            return $"user-claims/{globalID.ToString()}";
        }
    }
}
