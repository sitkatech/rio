
Param(
    [Parameter (Mandatory = $false)]
    [string] $iniFile = ".\build.ini",
    [Parameter (Mandatory = $false)]
    [string] $secretsIniFile = ".\secrets.ini"
)

Import-Module .\Get-Config.psm1

$config = Get-Config -iniFile $iniFile
$secrets = Get-Config -iniFile $secretsIniFile

$backupConnectionString = $secrets.BackupConnectionString

if ("" -ne $backupConnectionString)
{
    $BackupStorageContext = New-AzStorageContext -ConnectionString $backupConnectionString

    $path = ".\temp"
    If(!(Test-Path $path))
    {
          New-Item -ItemType Directory -Force -Path $path
    }
    $dbName = $config.DatabaseName
    $blobName = $dbName + ".bacpac"
    $destination = "temp\" + $blobName    
    Get-AzStorageBlobContent -Container $config.ContainerName -Blob $blobName -Context $BackupStorageContext -Destination $destination -Force
   
}
else
{
    Write-Output "Null values, no Connection String"
}