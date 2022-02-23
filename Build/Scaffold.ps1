$projectPath = "C:\git\sitkatech\edf"
$apiModelsPath = $projectPath + "\Rio.Models\DataTransferObjects\Generated"
$apiEFModelsPath = $projectPath + "\Rio.EFModels\Entities\Generated"
$apiEFModelExtensionMethodsPath = $apiEFModelsPath + "\ExtensionMethods"
$apiEFModelsDbContextPath = $apiEFModelsPath + "\RioDbContext.cs"

"Scaffold"
& Scaffold-DbContext "Server=.\;Database=EDFDB;Trusted_Connection=True;" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Entities/Generated -Project Rio.EFModels -Context "RioDbContext" -Force -StartupProject Rio.EFModels -DataAnnotations -UseDatabaseNames

# fix the DbContext OnConfiguring line

(Get-Content -Raw $apiEFModelsDbContextPath) -replace 'protected override void OnConfiguring\(DbContextOptionsBuilder optionsBuilder\)\s*{\s*if \(!optionsBuilder.IsConfigured\)\s*{[^}]*}\s*}', '' | Out-File $apiEFModelsDbContextPath -encoding ascii

# remove .Generated from the namespace

ForEach ($File in (Get-ChildItem -Path $apiEFModelsPath -Recurse -File)) {
    (Get-Content -Raw $File.FullName) -replace 'Entities.Generated', 'Entities' | Out-File $File.FullName -encoding ascii
}

Start-Process -Wait -FilePath "C:\git\sitkatech\efcorepocogenerator\EFCorePOCOGenerator\bin\Debug\net5.0\EFCorePOCOGenerator.exe" --db-name="EDFDB", --generate-simple-dtos=true, --csharp-dto-namespace="Rio.Models.DataTransferObjects", --code-namespace="Rio.EFModels.Entities", --api-efmodels-output-dir=$apiEFModelExtensionMethodsPath, --api-models-output-dir=$apiModelsPath, --table-exclude-list="DatabaseMigration"