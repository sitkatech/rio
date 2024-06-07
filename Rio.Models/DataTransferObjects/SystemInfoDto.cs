using System.Diagnostics;
using System.IO;
using System;
using System.Reflection;

namespace Rio.Models.DataTransferObjects;

public class SystemInfoDto
{
    public string Environment { get; set; }
    public string CurrentTimeUTC { get; set; }
    public string Application => Assembly.GetEntryAssembly().GetName().Name;
    public string FullInformationalVersion { get; set; }
    public string PodName { get; set; }
    public string Version { get; set; }
    public DateTime CompilationDateTime { get; set; }

    public SystemInfoDto()
    {
        FullInformationalVersion = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion;
        var assemblyVersion = Assembly.GetEntryAssembly()!.GetName().Version;
        Version = $"{assemblyVersion!.Major}.{assemblyVersion!.Minor}.{assemblyVersion!.Build}";

        var localAssemblyPathString = new Uri(Assembly.GetExecutingAssembly().Location).LocalPath;
        var fileInfo = new FileInfo(localAssemblyPathString);
        CompilationDateTime = fileInfo.LastWriteTime;
    }
}