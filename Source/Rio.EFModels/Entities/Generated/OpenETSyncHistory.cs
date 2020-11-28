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
        [Required]
        [StringLength(100)]
        public string YearsInUpdateSeparatedByComma { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime LastUpdatedDate { get; set; }

        [ForeignKey(nameof(OpenETSyncResultTypeID))]
        [InverseProperty("OpenETSyncHistory")]
        public virtual OpenETSyncResultType OpenETSyncResultType { get; set; }
    }
}
