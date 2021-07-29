using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Rio.EFModels.Entities
{
    [Table("ParcelMonthlyEvapotranspiration")]
    [Index(nameof(ParcelID), nameof(WaterYear), nameof(WaterMonth), Name = "AK_ParcelMonthlyEvapotranspiration_ParcelID_WaterYear_WaterMonth", IsUnique = true)]
    public partial class ParcelMonthlyEvapotranspiration
    {
        [Key]
        public int ParcelMonthlyEvapotranspirationID { get; set; }
        public int ParcelID { get; set; }
        public int WaterYear { get; set; }
        public int WaterMonth { get; set; }
        [Column(TypeName = "decimal(10, 4)")]
        public decimal? EvapotranspirationRate { get; set; }
        [Column(TypeName = "decimal(10, 4)")]
        public decimal? OverriddenEvapotranspirationRate { get; set; }

        [ForeignKey(nameof(ParcelID))]
        [InverseProperty("ParcelMonthlyEvapotranspirations")]
        public virtual Parcel Parcel { get; set; }
    }
}
