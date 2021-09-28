using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rio.EFModels.Entities
{
    public partial class OpenETSyncWaterYearStatus
    {
        [Key]
        public int OpenETSyncWaterYearStatusID { get; set; }
        public int WaterYear { get; set; }
        public int OpenETSyncStatusTypeID { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? LastUpdatedDate { get; set; }

        [ForeignKey(nameof(OpenETSyncStatusTypeID))]
        [InverseProperty("OpenETSyncWaterYearStatus")]
        public virtual OpenETSyncStatusType OpenETSyncStatusType { get; set; }
    }
}
