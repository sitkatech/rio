//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[OpenETGoogleBucketResponseEvapotranspirationData]
using System;


namespace Rio.Models.DataTransferObjects
{
    public partial class OpenETGoogleBucketResponseEvapotranspirationDataDto
    {
        public int OpenETGoogleBucketResponseEvapotranspirationDataID { get; set; }
        public string ParcelNumber { get; set; }
        public int WaterMonth { get; set; }
        public int WaterYear { get; set; }
        public decimal? EvapotranspirationRateInches { get; set; }
    }

    public partial class OpenETGoogleBucketResponseEvapotranspirationDataSimpleDto
    {
        public int OpenETGoogleBucketResponseEvapotranspirationDataID { get; set; }
        public string ParcelNumber { get; set; }
        public int WaterMonth { get; set; }
        public int WaterYear { get; set; }
        public decimal? EvapotranspirationRateInches { get; set; }
    }

}