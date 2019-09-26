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
                DateRegistered = waterTransferRegistration.DateRegistered
            };
        }

        public static WaterTransferRegistration GetWaterTransferRegistrationByWaterTransferType(this WaterTransfer waterTransfer, WaterTransferTypeEnum waterTransferTypeEnum)
        {
            return waterTransfer.WaterTransferRegistration.Single(x => x.WaterTransferTypeID == (int)waterTransferTypeEnum);
        }
    }
}