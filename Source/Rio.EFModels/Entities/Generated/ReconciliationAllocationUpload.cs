using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rio.EFModels.Entities
{
    public partial class ReconciliationAllocationUpload
    {
        [Key]
        public int ReconciliationAllocationUploadID { get; set; }
        public int UploadUserID { get; set; }
        public int FileResourceID { get; set; }
        public int ReconciliationAllocationUploadStatusID { get; set; }

        [ForeignKey(nameof(FileResourceID))]
        [InverseProperty("ReconciliationAllocationUpload")]
        public virtual FileResource FileResource { get; set; }
        [ForeignKey(nameof(ReconciliationAllocationUploadStatusID))]
        [InverseProperty("ReconciliationAllocationUpload")]
        public virtual ReconciliationAllocationUploadStatus ReconciliationAllocationUploadStatus { get; set; }
        [ForeignKey(nameof(UploadUserID))]
        [InverseProperty(nameof(User.ReconciliationAllocationUpload))]
        public virtual User UploadUser { get; set; }
    }
}
