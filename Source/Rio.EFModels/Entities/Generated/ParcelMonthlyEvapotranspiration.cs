using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rio.EFModels.Entities
{
    public partial class ParcelMonthlyEvapotranspiration
    {
        public int ParcelMonthlyEvapotranspirationID { get; set; }
        public int ParcelID { get; set; }
        public int WaterYear { get; set; }
        public int WaterMonth { get; set; }
        [Column(TypeName = "decimal(10, 4)")]
        public decimal EvapotranspirationRate { get; set; }

        [ForeignKey("ParcelID")]
        [InverseProperty("ParcelMonthlyEvapotranspiration")]
        public virtual Parcel Parcel { get; set; }
    }
}
