using System;
using System.Collections.Generic;
using System.Text;

namespace Rio.EFModels.Entities
{
    public partial class OpenETSyncResultType
    {
    }

    public enum OpenETSyncResultTypeEnum
    {
        InProgress = 1,
        Succeeded = 2,
        Failed = 3,
        NoNewData = 4,
        DataNotAvailable = 5,
        Created = 6
    }
}
