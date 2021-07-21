using Rio.Models.DataTransferObjects;

namespace Rio.EFModels.Entities
{
    public static class vOpenETMostRecentSyncHistoryForYearAndMonthExtensionMethods
    {
        public static OpenETSyncHistoryDto AsOpenETSyncHistoryDto(
            this vOpenETMostRecentSyncHistoryForYearAndMonth vOpenETMostRecentSyncHistoryForYearAndMonth)
        {
            return new OpenETSyncHistoryDto()
            {
                OpenETSyncHistoryID = vOpenETMostRecentSyncHistoryForYearAndMonth.OpenETSyncHistoryID,
                OpenETSyncResultType = vOpenETMostRecentSyncHistoryForYearAndMonth.OpenETSyncResultType.AsDto(),
                WaterYearMonth = vOpenETMostRecentSyncHistoryForYearAndMonth.WaterYearMonth.AsDto(),
                CreateDate = vOpenETMostRecentSyncHistoryForYearAndMonth.CreateDate,
                UpdateDate = vOpenETMostRecentSyncHistoryForYearAndMonth.UpdateDate,
                GoogleBucketFileRetrievalURL = vOpenETMostRecentSyncHistoryForYearAndMonth.GoogleBucketFileRetrievalURL,
                ErrorMessage = vOpenETMostRecentSyncHistoryForYearAndMonth.ErrorMessage
            };
        }
    }
}