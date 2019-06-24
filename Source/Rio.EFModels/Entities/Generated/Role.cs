using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rio.EFModels.Entities
{
    public partial class Role
    {
        public Role()
        {
            User = new HashSet<User>();
        }

        [Column("RoleID")]
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

        [InverseProperty("Role")]
        public virtual ICollection<User> User { get; set; }
    }
}
