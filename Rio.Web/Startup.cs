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
            mainAppApiUrl = configuration["mainAppApiUrl"];
            createAccountRedirectUrl = configuration["createAccountRedirectUrl"];
            allowTrading = bool.Parse(configuration["allowTrading"]);
            includeWaterSupply = bool.Parse(configuration["includeWaterSupply"]);
            geoserverMapServiceUrl = configuration["geoserverMapServiceUrl"];
            keystoneAuthConfiguration = new KeystoneAuthConfigurationDto(configuration);
            platformLongName = configuration["platformLongName"];
            platformShortName = configuration["platformShortName"];
            leadOrganizationShortName = configuration["leadOrganizationShortName"];
            leadOrganizationLongName = configuration["leadOrganizationLongName"];
            leadOrganizationHomeUrl = configuration["leadOrganizationHomeUrl"];
            faviconFilename = configuration["faviconFilename"];
            homepageBannerFilename = configuration["homepageBannerFilename"];
            leadOrganizationLogoFilename = configuration["leadOrganizationLogoFilename"];
            enabledGETIntegration = bool.Parse(configuration["enabledGETIntegration"]);
            navThemeColor = configuration["navThemeColor"];
            applicationInternalName = configuration["applicationInternalName"];
            allowOpenETSync = bool.Parse(configuration["allowOpenETSync"]);
            appInsightsInstrumentationKey =  configuration["appInsightsInstrumentationKey"];
            parcelBoundingBoxLeft =  double.Parse(configuration["parcelBoundingBoxLeft"]);
            parcelBoundingBoxRight =  double.Parse(configuration["parcelBoundingBoxRight"]);
            parcelBoundingBoxTop =  double.Parse(configuration["parcelBoundingBoxTop"]);
            parcelBoundingBoxBottom =  double.Parse(configuration["parcelBoundingBoxBottom"]);
        }

        public string mainAppApiUrl { get; set; }
        public string createAccountRedirectUrl { get; set; }
        public bool allowTrading { get; set; }
        public bool allowOpenETSync {get; set;}
        public bool includeWaterSupply { get; set; }
        public string geoserverMapServiceUrl { get; set; }
        public KeystoneAuthConfigurationDto keystoneAuthConfiguration { get; set; }
        public string platformLongName { get; set; }
        public string platformShortName { get; set; }
        public string leadOrganizationShortName { get; set; }
        public string leadOrganizationLongName { get; set; }
        public string leadOrganizationHomeUrl { get; set; }
        public string faviconFilename {get; set;}
        public string homepageBannerFilename {get; set;}
        public string leadOrganizationLogoFilename { get; set;}
        public bool enabledGETIntegration { get; set;}
        public string navThemeColor { get; set;}
        public string applicationInternalName { get; set;}
        public string appInsightsInstrumentationKey {get; set;}
        public double parcelBoundingBoxLeft {get; set;}
        public double parcelBoundingBoxRight {get; set;}
        public double parcelBoundingBoxTop {get; set;}
        public double parcelBoundingBoxBottom {get; set;}
    }

    public class KeystoneAuthConfigurationDto
    {
        public KeystoneAuthConfigurationDto(IConfiguration configuration)
        {
            clientId = configuration["keystone_clientId"];
            issuer = configuration["keystone_issuer"];
            redirectUriRelative = configuration["keystone_redirectUriRelative"];
            scope = configuration["keystone_scope"];
            sessionChecksEnabled = bool.Parse(configuration["keystone_sessionCheckEnabled"]);
            logoutUrl = configuration["keystone_logoutUrl"];
            postLogoutRedirectUri = configuration["keystone_postLogoutRedirectUri"];
            waitForTokenInMsec = int.Parse(configuration["keystone_waitForTokenInMsec"]);
            responseType = configuration["keystone_responseType"];
            disablePKCE = bool.Parse(configuration["keystone_disablePKCE"]);
        }

        public string clientId { get; set; }
        public string issuer { get; set; }
        public string redirectUriRelative { get; set; }
        public string scope { get; set; }
        public bool sessionChecksEnabled { get; set; }
        public string logoutUrl { get; set; }
        public string postLogoutRedirectUri { get; set; }
        public int waitForTokenInMsec { get; set; }
        public string responseType {get; set;}
        public bool disablePKCE {get; set;}
    }
}
