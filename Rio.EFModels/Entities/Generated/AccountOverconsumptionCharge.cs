using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Rio.EFModels.Entities
{
    [Table("AccountOverconsumptionCharge")]
    [Index("AccountID", "WaterYearID", Name = "AK_AccountOverconsumptionCharge_AccountID_WaterYearID", IsUnique = true)]
    public partial class AccountOverconsumptionCharge
    {
        [Key]
        public int AccountOverconsumptionChargeID { get; set; }
        public int AccountID { get; set; }
        public int WaterYearID { get; set; }
        [Column(TypeName = "decimal(12, 2)")]
        public decimal OverconsumptionAmount { get; set; }
        [Column(TypeName = "decimal(12, 2)")]
        public decimal OverconsumptionCharge { get; set; }

        [ForeignKey("AccountID")]
        [InverseProperty("AccountOverconsumptionCharges")]
        public virtual Account Account { get; set; }
        [ForeignKey("WaterYearID")]
        [InverseProperty("AccountOverconsumptionCharges")]
        public virtual WaterYear WaterYear { get; set; }
    }
}
