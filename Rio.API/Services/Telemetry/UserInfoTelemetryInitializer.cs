using System.Linq;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Http;

namespace Rio.API.Services.Telemetry
{
    public class UserInfoTelemetryInitializer : ITelemetryInitializer
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string User = "UserDisplayName";
        private const string UserGuid = "UserGuid";
        private const string UserEmail = "UserEmail";

        public UserInfoTelemetryInitializer(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public void Initialize(ITelemetry telemetry)
        {
            if (!(telemetry is RequestTelemetry requestTelemetry))
                return;

            // user data should be added always if we have it.
            if (!_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
                return;

            if (!requestTelemetry.Properties.ContainsKey(User))
            {
                requestTelemetry.Properties.Add(User, _httpContextAccessor.HttpContext.User.Identity.Name);
            }
            if (!requestTelemetry.Properties.ContainsKey(UserEmail) && _httpContextAccessor.HttpContext.User.Claims.Any(c => c.Type == "email"))
            {
                requestTelemetry.Properties.Add(UserEmail, _httpContextAccessor.HttpContext.User.FindFirst("email").Value);
            }
            if (!requestTelemetry.Properties.ContainsKey(UserGuid) && _httpContextAccessor.HttpContext.User.Claims.Any(c => c.Type == "sub"))
            {
                requestTelemetry.Properties.Add(UserGuid, _httpContextAccessor.HttpContext.User.FindFirst("sub").Value);
            }
        }
    }
}