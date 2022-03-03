function Deploy-DacPac {
    Param (
        [string]$dacPacPath,
        [string]$publishXmlFilePath
    )
    "Deploying DacPac."
    
    & "C:\Program Files (x86)\Microsoft SQL Server\140\DAC\bin\SqlPackage.exe" /a:Publish /sf:$dacPacPath /Profile:$publishXmlFilePath

}
