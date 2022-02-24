Param(
  [Parameter (Mandatory = $false)]
  [string] $iniFile = ".\build.ini"
)

Import-Module .\Get-Config.psm1

$config = Get-Config -iniFile $iniFile 

$connectionString = "Server=" + $config.Server + ";Database=" + $config.DatabaseName + ";Trusted_Connection=True;"

"Scaffold"
& Scaffold-DbContext $connectionString Microsoft.EntityFrameworkCore.SqlServer -OutputDir Entities/Generated -Project $config.ApiEFModelsProject -Context $config.ApiEFModelsDbContextName -Force -StartupProject $config.ApiEFModelsProject -DataAnnotations -UseDatabaseNames

# fix the DbContext OnConfiguring line
$apiEFModelsDbContextPath = $config.ApiEFModelsPath + "/" + $config.ApiEFModelsDbContextName + ".cs"

(Get-Content -Raw $apiEFModelsDbContextPath) -replace 'protected override void OnConfiguring\(DbContextOptionsBuilder optionsBuilder\)\s*{\s*if \(!optionsBuilder.IsConfigured\)\s*{[^}]*}\s*}', '' | Out-File $apiEFModelsDbContextPath -encoding ascii

# remove .Generated from the namespace

ForEach ($File in (Get-ChildItem -Path $config.ApiEFModelsPath -Recurse -File)) {
    (Get-Content -Raw $File.FullName) -replace 'Entities.Generated', 'Entities' | Out-File $File.FullName -encoding ascii
}

$args = "--db-server-name=" + $config.Server + " --db-name=" + $config.DatabaseName + " --generate-simple-dtos=true --csharp-dto-namespace=" + $config.ApiModelsNamespace + " --code-namespace=" + $config.ApiEFModelsNamespace + " --api-efmodels-output-dir=" + $config.ApiEFModelExtensionMethodsPath + " --api-models-output-dir=" + $config.ApiModelsPath + " --table-exclude-list=" + $config.TableExcludeList

Start-Process -Wait -FilePath $config.EFPocoGeneratorExePath -ArgumentList $args