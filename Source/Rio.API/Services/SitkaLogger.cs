using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Logging;
using Serilog.Core;

namespace Rio.API.Services
{
    public class SitkaLogger
    {
        private ILogger<SitkaLogger> _logger;
        private IHttpContextAccessor _contextAccessor;
        private RioConfiguration _rioConfiguration;

        public SitkaLogger(ILogger<SitkaLogger> logger, IHttpContextAccessor contextAccessor, RioConfiguration rioConfiguration)
        {
            _logger = logger;
            _contextAccessor = contextAccessor;
            _rioConfiguration = rioConfiguration;
        }

        /// <summary>
        /// Logs an error message when it is inappropriate to include all request information.
        /// </summary>
        /// <param name="exception"></param>
        public void LogAbridgedErrorMessage(Exception exception)
        {
            LogDetailedErrorMessage("Unhandled Exception", exception, null);
        }

        public void LogDetailedErrorMessage(Exception exception)
        {
            var context = _contextAccessor.HttpContext;
            LogDetailedErrorMessage("Unhandled Exception", exception, context);
        }

        public void LogDetailedErrorMessage(string additionalMessage, Exception exception)
        {
            var context = _contextAccessor.HttpContext;
            LogDetailedErrorMessage(additionalMessage, exception, context);
        }

        public void LogDetailedErrorMessage(string additionalMessage, Exception exception, HttpContext context)
        {
            LogDetailedErrorMessage(string.Format("{0}:{1}{2}", additionalMessage, Environment.NewLine, exception), context);
        }

        public void LogDetailedErrorMessage(string additionalMessage)
        {
            var context = _contextAccessor.HttpContext;
            LogDetailedErrorMessage(additionalMessage, context);
        }

        public void LogDetailedErrorMessage(string additionalMessage, HttpContext context)
        {
            var sessionDebugInfo = context != null ? GetUserAndSessionInformationForError(context) : string.Empty;
            var requestDebugInfo = context != null ? DebugInfo(context) : string.Empty;
            var logMessage = string.Format("{1}{0}{0}{2}{0}{0}{3}", Environment.NewLine, additionalMessage, sessionDebugInfo, requestDebugInfo);
            _logger.LogError(logMessage);
        }

        public static string DebugInfo(HttpContext context)
        {
            var requestBody = HttpDebugInfo.GetRequestBody(context);
            return String.Format("IP Address: {1}{0}URL: {2}{0}{0}Begin Http Request Body:-->{0}{3}{0}<--End Http Request Body", Environment.NewLine, context.Connection.RemoteIpAddress, context.Request.GetDisplayUrl(), requestBody);
        }

        /// <summary>
        /// Projects (MM, Taurus, EE, etc.) should implement this for themselves to pick up logging of User & Session information
        /// </summary>
        public virtual string GetUserAndSessionInformationForError(HttpContext context)
        {
            var username = string.Empty;
            if (context != null && context.User != null && context.User.Identity != null)
            {
                username = context.User.Identity.Name;
            }
            return String.Format("Username: {0}", username);
        }
    }

    public static class HttpDebugInfo
    {
        public static string DebugInfoFromHttpRequestIfAny(HttpContext context)
        {
            string debugInfo = String.Empty;
            if (context != null)
                debugInfo = DebugInfo(context);
            return debugInfo;
        }

        public static string DebugInfo(HttpContext context)
        {
            var requestBody = GetRequestBody(context);
            return String.Format("IP Address: {1}{0}URL: {2}{0}Method:{3}{0}Http Request Body:{0}{4}", Environment.NewLine, context.Connection.RemoteIpAddress, context.Request.GetDisplayUrl(), context.Request.Method, requestBody);
        }

        public static string GetRequestBody(HttpContext context)
        {
            return context.Items.ContainsKey("request_body") ? (string) context.Items["request_body"] : "Request body not captured";
        }
    }
}
