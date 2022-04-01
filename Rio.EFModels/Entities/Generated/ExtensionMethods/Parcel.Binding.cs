//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[Parcel]
namespace Rio.EFModels.Entities
{
    public partial class Parcel
    {
        public ParcelStatus ParcelStatus => ParcelStatus.AllLookupDictionary[ParcelStatusID];
    }
}