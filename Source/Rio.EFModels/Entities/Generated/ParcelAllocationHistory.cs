using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Rio.EFModels.Entities
{
    [Table("ParcelAllocationHistory")]
    public partial class ParcelAllocationHistory
    {
        [Key]
        public int ParcelAllocationHistoryID { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime ParcelAllocationHistoryDate { get; set; }
        public int ParcelAllocationHistoryWaterYear { get; set; }
        public int ParcelAllocationTypeID { get; set; }
        public int UserID { get; set; }
        public int? FileResourceID { get; set; }
        [Column(TypeName = "decimal(10, 4)")]
        public decimal? ParcelAllocationHistoryValue { get; set; }

        [ForeignKey(nameof(FileResourceID))]
        [InverseProperty("ParcelAllocationHistories")]
        public virtual FileResource FileResource { get; set; }
        [ForeignKey(nameof(ParcelAllocationTypeID))]
        [InverseProperty("ParcelAllocationHistories")]
        public virtual ParcelAllocationType ParcelAllocationType { get; set; }
        [ForeignKey(nameof(UserID))]
        [InverseProperty("ParcelAllocationHistories")]
        public virtual User User { get; set; }
    }
}
