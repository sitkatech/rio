using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Rio.EFModels.Entities
{
    [Table("Role")]
    [Index(nameof(RoleDisplayName), Name = "AK_Role_RoleDisplayName", IsUnique = true)]
    [Index(nameof(RoleName), Name = "AK_Role_RoleName", IsUnique = true)]
    public partial class Role
    {
        public Role()
        {
            Users = new HashSet<User>();
        }

        [Key]
        public int RoleID { get; set; }
        [Required]
        [StringLength(100)]
        public string RoleName { get; set; }
        [Required]
        [StringLength(100)]
        public string RoleDisplayName { get; set; }
        [StringLength(255)]
        public string RoleDescription { get; set; }
        public int SortOrder { get; set; }

        [InverseProperty(nameof(User.Role))]
        public virtual ICollection<User> Users { get; set; }
    }
}
