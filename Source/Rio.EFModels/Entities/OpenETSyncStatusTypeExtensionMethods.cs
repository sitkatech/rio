using System;
using System.Collections.Generic;
using System.Text;
using Rio.Models.DataTransferObjects;

namespace Rio.EFModels.Entities
{
    public static class OpenETSyncStatusTypeExtensionMethods
    {
        public static OpenETSyncStatusTypeDto AsDto(this OpenETSyncStatusType openETSyncStatusType)
        {
            return new OpenETSyncStatusTypeDto()
            {
                OpenETSyncStatusTypeID = openETSyncStatusType.OpenETSyncStatusTypeID,
                OpenETSyncStatusTypeName = openETSyncStatusType.OpenETSyncStatusTypeName,
                OpenETSyncStatusTypeDisplayName = openETSyncStatusType.OpenETSyncStatusTypeDisplayName
            };
        }
    }
}