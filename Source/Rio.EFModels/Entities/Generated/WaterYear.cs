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
            OpenETSyncHistory = new HashSet<OpenETSyncHistory>();
        }

        [Key]
        public int WaterYearID { get; set; }
        public int Year { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? FinalizeDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? ParcelLayerUpdateDate { get; set; }

        [InverseProperty("WaterYear")]
        public virtual ICollection<OpenETSyncHistory> OpenETSyncHistory { get; set; }
    }
}
