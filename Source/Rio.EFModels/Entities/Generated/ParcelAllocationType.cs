﻿using System;
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
        }

        [Key]
        public int ParcelAllocationTypeID { get; set; }
        [Required]
        [StringLength(50)]
        public string ParcelAllocationTypeName { get; set; }
        [Required]
        [StringLength(50)]
        public string ParcelAllocationTypeDisplayName { get; set; }

        [InverseProperty("ParcelAllocationType")]
        public virtual ICollection<ParcelAllocation> ParcelAllocation { get; set; }
    }
}