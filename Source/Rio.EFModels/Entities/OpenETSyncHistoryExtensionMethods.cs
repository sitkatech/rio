﻿using System;
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
                WaterYearMonth = openETSyncHistory.WaterYearMonth.AsDto(),
                CreateDate = openETSyncHistory.CreateDate,
                UpdateDate = openETSyncHistory.UpdateDate,
                GoogleBucketFileRetrievalURL = openETSyncHistory.GoogleBucketFileRetrievalURL,
                ErrorMessage = openETSyncHistory.ErrorMessage
            };
        }
    }
}
