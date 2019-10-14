using System;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Rio.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = new HostBuilder()
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
                                    var httpsConnectionAdapterOptions = new HttpsConnectionAdapterOptions()
                                    {
                                        ClientCertificateMode = ClientCertificateMode.AllowCertificate,
                                        ServerCertificate = new X509Certificate2("api-dev.rio.org.pfx", "password#1"),
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
    }
}
