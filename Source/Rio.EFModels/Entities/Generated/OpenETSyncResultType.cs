using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Rio.EFModels.Entities
{
    [Table("OpenETSyncResultType")]
    [Index(nameof(OpenETSyncResultTypeName), Name = "AK_OpenETSyncResultType_AK_OpenETSyncResultTypeName", IsUnique = true)]
    [Index(nameof(OpenETSyncResultTypeDisplayName), Name = "AK_OpenETSyncResultType_OpenETSyncResultTypeDisplayName", IsUnique = true)]
    public partial class OpenETSyncResultType
    {
        public OpenETSyncResultType()
        {
            OpenETSyncHistories = new HashSet<OpenETSyncHistory>();
        }

        [Key]
        public int OpenETSyncResultTypeID { get; set; }
        [Required]
        [StringLength(100)]
        public string OpenETSyncResultTypeName { get; set; }
        [Required]
        [StringLength(100)]
        public string OpenETSyncResultTypeDisplayName { get; set; }

        [InverseProperty(nameof(OpenETSyncHistory.OpenETSyncResultType))]
        public virtual ICollection<OpenETSyncHistory> OpenETSyncHistories { get; set; }
    }
}
