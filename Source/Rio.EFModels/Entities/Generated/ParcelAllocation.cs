﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rio.EFModels.Entities
{
    public partial class ParcelAllocation
    {
        [Key]
        public int ParcelAllocationID { get; set; }
        public int ParcelID { get; set; }
        public int WaterYear { get; set; }
        public int ParcelAllocationTypeID { get; set; }
        [Column(TypeName = "decimal(10, 4)")]
        public decimal AcreFeetAllocated { get; set; }

        [ForeignKey(nameof(ParcelID))]
        [InverseProperty("ParcelAllocation")]
        public virtual Parcel Parcel { get; set; }
        [ForeignKey(nameof(ParcelAllocationTypeID))]
        [InverseProperty("ParcelAllocation")]
        public virtual ParcelAllocationType ParcelAllocationType { get; set; }
    }
}
