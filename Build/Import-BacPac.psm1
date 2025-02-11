function Import-BacPac {
    Param (
        [string]$sqlConnectionString,
        [string]$bacpacPath
    )
    "Importing BacPac."
    $sqlConnectionString

    & sqlpackage /a:Import /sf:$bacpacPath /tcs:$sqlConnectionString
}
