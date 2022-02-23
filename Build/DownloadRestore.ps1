
"Download EDF"
& "$PSScriptRoot\DatabaseDownload.ps1" -iniFile "./build.ini"

"Restore EDF"
& "$PSScriptRoot\DatabaseRestore.ps1" -iniFile "./build.ini"
