//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[WaterTransferRegistrationParcel]
using System;


namespace Rio.Models.DataTransferObjects
{
    public partial class WaterTransferRegistrationParcelDto
    {
        public int WaterTransferRegistrationParcelID { get; set; }
        public WaterTransferRegistrationSimpleDto WaterTransferRegistration { get; set; }
        public ParcelDto Parcel { get; set; }
        public int AcreFeetTransferred { get; set; }
    }

    public partial class WaterTransferRegistrationParcelSimpleDto
    {
        public int WaterTransferRegistrationParcelID { get; set; }
        public int WaterTransferRegistrationID { get; set; }
        public int ParcelID { get; set; }
        public int AcreFeetTransferred { get; set; }
    }

}