using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Rio.EFModels.Entities
{
    [Table("TransactionType")]
    public partial class TransactionType
    {
        public TransactionType()
        {
            ParcelLedgers = new HashSet<ParcelLedger>();
        }

        [Key]
        public int TransactionTypeID { get; set; }
        [Required]
        [StringLength(50)]
        public string TransactionTypeName { get; set; }
        public int SortOrder { get; set; }

        [InverseProperty(nameof(ParcelLedger.TransactionType))]
        public virtual ICollection<ParcelLedger> ParcelLedgers { get; set; }
    }
}
