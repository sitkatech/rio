//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[WaterTransferRegistration]
namespace Rio.EFModels.Entities
{
    public partial class WaterTransferRegistration
    {
        public WaterTransferType WaterTransferType => WaterTransferType.AllLookupDictionary[WaterTransferTypeID];
        public WaterTransferRegistrationStatus WaterTransferRegistrationStatus => WaterTransferRegistrationStatus.AllLookupDictionary[WaterTransferRegistrationStatusID];
    }
}