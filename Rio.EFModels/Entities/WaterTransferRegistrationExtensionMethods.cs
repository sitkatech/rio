using System.Linq;
using Rio.Models.DataTransferObjects;

namespace Rio.EFModels.Entities
{
    public static partial class WaterTransferRegistrationExtensionMethods
    {
        static partial void DoCustomMappings(WaterTransferRegistration waterTransferRegistration,
            WaterTransferRegistrationDto waterTransferRegistrationDto)
        {
            waterTransferRegistrationDto.IsRegistered = waterTransferRegistration.IsRegistered;
            waterTransferRegistrationDto.IsCanceled = waterTransferRegistration.IsCanceled;
            waterTransferRegistrationDto.IsPending = waterTransferRegistration.IsPending;
        }
        static partial void DoCustomSimpleDtoMappings(WaterTransferRegistration waterTransferRegistration,
            WaterTransferRegistrationSimpleDto waterTransferRegistrationSimpleDto)
        {
            waterTransferRegistrationSimpleDto.IsRegistered = waterTransferRegistration.IsRegistered;
            waterTransferRegistrationSimpleDto.IsCanceled = waterTransferRegistration.IsCanceled;
            waterTransferRegistrationSimpleDto.IsPending = waterTransferRegistration.IsPending;
            waterTransferRegistrationSimpleDto.Account = waterTransferRegistration.Account.AsSimpleDto();
        }

        public static WaterTransferRegistration GetWaterTransferRegistrationByWaterTransferType(this WaterTransfer waterTransfer, WaterTransferTypeEnum waterTransferTypeEnum)
        {
            return waterTransfer.WaterTransferRegistrations.Single(x => x.WaterTransferTypeID == (int)waterTransferTypeEnum);
        }
    }
}