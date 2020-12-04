using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rio.EFModels.Entities
{
    public partial class OpenETSyncResultType
    {
        public OpenETSyncResultType()
        {
            OpenETSyncHistory = new HashSet<OpenETSyncHistory>();
        }

        [Key]
        public int OpenETSyncResultTypeID { get; set; }
        [Required]
        [StringLength(100)]
        public string OpenETSyncResultTypeName { get; set; }
        [Required]
        [StringLength(100)]
        public string OpenETSyncResultTypeDisplayName { get; set; }

        [InverseProperty("OpenETSyncResultType")]
        public virtual ICollection<OpenETSyncHistory> OpenETSyncHistory { get; set; }
    }
}
