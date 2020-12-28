using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.AspNetCore.Http;

namespace Rio.API.Services.Telemetry
{
    public static class TelemetryHelper
    {
        public static async Task PostBodyTelemetryMiddleware(HttpContext context, Func<Task> next)
        {
            var request = context.Request;
            var requestTelemetry = context.Features.Get<RequestTelemetry>();

            if (request?.Body?.CanRead == true && requestTelemetry != null)
            {
                request.EnableBuffering();

                var bodySize = (int)(request.ContentLength ?? request.Body.Length);
                var requestBodyString = "No body to return";
                if (bodySize > 0)
                {
                    if ((bodySize / 1024L / 1024L) < 30)
                    {
                        request.Body.Position = 0;

                        byte[] body;

                        await using (var ms = new MemoryStream(bodySize))
                        {
                            await request.Body.CopyToAsync(ms);

                            body = ms.ToArray();
                        }

                        request.Body.Position = 0;

                        requestBodyString = Encoding.UTF8.GetString(body);
                    }
                    else
                    {
                        requestBodyString = "Request body too large and therefore not logged";
                    }
                }

                if (!requestTelemetry.Properties.ContainsKey("RequestBody"))
                {
                    requestTelemetry.Properties.Add("RequestBody", requestBodyString);
                }
                else
                {
                    requestTelemetry.Properties["RequestBody"] = requestBodyString;
                }
            }

            await next();
        }
    }
}