using System.Web;

namespace Rio.API.Services
{
    public static class StringUtilities
    {
        public static string HtmlEncode(this string value)
        {
            return string.IsNullOrWhiteSpace(value) ? value : HttpUtility.HtmlEncode(value);
        }

        public static string HtmlEncodeWithBreaks(this string value)
        {
            var ret = value.HtmlEncode();
            return string.IsNullOrWhiteSpace(ret) ? ret : ret.Replace("\r\n", "\n").Replace("\r", "\n").Replace("\n", "<br/>\r\n");
        }

    }
}