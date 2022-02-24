Param(
  [Parameter (Mandatory = $false)]
  [string] $iniFile = ".\build.ini"
)

Import-Module .\Get-Config.psm1

$config = Get-Config -iniFile $iniFile 

"Scaffold"
& Scaffold-DbContext "Server=$config.Server;Database=$config.DatabaseName;Trusted_Connection=True;" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Entities/Generated -Project Rio.EFModels -Context "RioDbContext" -Force -StartupProject Rio.EFModels -DataAnnotations -UseDatabaseNames

# fix the DbContext OnConfiguring line

(Get-Content -Raw $config.ApiEFModelsDbContextPath) -replace 'protected override void OnConfiguring\(DbContextOptionsBuilder optionsBuilder\)\s*{\s*if \(!optionsBuilder.IsConfigured\)\s*{[^}]*}\s*}', '' | Out-File $config.ApiEFModelsDbContextPath -encoding ascii

# remove .Generated from the namespace

ForEach ($File in (Get-ChildItem -Path $config.ApiEFModelsPath -Recurse -File)) {
    (Get-Content -Raw $File.FullName) -replace 'Entities.Generated', 'Entities' | Out-File $File.FullName -encoding ascii
}

Start-Process -Wait -FilePath "C:\git\sitkatech\efcorepocogenerator\EFCorePOCOGenerator\bin\Debug\net5.0\EFCorePOCOGenerator.exe" --db-name=$config.DatabaseName, --generate-simple-dtos=true, --csharp-dto-namespace="Rio.Models.DataTransferObjects", --code-namespace="Rio.EFModels.Entities", --api-efmodels-output-dir=$config.ApiEFModelExtensionMethodsPath, --api-models-output-dir=$config.ApiModelsPath, --table-exclude-list="DatabaseMigration"