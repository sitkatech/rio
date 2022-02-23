//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[WaterTransferRegistration]
using System;


namespace Rio.Models.DataTransferObjects
{
    public partial class WaterTransferRegistrationDto
    {
        public int WaterTransferRegistrationID { get; set; }
        public WaterTransferDto WaterTransfer { get; set; }
        public WaterTransferTypeDto WaterTransferType { get; set; }
        public AccountDto Account { get; set; }
        public WaterTransferRegistrationStatusDto WaterTransferRegistrationStatus { get; set; }
        public DateTime StatusDate { get; set; }
    }

    public partial class WaterTransferRegistrationSimpleDto
    {
        public int WaterTransferRegistrationID { get; set; }
        public int WaterTransferID { get; set; }
        public int WaterTransferTypeID { get; set; }
        public int AccountID { get; set; }
        public int WaterTransferRegistrationStatusID { get; set; }
        public DateTime StatusDate { get; set; }
    }

}