﻿namespace Rio.API.Services
{
    public class RioConfiguration
    {
        public string KEYSTONE_HOST { get; set; }
        public string DB_CONNECTION_STRING { get; set; }
        public string SITKA_EMAIL_REDIRECT { get; set; }
        public string WEB_URL { get; set; }
        public string KEYSTONE_REDIRECT_URL { get; set; }
        public bool AllowTrading { get; set; }
        public bool IncludeWaterSupply { get; set; }
        public string PlatformLongName { get; set; }
        public string PlatformShortName { get; set; }
        public string LeadOrganizationLongName { get; set; }
        public string LeadOrganizationShortName { get; set; }
        public string LeadOrganizationHomeUrl { get; set; }
        public string LeadOrganizationEmail { get; set; }
        public string VerificationKeyChars { get; set; }
        public string ValidParcelNumberRegexPattern { get; set; }
        public string ValidParcelNumberPatternAsStringForDisplay { get; set; }
        public string OpenETAPIKey { get; set; }
        public string OpenETShapefilePath { get; set; }
        public string OpenETAPIBaseUrl { get; set; }
        public string OpenETRasterTimeSeriesMultipolygonRoute { get; set; }
        public string OpenETRasterMetadataRoute { get; set; }
        public string OpenETAllFilesReadyForExportRoute { get; set; }
        public string ParcelBoundingBoxLeft {get; set;}
        public string ParcelBoundingBoxRight { get; set; }
        public string ParcelBoundingBoxTop { get; set; }
        public string ParcelBoundingBoxBottom { get; set; }
        public string OpenETRasterTimeseriesMultipolygonColumnToUseAsIdentifier { get; set; }
        public bool AllowOpenETSync { get; set; }
        public string Ogr2OgrExecutable { get; set; }
        public string OgrInfoExecutable { get; set; }
        public string SendGridApiKey { get; set; }
        public string HostName { get; set; }
    }
}