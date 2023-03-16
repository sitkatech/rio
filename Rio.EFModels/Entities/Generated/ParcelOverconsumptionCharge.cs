using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Rio.EFModels.Entities
{
    [Table("ParcelOverconsumptionCharge")]
    [Index("ParcelID", "WaterYearID", Name = "AK_ParcelOverconsumptionCharge_ParcelID_WaterYearID", IsUnique = true)]
    public partial class ParcelOverconsumptionCharge
    {
        [Key]
        public int ParcelOverconsumptionChargeID { get; set; }
        public int ParcelID { get; set; }
        public int WaterYearID { get; set; }
        [Column(TypeName = "decimal(10, 4)")]
        public decimal OverconsumptionAmount { get; set; }
        [Column(TypeName = "decimal(10, 4)")]
        public decimal OverconsumptionCharge { get; set; }

        [ForeignKey("ParcelID")]
        [InverseProperty("ParcelOverconsumptionCharges")]
        public virtual Parcel Parcel { get; set; }
        [ForeignKey("WaterYearID")]
        [InverseProperty("ParcelOverconsumptionCharges")]
        public virtual WaterYear WaterYear { get; set; }
    }
}
