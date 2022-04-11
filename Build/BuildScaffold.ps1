Param(
  [Parameter (Mandatory = $false)]
  [string] $iniFile = ".\build.ini",
  [Parameter (Mandatory = $true)]
  [string] $tenantIniFile
)

& "$PSScriptRoot\DatabaseBuild.ps1" -iniFile $iniFile -tenantIniFile $tenantIniFile 

& "$PSScriptRoot\Scaffold.ps1" -iniFile $iniFile -tenantIniFile $tenantIniFile 
