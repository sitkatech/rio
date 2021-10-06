using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Rio.EFModels.Entities
{
    [Table("ParcelAllocationType")]
    [Index(nameof(ParcelAllocationTypeName), Name = "AK_ParcelAllocationType_ParcelAllocationTypeName", IsUnique = true)]
    public partial class ParcelAllocationType
    {
        public ParcelAllocationType()
        {
            ParcelAllocationHistories = new HashSet<ParcelAllocationHistory>();
            ParcelAllocations = new HashSet<ParcelAllocation>();
            ParcelLedgers = new HashSet<ParcelLedger>();
        }

        [Key]
        public int ParcelAllocationTypeID { get; set; }
        [Required]
        [StringLength(50)]
        public string ParcelAllocationTypeName { get; set; }
        public bool IsAppliedProportionally { get; set; }
        public string ParcelAllocationTypeDefinition { get; set; }
        public bool IsSourcedFromApi { get; set; }
        public int SortOrder { get; set; }

        [InverseProperty(nameof(ParcelAllocationHistory.ParcelAllocationType))]
        public virtual ICollection<ParcelAllocationHistory> ParcelAllocationHistories { get; set; }
        [InverseProperty(nameof(ParcelAllocation.ParcelAllocationType))]
        public virtual ICollection<ParcelAllocation> ParcelAllocations { get; set; }
        [InverseProperty(nameof(ParcelLedger.WaterType))]
        public virtual ICollection<ParcelLedger> ParcelLedgers { get; set; }
    }
}
