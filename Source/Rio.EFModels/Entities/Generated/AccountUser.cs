using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rio.EFModels.Entities
{
    public partial class AccountUser
    {
        [Key]
        public int AccountUserID { get; set; }
        public int UserID { get; set; }
        public int AccountID { get; set; }

        [ForeignKey(nameof(AccountID))]
        [InverseProperty("AccountUser")]
        public virtual Account Account { get; set; }
        [ForeignKey(nameof(UserID))]
        [InverseProperty("AccountUser")]
        public virtual User User { get; set; }
    }
}
