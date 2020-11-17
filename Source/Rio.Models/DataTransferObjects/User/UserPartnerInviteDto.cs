using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Rio.Models.Validation;

namespace Rio.Models.DataTransferObjects.User
{
    public class UserPartnerInviteDto
    {
        [Required(ErrorMessage = "First name is required")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Last name is required")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }
        [EnsureMinimumElements(1, ErrorMessage = "Please select at least one Account to grant the new user access to")]
        public List<int> AccountIDs { get; set; }
    }
}
