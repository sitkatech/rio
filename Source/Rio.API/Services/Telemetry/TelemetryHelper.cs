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

            if (request?.Body?.CanRead == true)
            {
                request.EnableBuffering();

                var bodySize = (int) (request.ContentLength ?? request.Body.Length);
                if (bodySize > 0)
                {
                    request.Body.Position = 0;

                    byte[] body;

                    await using (var ms = new MemoryStream(bodySize))
                    {
                        await request.Body.CopyToAsync(ms);

                        body = ms.ToArray();
                    }

                    request.Body.Position = 0;

                    var requestTelemetry = context.Features.Get<RequestTelemetry>();
                    if (requestTelemetry != null)
                    {
                        var requestBodyString = Encoding.UTF8.GetString(body);

                        requestTelemetry.Properties.Add("RequestBody", requestBodyString);
                    }
                }
            }

            await next();
        }
    }
}