using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

namespace Rio.Web
{
    public class Startup
    {
        private readonly IWebHostEnvironment _environment;
        public IConfiguration Configuration { get; set; }

        public Startup(IWebHostEnvironment environment)
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            var builder = new ConfigurationBuilder()
                .SetBasePath(currentDirectory)
                .AddEnvironmentVariables();

            Configuration = builder.Build();

            _environment = environment;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory, IHostApplicationLifetime applicationLifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                var options = new RewriteOptions().AddRedirectToHttps(301, 9001);
                app.UseRewriter(options);
            }
            
            app.Use(async (context, next) =>
            {
                if (context.Request.Path.Value == "/assets/config.json")
                {
                    var result = new ConfigDto(Configuration);
                    var json = JsonConvert.SerializeObject(result);
                    await context.Response.WriteAsync(json);
                    return;
                }

                await next();

                if (context.Response.StatusCode == 404 && !Path.HasExtension(context.Request.Path.Value))
                {
                    context.Request.Path = "/index.html";
                    await next();
                }
            });

            app.UseDefaultFiles();
            app.UseStaticFiles();
        }
    }

    public class ConfigDto
    {
        public ConfigDto(IConfiguration configuration)
        {
            Production = bool.Parse(configuration["Production"]);
            Staging = bool.Parse(configuration["Staging"]);
            Dev = bool.Parse(configuration["Dev"]);
            ApiHostName = configuration["ApiHostName"];
            CreateAccountUrl = configuration["CreateAccountUrl"];
            CreateAccountRedirectUrl = configuration["CreateAccountRedirectUrl"];
            AllowTrading = bool.Parse(configuration["AllowTrading"]);
            KeystoneSupportBaseUrl = configuration["KeystoneSupportBaseUrl"];
            GeoserverMapServiceUrl = configuration["GeoserverMapServiceUrl"];
            KeystoneAuthConfiguration = new KeystoneAuthConfigurationDto(configuration);
            PlatformLongName = configuration["PlatformLongName"];
            PlatformShortName = configuration["PlatformShortName"];
            LeadOrganizationShortName = configuration["LeadOrganizationShortName"];
            LeadOrganizationLongName = configuration["LeadOrganizationLongName"];
            LeadOrganizationHomeUrl = configuration["LeadOrganizationHomeUrl"];
            FaviconFilename = configuration["FaviconFilename"];
            LeadOrganizationLogoFilename = configuration["LeadOrganizationLogoFilename"];
            EnabledGETIntegration = bool.Parse(configuration["EnabledGETIntegration"]);
            ContactInfoPhone = configuration["ContactInfoPhone"];
            ContactInfoEmail = configuration["ContactInfoEmail"];
            ContactInfoMailingAddress = configuration["ContactInfoMailingAddress"];
            ContactInfoPhysicalAddress = configuration["ContactInfoPhysicalAddress"];
            NavThemeColor = configuration["NavThemeColor"];
            ApplicationType = configuration["ApplicationType"];
        }

        [JsonProperty("production")]
        public bool Production { get; set; }
        [JsonProperty("staging")]
        public bool Staging { get; set; }
        [JsonProperty("dev")]
        public bool Dev { get; set; }
        [JsonProperty("apiHostName")]
        public string ApiHostName { get; set; }
        [JsonProperty("createAccountUrl")]
        public string CreateAccountUrl { get; set; }
        [JsonProperty("createAccountRedirectUrl")]
        public string CreateAccountRedirectUrl { get; set; }
        [JsonProperty("allowTrading")]
        public bool AllowTrading { get; set; }
        [JsonProperty("keystoneSupportBaseUrl")]
        public string KeystoneSupportBaseUrl { get; set; }
        [JsonProperty("geoserverMapServiceUrl")]
        public string GeoserverMapServiceUrl { get; set; }
        [JsonProperty("keystoneAuthConfiguration")]
        public KeystoneAuthConfigurationDto KeystoneAuthConfiguration { get; set; }
        [JsonProperty("platformLongName")]
        public string PlatformLongName { get; set; }
        [JsonProperty("platformShortName")]
        public string PlatformShortName { get; set; }
        [JsonProperty("leadOrganizationShortName")]
        public string LeadOrganizationShortName { get; set; }
        [JsonProperty("leadOrganizationLongName")]
        public string LeadOrganizationLongName { get; set; }
        [JsonProperty("leadOrganizationHomeUrl")]
        public string LeadOrganizationHomeUrl { get; set; }
        [JsonProperty("faviconFilename")]
        public string FaviconFilename {get; set;}
        [JsonProperty("leadOrganizationLogoFilename")]
        public string LeadOrganizationLogoFilename { get; set;}
        [JsonProperty("enabledGETIntegration")]
        public bool EnabledGETIntegration { get; set;}
        [JsonProperty("contactInfoPhone")]
        public string ContactInfoPhone { get; set;}
        [JsonProperty("contactInfoEmail")]
        public string ContactInfoEmail { get; set;}
        [JsonProperty("contactInfoMailingAddress")]
        public string ContactInfoMailingAddress { get; set;}
        [JsonProperty("contactInfoPhysicalAddress")]
        public string ContactInfoPhysicalAddress { get; set;}
        [JsonProperty("navThemeColor")]
        public string NavThemeColor { get; set;}
        [JsonProperty("applicationType")]
        public string ApplicationType { get; set;}
    }

    public class KeystoneAuthConfigurationDto
    {
        public KeystoneAuthConfigurationDto(IConfiguration configuration)
        {
            ClientID = configuration["Keystone_ClientID"];
            Issuer = configuration["Keystone_Issuer"];
            RedirectUriRelative = configuration["Keystone_RedirectUriRelative"];
            Scope = configuration["Keystone_Scope"];
            SessionChecksEnabled = bool.Parse(configuration["Keystone_SessionCheckEnabled"]);
            LogoutUrl = configuration["Keystone_LogoutUrl"];
            PostLogoutRedirectUri = configuration["Keystone_PostLogoutRedirectUri"];
            WaitForTokenInMsec = int.Parse(configuration["WaitForTokenInMsec"]);
        }

        [JsonProperty("clientId")]
        public string ClientID { get; set; }
        [JsonProperty("issuer")]
        public string Issuer { get; set; }
        [JsonProperty("redirectUriRelative")]
        public string RedirectUriRelative { get; set; }
        [JsonProperty("scope")]
        public string Scope { get; set; }
        [JsonProperty("sessionChecksEnabled")]
        public bool SessionChecksEnabled { get; set; }
        [JsonProperty("logoutUrl")]
        public string LogoutUrl { get; set; }
        [JsonProperty("postLogoutRedirectUri")]
        public string PostLogoutRedirectUri { get; set; }
        [JsonProperty("waitForTokenInMsec")]
        public int WaitForTokenInMsec { get; set; }
    }
}
