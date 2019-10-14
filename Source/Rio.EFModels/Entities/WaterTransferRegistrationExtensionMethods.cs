using System.Linq;
using Rio.Models.DataTransferObjects.WaterTransfer;

namespace Rio.EFModels.Entities
{
    public static class WaterTransferRegistrationExtensionMethods
    {
        public static WaterTransferRegistrationDto AsDto(this WaterTransferRegistration waterTransferRegistration)
        {
            return new WaterTransferRegistrationDto
            {
                UserID = waterTransferRegistration.UserID,
                WaterTransferTypeID = waterTransferRegistration.WaterTransferTypeID,
                StatusDate = waterTransferRegistration.StatusDate
            };
        }

        public static WaterTransferRegistrationSimpleDto AsSimpleDto(this WaterTransferRegistration waterTransferRegistration)
        {
            return new WaterTransferRegistrationSimpleDto
            {
                User = waterTransferRegistration.User.AsSimpleDto(),
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
            return waterTransfer.WaterTransferRegistration.Single(x => x.WaterTransferTypeID == (int)waterTransferTypeEnum);
        }
    }
}