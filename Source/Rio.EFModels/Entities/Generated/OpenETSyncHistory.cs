using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rio.EFModels.Entities
{
    public partial class OpenETSyncHistory
    {
        [Key]
        public int OpenETSyncHistoryID { get; set; }
        public int OpenETSyncResultTypeID { get; set; }
        public int WaterYearID { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreateDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime UpdateDate { get; set; }
        [StringLength(50)]
        public string GoogleBucketFileSuffixForRetrieval { get; set; }
        [StringLength(100)]
        public string TrackingNumber { get; set; }

        [ForeignKey(nameof(OpenETSyncResultTypeID))]
        [InverseProperty("OpenETSyncHistory")]
        public virtual OpenETSyncResultType OpenETSyncResultType { get; set; }
        [ForeignKey(nameof(WaterYearID))]
        [InverseProperty("OpenETSyncHistory")]
        public virtual WaterYear WaterYear { get; set; }
    }
}
