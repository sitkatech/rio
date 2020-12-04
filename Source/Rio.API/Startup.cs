using System;
using System.Net.Mail;
using Hangfire;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Rio.API.Services;
using Rio.API.Services.Authorization;
using Rio.EFModels.Entities;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Internal;

namespace Rio.API
{
    public class Startup
    {
        private readonly IWebHostEnvironment _environment;
        public Startup(IWebHostEnvironment environment, IConfiguration configuration)
        {
            Configuration = configuration;
            _environment = environment;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
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

            // todo: Calling 'BuildServiceProvider' from application code results in an additional copy of singleton services being created.
            // Consider alternatives such as dependency injecting services as parameters to 'Configure'.
            var rioConfiguration = services.BuildServiceProvider().GetService<IOptions<RioConfiguration>>().Value;

            var keystoneHost = rioConfiguration.KEYSTONE_HOST;
            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme).AddIdentityServerAuthentication(options =>
            {
                options.Authority = keystoneHost;
                options.RequireHttpsMetadata = false;
                options.LegacyAudienceValidation = true;
                options.EnableCaching = false;
                options.SupportedTokens = SupportedTokens.Jwt;
            });

            var connectionString = rioConfiguration.DB_CONNECTION_STRING;
            services.AddDbContext<RioDbContext>(c => { c.UseSqlServer(connectionString, x => x.UseNetTopologySuite()); });

            services.AddSingleton(Configuration);
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddTransient(s => new KeystoneService(s.GetService<IHttpContextAccessor>(), keystoneHost.Replace("core", "")));

            services.AddSingleton(x => new SitkaSmtpClientService(rioConfiguration));

            services.AddScoped(s => s.GetService<IHttpContextAccessor>().HttpContext);
            services.AddScoped(s => UserContext.GetUserFromHttpContext(s.GetService<RioDbContext>(), s.GetService<IHttpContextAccessor>().HttpContext));
            services.AddScoped<ICimisPrecipJob, CimisPrecipJob>();

            var rioDBbConnectionString = Configuration["DB_CONNECTION_STRING"];
            // Add Hangfire services.
            services.AddHangfire(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(rioDBbConnectionString, new SqlServerStorageOptions
                {
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                    QueuePollInterval = TimeSpan.Zero,
                    UseRecommendedIsolationLevel = true,
                    DisableGlobalLocks = true
                }));

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                //app.UseDeveloperExceptionPage();
                app.UseExceptionHandler(
                    options =>
                    {
                        //options.UseDeveloperExceptionPage();
                        options.Run(
                            async context =>
                            {
                                var ex = context.Features.Get<IExceptionHandlerFeature>();
                                if (ex != null)
                                {
                                    var err = $"<strong>Error: {ex.Error.Message}</strong><br/>Source:{ex.Error.Source}<br/ >Request Path:{context.Request.Path}<hr />";
                                    err += $"QueryString: {(context.Request.QueryString.HasValue ? context.Request.QueryString.ToString() : "No query arguments")}<hr />";

                                    err += $"Stack Trace: {ex.Error.StackTrace.Replace(Environment.NewLine, "<br />")}";
                                    if (ex.Error.InnerException != null)
                                        err +=
                                            $"Inner Exception:{ex.Error.InnerException?.Message.Replace(Environment.NewLine, "<br />")}";
                                    err += "<hr/>";
                                    // This bit here to check for a form collection!
                                    if (context.Request.HasFormContentType && context.Request.Form.Any())
                                    {
                                        err += "<table border=\"1\"><tr><td colspan=\"2\">Form collection:</td></tr>";
                                        foreach (var form in context.Request.Form)
                                        {
                                            err += $"<tr><td>{form.Key}</td><td>{form.Value}</td></tr>";
                                        }
                                        err += "</table>";
                                    }

                                    var smtpClient = context.RequestServices.GetRequiredService<SitkaSmtpClientService>();
                                    var supportMailAddress = new MailAddress(Configuration["SupportEmailAddress"]);
                                    var mailMessage = new MailMessage
                                    {
                                        Subject = $"{Configuration["PlatformShortName"]} error",
                                        Body = err,
                                        IsBodyHtml = true,
                                        From = supportMailAddress
                                    };
                                    mailMessage.To.Add(supportMailAddress);
                                    smtpClient.Send(mailMessage);
                                }
                            });
                        //options.UseExceptionHandler("/Home/Error");
                        //options.UseStatusCodePagesWithReExecute("/Home/Error/{0}");
                    }
                );
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
        }
    }
}
