using System;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Protocols;
using Rio.API.Services;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;
using Serilog.Sinks.Email;

namespace Rio.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            SetupLogger();

            var host = new HostBuilder()
                .UseSerilog()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseKestrel(serverOptions =>
                        {
                            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                            serverOptions.Listen(IPAddress.Any, 80);

                            // 1/23 CG & MK - This is done so that Azure wont load the cert, it will only be used locally.
                            if (env == Microsoft.Extensions.Hosting.Environments.Development)
                            {
                                serverOptions.Listen(IPAddress.Any, 443, configure =>
                                {
                                    var devSSLCertLocation = Environment.GetEnvironmentVariable("DevSSLCertLocation");
                                    var httpsConnectionAdapterOptions = new HttpsConnectionAdapterOptions()
                                    {
                                        ClientCertificateMode = ClientCertificateMode.AllowCertificate,
                                        ServerCertificate = new X509Certificate2(devSSLCertLocation, "password#1"),
                                        ClientCertificateValidation = (certificate2, chain, arg3) => true
                                    };

                                    configure.UseHttps(httpsConnectionAdapterOptions);
                                });
                            }
                        })
                        .UseIISIntegration()
                        .UseStartup<Startup>();
                })
                .ConfigureAppConfiguration((hostContext, config) =>
                {
                    // delete all default configuration providers
                    config.Sources.Clear();
                    config.SetBasePath(Directory.GetCurrentDirectory());
                    config.AddEnvironmentVariables();

                    var configurationRoot = config.Build();

                    var secretPath = configurationRoot["SECRET_PATH"];
                    if (File.Exists(secretPath))
                    {
                        config.AddJsonFile(secretPath);
                    }
                })
                .Build();

            host.Run();
        }

        private static void SetupLogger()
        {
            var emailOutputTemplate = @"Time:{Timestamp: yyyy-MM-dd HH:mm:ss.fff zzz}{NewLine}
Level:{Level}{NewLine}
Request Path:{RequestPath}{NewLine}
Message:{Message}{NewLine}
Exception:{Exception}{NewLine}
Additional Properties:{Properties:j}";
            Serilog.Debugging.SelfLog.Enable(Console.WriteLine);
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (environment != Environments.Development)
            {
                Log.Logger = new LoggerConfiguration()
                    .WriteTo.Console()
                    .WriteTo.File($"log.txt", rollingInterval: RollingInterval.Day, restrictedToMinimumLevel: LogEventLevel.Warning)
                    .WriteTo.Email(new EmailConnectionInfo
                    {
                        MailServer = Environment.GetEnvironmentVariable("SMTP_HOST"),
                        Port = Int32.Parse(Environment.GetEnvironmentVariable("SMTP_PORT")),
                        FromEmail = Environment.GetEnvironmentVariable("SupportEmailAddress"),
                        ToEmail = Environment.GetEnvironmentVariable("SupportEmailAddress"),
                        EmailSubject =
                                $"{Environment.GetEnvironmentVariable("PlatformShortName")} Alert: {Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")} error"
                    }, restrictedToMinimumLevel: LogEventLevel.Error, batchPostingLimit: 1,
                        outputTemplate: emailOutputTemplate)
                    .CreateLogger();
            }
            else
            {
                Log.Logger = new LoggerConfiguration()
                    .WriteTo.Console()
                    .CreateLogger();
            }
        }
    }
}
