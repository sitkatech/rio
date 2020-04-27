using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rio.EFModels.Entities
{
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
        [InverseProperty("ParcelAllocationHistory")]
        public virtual FileResource FileResource { get; set; }
        [ForeignKey(nameof(ParcelAllocationTypeID))]
        [InverseProperty("ParcelAllocationHistory")]
        public virtual ParcelAllocationType ParcelAllocationType { get; set; }
        [ForeignKey(nameof(UserID))]
        [InverseProperty("ParcelAllocationHistory")]
        public virtual User User { get; set; }
    }
}
