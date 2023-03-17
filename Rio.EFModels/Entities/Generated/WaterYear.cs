using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Rio.EFModels.Entities
{
    [Table("WaterYear")]
    [Index("Year", Name = "AK_WaterYear_Year", IsUnique = true)]
    public partial class WaterYear
    {
        public WaterYear()
        {
            AccountOverconsumptionCharges = new HashSet<AccountOverconsumptionCharge>();
            AccountParcelWaterYears = new HashSet<AccountParcelWaterYear>();
            WaterYearMonths = new HashSet<WaterYearMonth>();
        }

        [Key]
        public int WaterYearID { get; set; }
        public int Year { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? ParcelLayerUpdateDate { get; set; }
        [Column(TypeName = "decimal(10, 4)")]
        public decimal OverconsumptionRate { get; set; }

        [InverseProperty("WaterYear")]
        public virtual ICollection<AccountOverconsumptionCharge> AccountOverconsumptionCharges { get; set; }
        [InverseProperty("WaterYear")]
        public virtual ICollection<AccountParcelWaterYear> AccountParcelWaterYears { get; set; }
        [InverseProperty("WaterYear")]
        public virtual ICollection<WaterYearMonth> WaterYearMonths { get; set; }
    }
}
