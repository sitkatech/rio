using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Rio.EFModels.Entities
{
    [Table("OpenETSyncHistory")]
    public partial class OpenETSyncHistory
    {
        [Key]
        public int OpenETSyncHistoryID { get; set; }
        public int WaterYearMonthID { get; set; }
        public int OpenETSyncResultTypeID { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreateDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime UpdateDate { get; set; }
        [StringLength(200)]
        public string GoogleBucketFileRetrievalURL { get; set; }
        public string ErrorMessage { get; set; }

        [ForeignKey(nameof(OpenETSyncResultTypeID))]
        [InverseProperty("OpenETSyncHistories")]
        public virtual OpenETSyncResultType OpenETSyncResultType { get; set; }
        [ForeignKey(nameof(WaterYearMonthID))]
        [InverseProperty("OpenETSyncHistories")]
        public virtual WaterYearMonth WaterYearMonth { get; set; }
    }
}
