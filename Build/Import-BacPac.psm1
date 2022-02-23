function Import-BacPac {
    Param (
        [string]$sqlConnectionString,
        [string]$bacpacPath
    )
    "Importing BacPac."
    $sqlConnectionString
    
    & "C:\Program Files (x86)\Microsoft SQL Server\140\DAC\bin\SqlPackage.exe" /a:Import /sf:$bacpacPath /tcs:$sqlConnectionString
}
