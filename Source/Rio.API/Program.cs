using System;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Microsoft.Extensions.Configuration;

namespace Rio.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseKestrel(options =>
                {
                    var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                    options.Listen(IPAddress.Any, 80);

                    // 1/23 CG & MK - This is done so that Azure wont load the cert, it will only be used locally.
                    if (env == EnvironmentName.Development)
                    {
                        options.Listen(IPAddress.Any, 443, configure =>
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
                .UseStartup<Startup>()
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
                });
    }
}
