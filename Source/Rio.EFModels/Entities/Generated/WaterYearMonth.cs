using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rio.EFModels.Entities
{
    public partial class WaterYearMonth
    {
        public WaterYearMonth()
        {
            OpenETSyncHistory = new HashSet<OpenETSyncHistory>();
        }

        [Key]
        public int WaterYearMonthID { get; set; }
        public int WaterYearID { get; set; }
        public int Month { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? FinalizeDate { get; set; }

        [ForeignKey(nameof(WaterYearID))]
        [InverseProperty("WaterYearMonth")]
        public virtual WaterYear WaterYear { get; set; }
        [InverseProperty("WaterYearMonth")]
        public virtual ICollection<OpenETSyncHistory> OpenETSyncHistory { get; set; }
    }
}
