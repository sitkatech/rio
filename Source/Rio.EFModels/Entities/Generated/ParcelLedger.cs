using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Rio.EFModels.Entities
{
    [Table("ParcelLedger")]
    [Index(nameof(ParcelID), nameof(TransactionDate), nameof(TransactionTypeID), nameof(WaterTypeID), Name = "AK_ParcelLedger_ParcelID_TransactionDate_TransactionTypeID_WaterTypeID", IsUnique = true)]
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
        public string TransactionDescription { get; set; }
        public int? UserID { get; set; }
        public string UserComment { get; set; }

        [ForeignKey(nameof(ParcelID))]
        [InverseProperty("ParcelLedgers")]
        public virtual Parcel Parcel { get; set; }
        [ForeignKey(nameof(TransactionTypeID))]
        [InverseProperty("ParcelLedgers")]
        public virtual TransactionType TransactionType { get; set; }
        [ForeignKey(nameof(WaterTypeID))]
        [InverseProperty("ParcelLedgers")]
        public virtual WaterType WaterType { get; set; }
    }
}
