
"Download Qanat"
& "$PSScriptRoot\DatabaseDownload.ps1" -iniFile "./build.ini" -secretsIniFile "./secrets.ini"

"Restore Qanat"
& "$PSScriptRoot\DatabaseRestore.ps1" -iniFile "./build.ini"

"Build Qanat"
& "$PSScriptRoot\DatabaseBuild.ps1" -iniFile "./build.ini"
