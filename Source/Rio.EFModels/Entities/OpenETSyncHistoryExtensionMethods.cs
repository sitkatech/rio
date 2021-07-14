using System;
using System.Collections.Generic;
using System.Text;
using Rio.Models.DataTransferObjects;

namespace Rio.EFModels.Entities
{
    public static class OpenETSyncHistoryExtensionMethods
    {
        public static OpenETSyncHistoryDto AsDto(this OpenETSyncHistory openETSyncHistory)
        {
            return new OpenETSyncHistoryDto()
            {
                OpenETSyncHistoryID = openETSyncHistory.OpenETSyncHistoryID,
                OpenETSyncResultType = openETSyncHistory.OpenETSyncResultType.AsDto(),
                WaterYear = openETSyncHistory.WaterYear.AsDto(),
                CreateDate = openETSyncHistory.CreateDate,
                UpdateDate = openETSyncHistory.UpdateDate,
                GoogleBucketFileSuffixForRetrieval = openETSyncHistory.GoogleBucketFileSuffixForRetrieval,
                TrackingNumber = openETSyncHistory.TrackingNumber
            };
        }
    }
}
