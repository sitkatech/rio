﻿namespace Rio.API.Services
{
    public class RioConfiguration
    {
        public string KEYSTONE_HOST { get; set; }
        public string RIO_DB_CONNECTION_STRING { get; set; }
        public string SMTP_HOST { get; set; }
        public int SMTP_PORT { get; set; }
        public string SITKA_EMAIL_REDIRECT { get; set; }
        public string RIO_WEB_URL { get; set; }
    }
}