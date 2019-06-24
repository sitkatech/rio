using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json.Linq;
using Rio.Models.Helpers;

namespace Rio.API.Services.Filter
{
    public class RequiresValidJSONBodyFilterAttribute : TypeFilterAttribute
    {
        private static string _message;
        public RequiresValidJSONBodyFilterAttribute(string message) : base(typeof(RequiresValidJSONBodyImpl))
        {
            _message = message;
        }

        private class RequiresValidJSONBodyImpl : IAsyncActionFilter
        {
            private readonly HttpContext _httpContext;
            public RequiresValidJSONBodyImpl(HttpContext httpContext)
            {
                _httpContext = httpContext;
            }

            public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
            {
                var request = _httpContext.Request;
                request.Body.Seek(0, SeekOrigin.Begin);
                request.EnableRewind();
                using (StreamReader reader = new StreamReader(request.Body))
                {
                    var bodyString = reader.ReadToEnd();
                    if (string.IsNullOrEmpty(bodyString))
                    {
                        context.Result = new BadRequestObjectResult($"{_message}");
                        return;
                    }

                    if (!bodyString.TryParseJson(out JObject result) || result == null)
                    {
                        context.Result = new BadRequestObjectResult($"{_message}");
                        return;
                    }
                }

                await next();
            }
        }
    }
}
