
Param(
  [Parameter (Mandatory = $false)]
  [string] $iniFile = ".\build.ini",
  [Parameter (Mandatory = $true)]
  [string] $tenantIniFile
)

Import-Module .\Get-Config.psm1

$config = Get-Config -iniFile $iniFile -tenantIniFile $tenantIniFile

$publishXmlLoc = $config.PublishXmlFile
$DatabaseProjectDacPacLocation = $config.DatabaseProjectDacPacLocation
$sqlProjLocation = $config.DatabaseProjectPath

Import-Module .\Build-DacPac.psm1
Import-Module .\Deploy-DacPac.psm1

# Drop database
Build-DacPac -dbProjPath $sqlProjLocation
Deploy-DacPac -dacPacPath $DatabaseProjectDacPacLocation -publishXmlFilePath $publishXmlLoc

# # Import the backed up db
# $path = "./temp/" + $currentDbName + ".bacpac"
# Import-BacPac -bacpacPath $path -sqlConnectionString $sqlConnectionString 

