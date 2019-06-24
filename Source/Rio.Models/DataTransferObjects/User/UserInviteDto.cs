using System.ComponentModel.DataAnnotations;

namespace Rio.Models.DataTransferObjects.User
{
    public class UserInviteDto
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public int? RoleID { get; set; }
    }
}