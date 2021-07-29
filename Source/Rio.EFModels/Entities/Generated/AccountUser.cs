using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Rio.EFModels.Entities
{
    [Table("AccountUser")]
    public partial class AccountUser
    {
        [Key]
        public int AccountUserID { get; set; }
        public int UserID { get; set; }
        public int AccountID { get; set; }

        [ForeignKey(nameof(AccountID))]
        [InverseProperty("AccountUsers")]
        public virtual Account Account { get; set; }
        [ForeignKey(nameof(UserID))]
        [InverseProperty("AccountUsers")]
        public virtual User User { get; set; }
    }
}
