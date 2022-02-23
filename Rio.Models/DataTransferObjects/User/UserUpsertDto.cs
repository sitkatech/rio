using System.ComponentModel.DataAnnotations;

namespace Rio.Models.DataTransferObjects.User
{
    public class UserUpsertDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string OrganizationName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        [Required]
        public int? RoleID { get; set; }
        [Required]
        public bool ReceiveSupportEmails { get; set; }
    }
}