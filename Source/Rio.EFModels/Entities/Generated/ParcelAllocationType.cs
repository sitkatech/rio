using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rio.EFModels.Entities
{
    public partial class ParcelAllocationType
    {
        public ParcelAllocationType()
        {
            ParcelAllocation = new HashSet<ParcelAllocation>();
            ParcelAllocationHistory = new HashSet<ParcelAllocationHistory>();
        }

        [Key]
        public int ParcelAllocationTypeID { get; set; }
        [Required]
        [StringLength(50)]
        public string ParcelAllocationTypeName { get; set; }
        public bool IsAppliedProportionally { get; set; }
        public string ParcelAllocationTypeDefinition { get; set; }

        [InverseProperty("ParcelAllocationType")]
        public virtual ICollection<ParcelAllocation> ParcelAllocation { get; set; }
        [InverseProperty("ParcelAllocationType")]
        public virtual ICollection<ParcelAllocationHistory> ParcelAllocationHistory { get; set; }
    }
}
