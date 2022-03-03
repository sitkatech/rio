using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Rio.EFModels.Entities
{
    [Table("AccountStatus")]
    [Index(nameof(AccountStatusDisplayName), Name = "AK_AccountStatus_AccountStatusDisplayName", IsUnique = true)]
    [Index(nameof(AccountStatusName), Name = "AK_AccountStatus_AccountStatusName", IsUnique = true)]
    public partial class AccountStatus
    {
        public AccountStatus()
        {
            Accounts = new HashSet<Account>();
        }

        [Key]
        public int AccountStatusID { get; set; }
        [Required]
        [StringLength(20)]
        public string AccountStatusName { get; set; }
        [Required]
        [StringLength(20)]
        public string AccountStatusDisplayName { get; set; }

        [InverseProperty(nameof(Account.AccountStatus))]
        public virtual ICollection<Account> Accounts { get; set; }
    }
}
