namespace Rio.API.Services
{
    public class RioConfiguration
    {
        public string KEYSTONE_HOST { get; set; }
        public string DB_CONNECTION_STRING { get; set; }
        public string SMTP_HOST { get; set; }
        public int SMTP_PORT { get; set; }
        public string SITKA_EMAIL_REDIRECT { get; set; }
        public string WEB_URL { get; set; }
        public string KEYSTONE_REDIRECT_URL { get; set; }
        public bool ALLOW_TRADING { get; set; }
        public string PlatformLongName { get; set; }
        public string PlatformShortName { get; set; }
        public string LeadOrganizationLongName { get; set; }
        public string LeadOrganizationShortName { get; set; }
        public string LeadOrganizationHomeUrl { get; set; }
        public string LeadOrganizationEmail { get; set; }
        public string HangfireUserName { get; set; }
        public string HangfirePassword { get; set; }
        public string CimisAppKey { get; set; }
        public string VerificationKeyChars { get; set; }
        public string OpenETAPIKey { get; set; }
    }
}