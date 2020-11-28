using System;
using System.Collections.Generic;
using System.Text;
using Rio.Models.DataTransferObjects;

namespace Rio.EFModels.Entities
{
    public static class OpenETSyncWaterYearStatusExtensionMethods
    { 
        public static OpenETSyncWaterYearStatusDto AsDto(this OpenETSyncWaterYearStatus openETSyncWaterYearStatus)
        {
            return new OpenETSyncWaterYearStatusDto()
            {
                OpenETSyncWaterYearStatusID = openETSyncWaterYearStatus.OpenETSyncWaterYearStatusID,
                WaterYear = openETSyncWaterYearStatus.WaterYear,
                OpenETSyncStatusType = openETSyncWaterYearStatus.OpenETSyncStatusType.AsDto(),
                LastUpdatedDate = openETSyncWaterYearStatus.LastUpdatedDate
            };
        }
    }
}
