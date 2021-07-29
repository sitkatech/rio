using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Rio.EFModels.Entities
{
    [Table("AccountReconciliation")]
    public partial class AccountReconciliation
    {
        [Key]
        public int AccountReconciliationID { get; set; }
        public int ParcelID { get; set; }
        public int AccountID { get; set; }

        [ForeignKey(nameof(AccountID))]
        [InverseProperty("AccountReconciliations")]
        public virtual Account Account { get; set; }
        [ForeignKey(nameof(ParcelID))]
        [InverseProperty("AccountReconciliations")]
        public virtual Parcel Parcel { get; set; }
    }
}
