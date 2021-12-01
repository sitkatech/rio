using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Rio.EFModels.Entities
{
    [Table("ParcelLedgerEntrySourceType")]
    public partial class ParcelLedgerEntrySourceType
    {
        public ParcelLedgerEntrySourceType()
        {
            ParcelLedgers = new HashSet<ParcelLedger>();
        }

        [Key]
        public int ParcelLedgerEntrySourceTypeID { get; set; }
        [Required]
        [StringLength(50)]
        public string ParcelLedgerEntrySourceTypeName { get; set; }

        [InverseProperty(nameof(ParcelLedger.ParcelLedgerEntrySourceType))]
        public virtual ICollection<ParcelLedger> ParcelLedgers { get; set; }
    }
}
