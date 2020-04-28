using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rio.EFModels.Entities
{
    public partial class ReconciliationAllocationUploadStatus
    {
        public ReconciliationAllocationUploadStatus()
        {
            ReconciliationAllocationUpload = new HashSet<ReconciliationAllocationUpload>();
        }

        [Key]
        public int ReconciliationAllocationUploadStatusID { get; set; }
        [StringLength(20)]
        public string ReconciliationAllocationUploadStatusName { get; set; }
        [StringLength(22)]
        public string ReconciliationAllocationUploadStatusDisplayName { get; set; }

        [InverseProperty("ReconciliationAllocationUploadStatus")]
        public virtual ICollection<ReconciliationAllocationUpload> ReconciliationAllocationUpload { get; set; }
    }
}
