using System.Reflection;

namespace Rio.Models.DataTransferObjects;

public class SystemInfoDto
{
    public string Environment { get; set; }
    public string CurrentTimeUTC { get; set; }
    public string Application => Assembly.GetEntryAssembly().GetName().Name;
    public string Version => Assembly.GetEntryAssembly().GetName().Version.ToString();
}