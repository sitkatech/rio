using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rio.EFModels.Entities
{
    public partial class WaterYear
    {
        public WaterYear()
        {
            AccountParcelWaterYear = new HashSet<AccountParcelWaterYear>();
            WaterYearMonth = new HashSet<WaterYearMonth>();
        }

        [Key]
        public int WaterYearID { get; set; }
        public int Year { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? ParcelLayerUpdateDate { get; set; }

        [InverseProperty("WaterYear")]
        public virtual ICollection<AccountParcelWaterYear> AccountParcelWaterYear { get; set; }
        [InverseProperty("WaterYear")]
        public virtual ICollection<WaterYearMonth> WaterYearMonth { get; set; }
    }
}
