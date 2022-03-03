function Build-DacPac {
    Param (
        [string]$dbProjPath
    )
    
    "Building DacPac."
    
    Import-Module .\Invoke-MsBuild.psm1
    
    $result = Invoke-MsBuild -Path $dbProjPath -MsBuildParameters "/p:CmdLineInMemoryStorage=True /p:DSP=Microsoft.Data.Tools.Schema.Sql.SqlAzureV12DatabaseSchemaProvider"

    return $result
}
