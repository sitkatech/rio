
"Download EDF"
& "$PSScriptRoot\DatabaseDownload.ps1" -iniFile "./build.ini" -tenantIniFile "./edf.ini"

"Restore EDF"
& "$PSScriptRoot\DatabaseRestore.ps1" -iniFile "./build.ini" -tenantIniFile "./edf.ini"

"Build EDF"
& "$PSScriptRoot\DatabaseBuild.ps1" -iniFile "./build.ini" -tenantIniFile "./edf.ini"
