

"Restore EDF"
& "$PSScriptRoot\DatabaseRestore.ps1"  -iniFile "./build.ini"

"Build EDF"
& "$PSScriptRoot\DatabaseBuild.ps1" -iniFile "./build.ini"
