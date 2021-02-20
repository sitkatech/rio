using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rio.EFModels.Entities
{
    public partial class AccountReconciliation
    {
        [Key]
        public int AccountReconciliationID { get; set; }
        public int ParcelID { get; set; }
        public int AccountID { get; set; }

        [ForeignKey(nameof(AccountID))]
        [InverseProperty("AccountReconciliation")]
        public virtual Account Account { get; set; }
        [ForeignKey(nameof(ParcelID))]
        [InverseProperty("AccountReconciliation")]
        public virtual Parcel Parcel { get; set; }
    }
}
