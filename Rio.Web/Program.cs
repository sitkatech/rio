using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace Rio.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Build().Run();
        }

        public static IHostBuilder BuildWebHost(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseKestrel(options =>
                    {
                        var env = Environment.GetEnvironmentVariable(
                            "ASPNETCORE_ENVIRONMENT"); // Same as env.IsDevelopment()

                        options.Listen(IPAddress.Any, 80);
                        // 1/23 CG & MK - This is done so that Azure wont load the cert, it will only be used locally.
                        if (env == Microsoft.Extensions.Hosting.Environments.Development)
                        {
                            options.Listen(IPAddress.Any, 443,
                                configure =>
                                {
                                    configure.UseHttps(new X509Certificate2("dev_cert.pfx", "password#1"));
                                });
                        }
                    });
                })
                .ConfigureLogging(logging => { logging.ClearProviders(); })
                .UseSerilog((context, services, configuration) =>
                {
                    configuration
                        .Enrich.FromLogContext()
                        .ReadFrom.Configuration(context.Configuration);
                });
            return host;
        }
    }
}
