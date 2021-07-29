using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Rio.EFModels.Entities
{
    [Table("AccountParcelWaterYear")]
    [Index(nameof(ParcelID), nameof(WaterYearID), Name = "AK_AccountParcelWaterYear_ParcelID_WaterYearID", IsUnique = true)]
    public partial class AccountParcelWaterYear
    {
        [Key]
        public int AccountParcelWaterYearID { get; set; }
        public int AccountID { get; set; }
        public int ParcelID { get; set; }
        public int WaterYearID { get; set; }

        [ForeignKey(nameof(AccountID))]
        [InverseProperty("AccountParcelWaterYears")]
        public virtual Account Account { get; set; }
        [ForeignKey(nameof(ParcelID))]
        [InverseProperty("AccountParcelWaterYears")]
        public virtual Parcel Parcel { get; set; }
        [ForeignKey(nameof(WaterYearID))]
        [InverseProperty("AccountParcelWaterYears")]
        public virtual WaterYear WaterYear { get; set; }
    }
}
