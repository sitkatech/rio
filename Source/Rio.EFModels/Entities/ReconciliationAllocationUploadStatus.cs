using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rio.EFModels.Entities
{
    public partial class ReconciliationAllocationUploadStatus
    {
        public static ReconciliationAllocationUploadStatus 
            GetReconciliationAllocationUploadStatusByEnum(RioDbContext dbContext, ReconciliationAllocationUploadStatusEnum reconciliationAllocationUploadStatusEnum)
        {
            return dbContext.ReconciliationAllocationUploadStatus.Single(x => x.ReconciliationAllocationUploadStatusID ==
                                                                              (int) reconciliationAllocationUploadStatusEnum);
        }
    }

    public enum ReconciliationAllocationUploadStatusEnum
    {
        Pending = 1,
        Accepted = 2,
        Canceled = 3,
        Rejected = 4
    }
}
