using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rio.EFModels.Entities
{
    public partial class AccountStatus
    {
        public AccountStatus()
        {
            Account = new HashSet<Account>();
        }

        [Key]
        public int AccountStatusID { get; set; }
        [Required]
        [StringLength(20)]
        public string AccountStatusName { get; set; }
        [Required]
        [StringLength(20)]
        public string AccountStatusDisplayName { get; set; }

        [InverseProperty("AccountStatus")]
        public virtual ICollection<Account> Account { get; set; }
    }
}
