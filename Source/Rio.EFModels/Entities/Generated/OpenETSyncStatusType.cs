using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rio.EFModels.Entities
{
    public partial class OpenETSyncStatusType
    {
        public OpenETSyncStatusType()
        {
            OpenETSyncWaterYearStatus = new HashSet<OpenETSyncWaterYearStatus>();
        }

        [Key]
        public int OpenETSyncStatusTypeID { get; set; }
        [Required]
        [StringLength(100)]
        public string OpenETSyncStatusTypeName { get; set; }
        [Required]
        [StringLength(100)]
        public string OpenETSyncStatusTypeDisplayName { get; set; }

        [InverseProperty("OpenETSyncStatusType")]
        public virtual ICollection<OpenETSyncWaterYearStatus> OpenETSyncWaterYearStatus { get; set; }
    }
}
