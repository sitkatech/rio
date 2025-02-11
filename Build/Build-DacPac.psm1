function Build-DacPac {
    Param (
        [string]$dbProjPath,
        [string]$msBuildPath = $null
    )

    "Building DacPac."

    Import-Module .\Invoke-MsBuild.psm1

    if ([string]::IsNullOrEmpty($msBuildPath)) {
        $result = Invoke-MsBuild -Path $dbProjPath -MsBuildParameters "/p:CmdLineInMemoryStorage=True /p:DSP=Microsoft.Data.Tools.Schema.Sql.SqlAzureV12DatabaseSchemaProvider"
    }
    else {
        $result = Invoke-MsBuild -MsBuildFilePath $msBuildPath -Path $dbProjPath -MsBuildParameters "/p:CmdLineInMemoryStorage=True /p:DSP=Microsoft.Data.Tools.Schema.Sql.SqlAzureV12DatabaseSchemaProvider"
    }
    return $result
}
