
Param(
  [Parameter (Mandatory = $false)]
  [string] $iniFile = ".\build.ini"
)

Import-Module .\Get-Config.psm1

$config = Get-Config -iniFile $iniFile 

$publishXmlLoc = $config.PublishXmlFile
$DatabaseProjectDacPacLocation = $config.DatabaseProjectDacPacLocation
$sqlProjLocation = $config.DatabaseProjectPath

Import-Module .\Build-DacPac.psm1
Import-Module .\Deploy-DacPac.psm1

# Drop database geooptix
Build-DacPac -dbProjPath $sqlProjLocation
Deploy-DacPac -dacPacPath $DatabaseProjectDacPacLocation -publishXmlFilePath $publishXmlLoc

# # Import the backed up db
# $path = "./temp/" + $currentDbName + ".bacpac"
# Import-BacPac -bacpacPath $path -sqlConnectionString $sqlConnectionString 

