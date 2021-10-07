using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Rio.EFModels.Entities
{
    [Table("WaterType")]
    [Index(nameof(WaterTypeName), Name = "AK_WaterType_WaterTypeName", IsUnique = true)]
    public partial class WaterType
    {
        public WaterType()
        {
            ParcelAllocationHistories = new HashSet<ParcelAllocationHistory>();
            ParcelAllocations = new HashSet<ParcelAllocation>();
            ParcelLedgers = new HashSet<ParcelLedger>();
        }

        [Key]
        public int WaterTypeID { get; set; }
        [Required]
        [StringLength(50)]
        public string WaterTypeName { get; set; }
        public bool IsAppliedProportionally { get; set; }
        public string WaterTypeDefinition { get; set; }
        public bool IsSourcedFromApi { get; set; }
        public int SortOrder { get; set; }

        [InverseProperty(nameof(ParcelAllocationHistory.WaterType))]
        public virtual ICollection<ParcelAllocationHistory> ParcelAllocationHistories { get; set; }
        [InverseProperty(nameof(ParcelAllocation.WaterType))]
        public virtual ICollection<ParcelAllocation> ParcelAllocations { get; set; }
        [InverseProperty(nameof(ParcelLedger.WaterType))]
        public virtual ICollection<ParcelLedger> ParcelLedgers { get; set; }
    }
}
