using System;
using System.Collections.Generic;
using System.Text;

namespace Rio.Models.DataTransferObjects
{
    public class OpenETSyncHistoryDto
    {
        public int OpenETSyncHistoryID { get; set; }
        public OpenETSyncResultTypeDto OpenETSyncResultType { get; set; }
        public string YearsInUpdateSeparatedByComma { get; set; }
        public string UpdatedFileSuffix { get; set; }
        public DateTime LastUpdatedDate { get; set; }
    }

    public class OpenETSyncResultTypeDto
    {
        public int OpenETSyncResultTypeID { get; set; }
        public string OpenETSyncResultTypeName { get; set; }
        public string OpenETSyncResultTypeDisplayName { get; set; }
    }
}
