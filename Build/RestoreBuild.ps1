

"Restore Qanat"
& "$PSScriptRoot\DatabaseRestore.ps1"  -iniFile "./build.ini"

"Build Qanat"
& "$PSScriptRoot\DatabaseBuild.ps1" -iniFile "./build.ini"
