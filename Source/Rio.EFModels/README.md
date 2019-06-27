To regenerate the EF Models:
- install the following nuget package: Microsoft.EntityFrameworkCore.Tools version 2.2.4
- run the following PowerShell command:
	Scaffold-DbContext "Server=.\;Database=RioDB;Trusted_Connection=True;" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Entities/Generated -Project Rio.EFModels -Context "RioDbContext" -Force -StartupProject Rio.API -DataAnnotations -Schemas dbo -UseDatabaseNames
- grep the Generated folder and remove ".Generated"