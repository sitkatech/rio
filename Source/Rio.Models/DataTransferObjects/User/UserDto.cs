using Rio.Models.DataTransferObjects.Role;
using System;

namespace Rio.Models.DataTransferObjects.User
{
    public class UserDto : UserSimpleDto
    {
        public Guid? UserGuid { get; set; }
        public string Phone { get; set; }
        public string LoginName { get; set; }
        public RoleDto Role { get; set; }
    }
}
