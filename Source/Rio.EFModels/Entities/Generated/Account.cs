using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rio.EFModels.Entities
{
    public partial class Account
    {
        public Account()
        {
            AccountParcel = new HashSet<AccountParcel>();
            AccountUser = new HashSet<AccountUser>();
        }

        [Key]
        public int AccountID { get; set; }
        public int AccountNumber { get; set; }
        [StringLength(255)]
        public string AccountName { get; set; }
        public int AccountStatusID { get; set; }
        public string Notes { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }

        [ForeignKey(nameof(AccountStatusID))]
        [InverseProperty("Account")]
        public virtual AccountStatus AccountStatus { get; set; }
        [InverseProperty("Account")]
        public virtual ICollection<AccountParcel> AccountParcel { get; set; }
        [InverseProperty("Account")]
        public virtual ICollection<AccountUser> AccountUser { get; set; }
    }
}
