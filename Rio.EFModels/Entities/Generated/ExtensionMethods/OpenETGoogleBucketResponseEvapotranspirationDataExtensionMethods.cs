//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[OpenETGoogleBucketResponseEvapotranspirationData]

using Rio.Models.DataTransferObjects;

namespace Rio.EFModels.Entities
{
    public static partial class OpenETGoogleBucketResponseEvapotranspirationDataExtensionMethods
    {
        public static OpenETGoogleBucketResponseEvapotranspirationDataDto AsDto(this OpenETGoogleBucketResponseEvapotranspirationData openETGoogleBucketResponseEvapotranspirationData)
        {
            var openETGoogleBucketResponseEvapotranspirationDataDto = new OpenETGoogleBucketResponseEvapotranspirationDataDto()
            {
                OpenETGoogleBucketResponseEvapotranspirationDataID = openETGoogleBucketResponseEvapotranspirationData.OpenETGoogleBucketResponseEvapotranspirationDataID,
                ParcelNumber = openETGoogleBucketResponseEvapotranspirationData.ParcelNumber,
                WaterMonth = openETGoogleBucketResponseEvapotranspirationData.WaterMonth,
                WaterYear = openETGoogleBucketResponseEvapotranspirationData.WaterYear,
                EvapotranspirationRateInches = openETGoogleBucketResponseEvapotranspirationData.EvapotranspirationRateInches
            };
            DoCustomMappings(openETGoogleBucketResponseEvapotranspirationData, openETGoogleBucketResponseEvapotranspirationDataDto);
            return openETGoogleBucketResponseEvapotranspirationDataDto;
        }

        static partial void DoCustomMappings(OpenETGoogleBucketResponseEvapotranspirationData openETGoogleBucketResponseEvapotranspirationData, OpenETGoogleBucketResponseEvapotranspirationDataDto openETGoogleBucketResponseEvapotranspirationDataDto);

        public static OpenETGoogleBucketResponseEvapotranspirationDataSimpleDto AsSimpleDto(this OpenETGoogleBucketResponseEvapotranspirationData openETGoogleBucketResponseEvapotranspirationData)
        {
            var openETGoogleBucketResponseEvapotranspirationDataSimpleDto = new OpenETGoogleBucketResponseEvapotranspirationDataSimpleDto()
            {
                OpenETGoogleBucketResponseEvapotranspirationDataID = openETGoogleBucketResponseEvapotranspirationData.OpenETGoogleBucketResponseEvapotranspirationDataID,
                ParcelNumber = openETGoogleBucketResponseEvapotranspirationData.ParcelNumber,
                WaterMonth = openETGoogleBucketResponseEvapotranspirationData.WaterMonth,
                WaterYear = openETGoogleBucketResponseEvapotranspirationData.WaterYear,
                EvapotranspirationRateInches = openETGoogleBucketResponseEvapotranspirationData.EvapotranspirationRateInches
            };
            DoCustomSimpleDtoMappings(openETGoogleBucketResponseEvapotranspirationData, openETGoogleBucketResponseEvapotranspirationDataSimpleDto);
            return openETGoogleBucketResponseEvapotranspirationDataSimpleDto;
        }

        static partial void DoCustomSimpleDtoMappings(OpenETGoogleBucketResponseEvapotranspirationData openETGoogleBucketResponseEvapotranspirationData, OpenETGoogleBucketResponseEvapotranspirationDataSimpleDto openETGoogleBucketResponseEvapotranspirationDataSimpleDto);
    }
}