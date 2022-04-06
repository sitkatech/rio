using System;
using System.ComponentModel.DataAnnotations;

namespace Rio.Models.DataTransferObjects.User
{
    public class UserCreateDto
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string LoginName { get; set; }
        [Required]
        public Guid UserGuid { get; set; }
        public string OrganizationName { get; set; }
        public string PhoneNumber { get; set; }
    }
}