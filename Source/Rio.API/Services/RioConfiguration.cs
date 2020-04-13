namespace Rio.API.Services
{
    public class RioConfiguration
    {
        public string KEYSTONE_HOST { get; set; }
        public string RIO_DB_CONNECTION_STRING { get; set; }
        public string SMTP_HOST { get; set; }
        public int SMTP_PORT { get; set; }
        public string SITKA_EMAIL_REDIRECT { get; set; }
        public string RIO_WEB_URL { get; set; }
        public string KEYSTONE_REDIRECT_URL { get; set; }
        public bool ALLOW_TRADING { get; set; }
        public string PlatformLongName { get; set; }
        public string PlatformShortName { get; set; }
        public string LeadOrganizationLongName { get; set; }
        public string LeadOrganizationShortName { get; set; }
        public string LeadOrganizationHomeUrl { get; set; }
        public string LeadOrganizationEmail { get; set; }
    }
}