using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Rio.EFModels.Entities
{
    [Table("ParcelLedger")]
    [Index(nameof(ParcelID), nameof(TransactionDate), nameof(TransactionTypeID), Name = "AK_ParcelLedger_ParcelID_TransactionDate_TransactionTypeID", IsUnique = true)]
    public partial class ParcelLedger
    {
        [Key]
        public int ParcelLedgerID { get; set; }
        public int ParcelID { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime TransactionDate { get; set; }
        public int TransactionTypeID { get; set; }
        public double TransactionAmount { get; set; }
        [Required]
        [StringLength(100)]
        public string TransactionDescription { get; set; }

        [ForeignKey(nameof(ParcelID))]
        [InverseProperty("ParcelLedgers")]
        public virtual Parcel Parcel { get; set; }
        [ForeignKey(nameof(TransactionTypeID))]
        [InverseProperty("ParcelLedgers")]
        public virtual TransactionType TransactionType { get; set; }
    }
}
