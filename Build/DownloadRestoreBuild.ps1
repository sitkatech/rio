Param(
  [Parameter (Mandatory = $false)]
  [string] $iniFile = ".\build.ini",
  [Parameter (Mandatory = $true)]
  [string] $tenantIniFile
)

"Download DB"
& "$PSScriptRoot\DatabaseDownload.ps1" -iniFile $iniFile -tenantIniFile $tenantIniFile 

"Restore DB"
& "$PSScriptRoot\DatabaseRestore.ps1" -iniFile $iniFile -tenantIniFile $tenantIniFile 

"Build DB"
& "$PSScriptRoot\DatabaseBuild.ps1" -iniFile $iniFile -tenantIniFile $tenantIniFile
