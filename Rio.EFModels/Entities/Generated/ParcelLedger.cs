using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Rio.EFModels.Entities
{
    [Table("ParcelLedger")]
    public partial class ParcelLedger
    {
        [Key]
        public int ParcelLedgerID { get; set; }
        public int ParcelID { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime TransactionDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime EffectiveDate { get; set; }
        public int TransactionTypeID { get; set; }
        [Column(TypeName = "decimal(10, 4)")]
        public decimal TransactionAmount { get; set; }
        public int? WaterTypeID { get; set; }
        [Required]
        [StringLength(200)]
        [Unicode(false)]
        public string TransactionDescription { get; set; }
        public int? UserID { get; set; }
        [Unicode(false)]
        public string UserComment { get; set; }
        public int ParcelLedgerEntrySourceTypeID { get; set; }
        [StringLength(100)]
        [Unicode(false)]
        public string UploadedFileName { get; set; }

        [ForeignKey("ParcelID")]
        [InverseProperty("ParcelLedgers")]
        public virtual Parcel Parcel { get; set; }
        [ForeignKey("UserID")]
        [InverseProperty("ParcelLedgers")]
        public virtual User User { get; set; }
        [ForeignKey("WaterTypeID")]
        [InverseProperty("ParcelLedgers")]
        public virtual WaterType WaterType { get; set; }
    }
}
