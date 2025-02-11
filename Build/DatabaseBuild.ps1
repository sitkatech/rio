
Param(
  [Parameter (Mandatory = $false)]
  [string] $iniFile = ".\build.ini"
)

Import-Module .\Get-Config.psm1

$config = Get-Config -iniFile $iniFile 

$publishXmlLoc = $config.PublishXmlFile
$DatabaseProjectDacPacLocation = $config.DatabaseProjectDacPacLocation
$sqlProjLocation = $config.DatabaseProjectPath
$msBuildPath = $config.MsBuildFilePath

Import-Module .\Build-DacPac.psm1
Import-Module .\Deploy-DacPac.psm1

$buildResult = Build-DacPac -dbProjPath $sqlProjLocation -msBuildPath $msBuildPath
Write-Output ("Command Run: '$($buildResult.CommandUsedToBuild)'")

if ($buildResult.BuildSucceeded -eq $true) {
  Write-Output ("Build completed successfully in {0:N1} seconds." -f $buildResult.BuildDuration.TotalSeconds)
        
  Deploy-DacPac -dacPacPath $DatabaseProjectDacPacLocation -publishXmlFilePath $publishXmlLoc
}
elseif ($buildResult.BuildSucceeded -eq $false) {
  Write-Output ("Build failed after {0:N1} seconds. Check the build log file '$($buildResult.BuildLogFilePath)' for errors." -f $buildResult.BuildDuration.TotalSeconds)

  Get-Content $($buildResult.BuildLogFilePath) -Tail 20
}
elseif ($null -eq $buildResult.BuildSucceeded) {
  Write-Output "Unsure if build passed or failed: $($buildResult.Message)"
}

