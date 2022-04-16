using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.ResponseCompression;
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
             // Adding response compression which greatly reduces size of static content delivery.
            // https://docs.microsoft.com/en-us/aspnet/core/performance/response-compression?view=aspnetcore-5.0
            services.AddResponseCompression(options =>
            {
                options.Providers.Add<BrotliCompressionProvider>();
                options.Providers.Add<GzipCompressionProvider>();
                // Appending any extra MIME types to compress.
                // Default list is here: https://docs.microsoft.com/en-us/aspnet/core/performance/response-compression?view=aspnetcore-5.0#mime-types-1
                // NOTE: It's not recommended to compress images or other binary files
                options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[]
                {
                    "image/svg+xml"
                });
                options.EnableForHttps = true; 
            });        
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

            app.UseResponseCompression();
            app.UseDefaultFiles();
            app.UseStaticFiles();
        }
    }

    public class ConfigDto
    {
        public ConfigDto(IConfiguration configuration)
        {
            MainAppApiUrl = configuration["MainAppApiUrl"];
            CreateAccountRedirectUrl = configuration["CreateAccountRedirectUrl"];
            AllowTrading = bool.Parse(configuration["AllowTrading"]);
            IncludeWaterSupply = bool.Parse(configuration["IncludeWaterSupply"]);
            GeoserverMapServiceUrl = configuration["GeoserverMapServiceUrl"];
            KeystoneAuthConfiguration = new KeystoneAuthConfigurationDto(configuration);
            PlatformLongName = configuration["PlatformLongName"];
            PlatformShortName = configuration["PlatformShortName"];
            LeadOrganizationShortName = configuration["LeadOrganizationShortName"];
            LeadOrganizationLongName = configuration["LeadOrganizationLongName"];
            LeadOrganizationHomeUrl = configuration["LeadOrganizationHomeUrl"];
            FaviconFilename = configuration["FaviconFilename"];
            HomepageBannerFilename = configuration["homepageBannerFilename"];
            LeadOrganizationLogoFilename = configuration["LeadOrganizationLogoFilename"];
            EnabledGETIntegration = bool.Parse(configuration["EnabledGETIntegration"]);
            NavThemeColor = configuration["NavThemeColor"];
            ApplicationInternalName = configuration["ApplicationInternalName"];
            AllowOpenETSync = bool.Parse(configuration["AllowOpenETSync"]);
            AppInsightsInstrumentationKey =  configuration["AppInsightsInstrumentationKey"];
            ParcelBoundingBoxLeft =  configuration["ParcelBoundingBoxRight"];
            ParcelBoundingBoxRight =  configuration["ParcelBoundingBoxRight"];
            ParcelBoundingBoxTop =  configuration["ParcelBoundingBoxRight"];
            ParcelBoundingBoxBottom =  configuration["ParcelBoundingBoxRight"];
        }

        [JsonProperty("mainAppApiUrl")]
        public string MainAppApiUrl { get; set; }
        [JsonProperty("createAccountRedirectUrl")]
        public string CreateAccountRedirectUrl { get; set; }
        [JsonProperty("allowTrading")]
        public bool AllowTrading { get; set; }
        [JsonProperty("allowOpenETSync")]
        public bool AllowOpenETSync {get; set;}
        [JsonProperty("includeWaterSupply")]
        public bool IncludeWaterSupply { get; set; }
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
        [JsonProperty("homepageBannerFilename")]
        public string HomepageBannerFilename {get; set;}
        [JsonProperty("leadOrganizationLogoFilename")]
        public string LeadOrganizationLogoFilename { get; set;}
        [JsonProperty("enabledGETIntegration")]
        public bool EnabledGETIntegration { get; set;}
        [JsonProperty("navThemeColor")]
        public string NavThemeColor { get; set;}
        [JsonProperty("applicationInternalName")]
        public string ApplicationInternalName { get; set;}
        [JsonProperty("appInsightsInstrumentationKey")]
        public string AppInsightsInstrumentationKey {get; set;}
        [JsonProperty("parcelBoundingBoxLeft")]
        public string ParcelBoundingBoxLeft {get; set;}
        [JsonProperty("parcelBoundingBoxRight")]
        public string ParcelBoundingBoxRight {get; set;}
        [JsonProperty("parcelBoundingBoxTop")]
        public string ParcelBoundingBoxTop {get; set;}
        [JsonProperty("parcelBoundingBoxBottom")]
        public string ParcelBoundingBoxBottom {get; set;}
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
            WaitForTokenInMsec = int.Parse(configuration["Keystone_WaitForTokenInMsec"]);
            ResponseType = configuration["Keystone_ResponseType"];
            DisablePKCE = bool.Parse(configuration["Keystone_DisablePKCE"]);
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
        [JsonProperty("responseType")]
        public string ResponseType {get; set;}
        [JsonProperty("disablePKCE")]
        public bool DisablePKCE {get; set;}
    }
}
