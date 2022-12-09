function Deploy-DacPac {
    Param (
        [string]$dacPacPath,
        [string]$publishXmlFilePath
    )
    "Deploying DacPac."

    & sqlpackage /a:Publish /sf:$dacPacPath /Profile:$publishXmlFilePath

}
