using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rio.EFModels.Entities
{
    public partial class vOpenETMostRecentSyncHistoryForYearAndMonth
    {
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
    }
}
