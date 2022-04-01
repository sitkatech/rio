Param(
  [Parameter (Mandatory = $false)]
  [string] $iniFile = ".\build.ini"
)

Import-Module .\Get-Config.psm1

$config = Get-Config -iniFile $iniFile 

$tablesFiles = Get-ChildItem -Path $config.DatabaseTablesDir -File
$tablesFiles += Get-ChildItem -Path $config.DatabaseViewsDir -File
$lookupTablesFiles = Get-ChildItem -Path $config.DatabaseLookupTablesDir -File

$tablesCompared = Compare-Object -ReferenceObject ($tablesFiles) -DifferenceObject ($lookupTablesFiles) -Property BaseName  | Where-Object{$_.sideIndicator -eq "<="}

$tablesComparedName = $tablesCompared.BaseName

$tablesIncluded = Compare-Object -ReferenceObject ($tablesComparedName) -DifferenceObject ($config.TableExcludeList.Split(",")) | Where-Object{$_.sideIndicator -eq "<="}

$tablesIncludedForEFScaffold = $tablesIncluded.InputObject

$connectionString = "Server=" + $config.Server + ";Database=" + $config.DatabaseName + ";Trusted_Connection=True;"

"Scaffold"
& Scaffold-DbContext $connectionString Microsoft.EntityFrameworkCore.SqlServer -OutputDir Entities/Generated -Project $config.ApiEFModelsProject -Context $config.ApiEFModelsDbContextName -Force -StartupProject $config.ApiEFModelsProject -DataAnnotations -UseDatabaseNames -NoOnConfiguring -Namespace $config.ApiEFModelsNamespace -Tables $tablesIncludedForEFScaffold

$args = "--db-server-name=" + $config.Server + " --db-name=" + $config.DatabaseName + " --generate-simple-dtos=true --csharp-dto-namespace=" + $config.ApiModelsNamespace + " --code-namespace=" + $config.ApiEFModelsNamespace + " --api-efmodels-output-dir=" + $config.ApiEFModelExtensionMethodsPath + " --api-models-output-dir=" + $config.ApiModelsPath + " --table-exclude-list=" + $config.TableExcludeList + " --enum-list=" + ($lookupTablesFiles.BaseName -join ",") + " --typescript-enums-output-dir=" + $config.TypescriptEnumsPath

Start-Process -Wait -FilePath $config.EFPocoGeneratorExePath -ArgumentList $args