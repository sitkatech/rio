using System;
using System.Collections.Generic;
using System.Text;

namespace Rio.Models.DataTransferObjects
{
    public class OpenETSyncWaterYearStatusDto
    {
        public int OpenETSyncWaterYearStatusID { get; set; }
        public int WaterYear { get; set; }
        public OpenETSyncStatusTypeDto OpenETSyncStatusType { get; set; }
        public DateTime? LastUpdatedDate { get; set; }
    }

    public class OpenETSyncStatusTypeDto
    {
        public int OpenETSyncStatusTypeID { get; set; }
        public string OpenETSyncStatusTypeName { get; set; }
        public string OpenETSyncStatusTypeDisplayName { get; set; }
    }
}
