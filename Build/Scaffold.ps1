Param(
  [Parameter (Mandatory = $false)]
  [string] $iniFile = ".\build.ini"
)

Import-Module .\Get-Config.psm1

$config = Get-Config -iniFile $iniFile 

$connectionString = "Server=" + $config.Server + ";Database=" + $config.DatabaseName + ";Trusted_Connection=True;"

"Scaffold"
& Scaffold-DbContext $connectionString Microsoft.EntityFrameworkCore.SqlServer -OutputDir Entities/Generated -Project $config.ApiEFModelsProject -Context $config.ApiEFModelsDbContextName -Force -StartupProject $config.ApiEFModelsProject -DataAnnotations -UseDatabaseNames -NoOnConfiguring -Namespace $config.ApiEFModelsNamespace -Schemas $config.DatabaseSchemas

$args = "--db-server-name=" + $config.Server + " --db-name=" + $config.DatabaseName + " --generate-simple-dtos=true --csharp-dto-namespace=" + $config.ApiModelsNamespace + " --code-namespace=" + $config.ApiEFModelsNamespace + " --api-efmodels-output-dir=" + $config.ApiEFModelExtensionMethodsPath + " --api-models-output-dir=" + $config.ApiModelsPath + " --table-exclude-list=" + $config.TableExcludeList

Start-Process -Wait -FilePath $config.EFPocoGeneratorExePath -ArgumentList $args