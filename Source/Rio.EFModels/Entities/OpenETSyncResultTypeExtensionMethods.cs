using System;
using System.Collections.Generic;
using System.Text;
using Rio.Models.DataTransferObjects;

namespace Rio.EFModels.Entities
{
    public static class OpenETSyncResultTypeExtensionMethods
    {
        public static OpenETSyncResultTypeDto AsDto(this OpenETSyncResultType openETSyncResultType)
        {
            return new OpenETSyncResultTypeDto()
            {
                OpenETSyncResultTypeID = openETSyncResultType.OpenETSyncResultTypeID,
                OpenETSyncResultTypeName = openETSyncResultType.OpenETSyncResultTypeName,
                OpenETSyncResultTypeDisplayName = openETSyncResultType.OpenETSyncResultTypeDisplayName
            };
        }
    }
}
