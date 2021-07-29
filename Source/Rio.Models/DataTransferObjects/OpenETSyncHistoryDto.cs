using System;
using System.Collections.Generic;
using System.Text;

namespace Rio.Models.DataTransferObjects
{
    public class OpenETSyncHistoryDto
    {
        public int OpenETSyncHistoryID { get; set; }
        public OpenETSyncResultTypeDto OpenETSyncResultType { get; set; }
        public WaterYearMonthDto WaterYearMonth { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public string GoogleBucketFileRetrievalURL { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class OpenETSyncResultTypeDto
    {
        public int OpenETSyncResultTypeID { get; set; }
        public string OpenETSyncResultTypeName { get; set; }
        public string OpenETSyncResultTypeDisplayName { get; set; }
    }
}
