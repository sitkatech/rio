function Get-Config {
    Param (
        [string]$iniFile
    )
    "Reading Config from " + $iniFile
    $h = @{}
    Get-Content $iniFile | foreach-object -begin {} -process {
        $label,$value = $_ -split '=',2
        if(($label.CompareTo("") -ne 0) -and ($label.StartsWith("[") -ne $True))
        {
            $h.Add($label, $value)
        }
    }

    return $h
}
