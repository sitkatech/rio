using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rio.EFModels.Entities
{
    public partial class AccountParcelWaterYear
    {
        [Key]
        public int AccountParcelWaterYearID { get; set; }
        public int AccountID { get; set; }
        public int ParcelID { get; set; }
        public int WaterYearID { get; set; }

        [ForeignKey(nameof(AccountID))]
        [InverseProperty("AccountParcelWaterYear")]
        public virtual Account Account { get; set; }
        [ForeignKey(nameof(ParcelID))]
        [InverseProperty("AccountParcelWaterYear")]
        public virtual Parcel Parcel { get; set; }
        [ForeignKey(nameof(WaterYearID))]
        [InverseProperty("AccountParcelWaterYear")]
        public virtual WaterYear WaterYear { get; set; }
    }
}
