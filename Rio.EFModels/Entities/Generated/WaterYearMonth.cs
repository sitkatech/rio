using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Rio.EFModels.Entities
{
    [Table("WaterYearMonth")]
    [Index("WaterYearID", "Month", Name = "AK_WaterYearMonth_WaterYearID_Month", IsUnique = true)]
    public partial class WaterYearMonth
    {
        public WaterYearMonth()
        {
            OpenETSyncHistories = new HashSet<OpenETSyncHistory>();
        }

        [Key]
        public int WaterYearMonthID { get; set; }
        public int WaterYearID { get; set; }
        public int Month { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? FinalizeDate { get; set; }

        [ForeignKey("WaterYearID")]
        [InverseProperty("WaterYearMonths")]
        public virtual WaterYear WaterYear { get; set; }
        [InverseProperty("WaterYearMonth")]
        public virtual ICollection<OpenETSyncHistory> OpenETSyncHistories { get; set; }
    }
}
