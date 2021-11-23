using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using Hangfire;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Rio.API.Services;
using Rio.API.Services.Authorization;
using Rio.EFModels.Entities;
using Hangfire.SqlServer;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DependencyCollector;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Logging;
using Rio.API.Services.Telemetry;
using Serilog;
using Serilog.Sinks.ApplicationInsights.Sinks.ApplicationInsights.TelemetryConverters;
using ILogger = Serilog.ILogger;

namespace Rio.API
{
    public class Startup
    {
        private readonly TelemetryClient _telemetryClient;
        private readonly IWebHostEnvironment _environment;
        private readonly string _instrumentationKey;
        public Startup(IWebHostEnvironment environment, IConfiguration configuration)
        {
            Configuration = configuration;
            _environment = environment;

            _instrumentationKey = Configuration["APPINSIGHTS_INSTRUMENTATIONKEY"];

            if (!string.IsNullOrWhiteSpace(_instrumentationKey))
            {
                _telemetryClient = new TelemetryClient(TelemetryConfiguration.CreateDefault())
                {
                    InstrumentationKey = _instrumentationKey
                };
            }
            else
            {
                _telemetryClient = new TelemetryClient(TelemetryConfiguration.CreateDefault());
            }
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApplicationInsightsTelemetry(_instrumentationKey);
            services.AddControllers().AddNewtonsoftJson(opt =>
                {
                    if (!_environment.IsProduction())
                    {
                        opt.SerializerSettings.Formatting = Formatting.Indented;
                    }
                    opt.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                    var resolver = opt.SerializerSettings.ContractResolver;
                    if (resolver != null)
                    {
                        if (resolver is DefaultContractResolver defaultResolver)
                        {
                            defaultResolver.NamingStrategy = null;
                        }
                    }
                });

            services.Configure<RioConfiguration>(Configuration);
            var rioConfiguration = Configuration.Get<RioConfiguration>();

            var keystoneHost = rioConfiguration.KEYSTONE_HOST;
            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    if (_environment.IsDevelopment())
                    {
                        // NOTE: CG 3/22 - This allows the self-signed cert on Keystone to work locally.
                        options.BackchannelHttpHandler = new HttpClientHandler()
                        {
                            ServerCertificateCustomValidationCallback = (message, certificate2, arg3, arg4) => true
                        };
                        //These allow the use of the container name and the url when developing.
                        options.TokenValidationParameters.ValidateIssuer = false;
                    }
                    options.TokenValidationParameters.ValidateAudience = false;
                    options.Authority = keystoneHost;
                    options.RequireHttpsMetadata = false;
                    options.SecurityTokenValidators.Clear();
                    options.SecurityTokenValidators.Add(new JwtSecurityTokenHandler
                    {
                        MapInboundClaims = false
                    });
                    options.TokenValidationParameters.NameClaimType = "name";
                    options.TokenValidationParameters.RoleClaimType = "role";
                });

            var connectionString = rioConfiguration.DB_CONNECTION_STRING;
            services.AddDbContext<RioDbContext>(c => { c.UseSqlServer(connectionString, x => x.UseNetTopologySuite()); });

            services.AddSingleton(Configuration);
            services.AddSingleton<ITelemetryInitializer, CloudRoleNameTelemetryInitializer>();
            services.AddSingleton<ITelemetryInitializer, UserInfoTelemetryInitializer>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            var logger = GetSerilogLogger();
            services.AddSingleton(logger);

            services.AddTransient(s => new KeystoneService(s.GetService<IHttpContextAccessor>(), keystoneHost));

            services.AddSingleton(x => new SitkaSmtpClientService(rioConfiguration));

            services.AddHttpClient("OpenETClient", c =>
            {
                c.BaseAddress = new Uri(rioConfiguration.OpenETAPIBaseUrl);
                c.Timeout = new TimeSpan(60 * TimeSpan.TicksPerSecond);
                c.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(rioConfiguration.OpenETAPIKey);
            });

            services.AddHttpClient("GenericClient", c =>
            {
                c.Timeout = new TimeSpan(60 * TimeSpan.TicksPerSecond);
            });

            services.AddScoped(s => s.GetService<IHttpContextAccessor>().HttpContext);
            services.AddScoped(s => UserContext.GetUserFromHttpContext(s.GetService<RioDbContext>(), s.GetService<IHttpContextAccessor>().HttpContext));
            services.AddScoped<IOpenETService, OpenETService>();
            services.AddScoped<ICimisPrecipJob, CimisPrecipJob>();
            services.AddScoped<IOpenETRetrieveFromBucketJob, OpenETRetrieveFromBucketJob>();
            services.AddScoped<IOpenETTriggerBucketRefreshJob, OpenETTriggerBucketRefreshJob>();

            // Add Hangfire services.
            services.AddHangfire(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(connectionString, new SqlServerStorageOptions
                {
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                    QueuePollInterval = TimeSpan.Zero,
                    UseRecommendedIsolationLevel = true,
                    DisableGlobalLocks = true
                }));

            services.AddControllers();


            #region Swagger
            // Base swagger services
            services.AddSwaggerGen(options =>
            {
                // extra options here if you wanted
            });
            #endregion

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime applicationLifetime, ILoggerFactory loggerFactory, ILogger logger)
        {
            loggerFactory.AddSerilog(logger);
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseCors(policy =>
            {
                //TODO: don't allow all origins
                policy.AllowAnyOrigin();
                policy.AllowAnyHeader();
                policy.AllowAnyMethod();
                policy.WithExposedHeaders("WWW-Authenticate");
            });

            app.UseAuthentication();
            app.UseAuthorization();

            app.Use(TelemetryHelper.PostBodyTelemetryMiddleware);

            #region Swagger
            // Register swagger middleware and enable the swagger UI which will be 
            // accessible at https://<apihostname>/swagger
            // NOTE: There is no auth on these endpoints out of the box.
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint($"/swagger/v1/swagger.json", "V1");
            });
            #endregion


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseHangfireDashboard("/hangfire", new DashboardOptions()
            {
                Authorization = new[] { new HangfireAuthorizationFilter(Configuration) }
            });
            app.UseHangfireServer(new BackgroundJobServerOptions()
            {
                WorkerCount = 1
            });
            GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute { Attempts = 0 });

            HangfireJobScheduler.ScheduleRecurringJobs();

            applicationLifetime.ApplicationStopping.Register(OnShutdown);

            var modules = app.ApplicationServices.GetServices<ITelemetryModule>();
            var dependencyModule = modules.OfType<DependencyTrackingTelemetryModule>().FirstOrDefault();

            if (dependencyModule != null)
            {
                var domains = dependencyModule.ExcludeComponentCorrelationHttpHeadersOnDomains;
                domains.Add("core.windows.net");
                domains.Add("10.0.75.1");
            }
        }

        private void OnShutdown()
        {
            _telemetryClient.Flush();
            Thread.Sleep(1000);
        }

        private ILogger GetSerilogLogger()
        {
            var outputTemplate = $"{{Timestamp:yyyy-MM-dd HH:mm:ss zzz}} {{Level}} | {{RequestId}}-{{SourceContext}}: {{Message}}{{NewLine}}{{Exception}}";
            var serilogLogger = new LoggerConfiguration()
                .ReadFrom.Configuration(Configuration)
                .WriteTo.Console(outputTemplate: outputTemplate);

            if (!_environment.IsDevelopment())
            {
                serilogLogger.WriteTo.ApplicationInsights(_telemetryClient, new TraceTelemetryConverter());
            }

            return serilogLogger.CreateLogger();
        }
    }
}
