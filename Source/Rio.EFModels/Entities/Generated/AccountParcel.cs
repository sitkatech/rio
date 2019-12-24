using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rio.EFModels.Entities
{
    public partial class AccountParcel
    {
        [Key]
        public int AccountParcelID { get; set; }
        public int AccountID { get; set; }
        public int ParcelID { get; set; }
        [StringLength(214)]
        public string OwnerName { get; set; }
        public int? EffectiveYear { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? SaleDate { get; set; }
        [StringLength(500)]
        public string Note { get; set; }

        [ForeignKey(nameof(AccountID))]
        [InverseProperty("AccountParcel")]
        public virtual Account Account { get; set; }
        [ForeignKey(nameof(ParcelID))]
        [InverseProperty("AccountParcel")]
        public virtual Parcel Parcel { get; set; }
    }
}
