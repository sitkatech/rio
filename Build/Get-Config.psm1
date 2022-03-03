function Get-Config {
    Param (
        [string]$iniFile
    )
    "Reading Config from " + $iniFile
        
    Get-Content $iniFile | foreach-object -begin {$h=@{}} -process { 
        $label,$value = $_ -split '=',2
        if(($label.CompareTo("") -ne 0) -and ($label.StartsWith("[") -ne $True)) 
        { 
            $h.Add($label, $value) 
        } 
    }

    return $h
}
