using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rio.EFModels.Entities
{
    public partial class ParcelMonthlyUsage
    {
        public int ParcelMonthlyUsageID { get; set; }
        public int ParcelID { get; set; }
        public int WaterYear { get; set; }
        public int WaterMonth { get; set; }
        [Column(TypeName = "decimal(10, 4)")]
        public decimal AcreFeetUsed { get; set; }

        [ForeignKey("ParcelID")]
        [InverseProperty("ParcelMonthlyUsage")]
        public virtual Parcel Parcel { get; set; }
    }
}
