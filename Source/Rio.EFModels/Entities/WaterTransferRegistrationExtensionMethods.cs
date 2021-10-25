using Rio.Models.DataTransferObjects.WaterTransfer;
using System.Linq;

namespace Rio.EFModels.Entities
{
    public static class WaterTransferRegistrationExtensionMethods
    {
        public static WaterTransferRegistrationSimpleDto AsSimpleDto(this WaterTransferRegistration waterTransferRegistration)
        {
            return new WaterTransferRegistrationSimpleDto
            {
                WaterTransferRegistrationID = waterTransferRegistration.WaterTransferRegistrationID,
                Account = waterTransferRegistration.Account.AsDto(),
                WaterTransferTypeID = waterTransferRegistration.WaterTransferTypeID,
                WaterTransferRegistrationStatusID = waterTransferRegistration.WaterTransferRegistrationStatusID,
                StatusDate = waterTransferRegistration.StatusDate,
                IsRegistered = waterTransferRegistration.IsRegistered,
                IsCanceled = waterTransferRegistration.IsCanceled,
                IsPending = waterTransferRegistration.IsPending
            };
        }

        public static WaterTransferRegistration GetWaterTransferRegistrationByWaterTransferType(this WaterTransfer waterTransfer, WaterTransferTypeEnum waterTransferTypeEnum)
        {
            return waterTransfer.WaterTransferRegistrations.Single(x => x.WaterTransferTypeID == (int)waterTransferTypeEnum);
        }
    }
}