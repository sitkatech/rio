function Get-Config {
    Param (
        [string]
        $iniFile,
        [string]
        $tenantIniFile
    )

    "Reading Config from " + $iniFile
        
    Get-Content $iniFile | foreach-object -begin {$h=@{}} -process { 
        $label,$value = $_ -split '=',2
        if(($label.CompareTo("") -ne 0) -and ($label.StartsWith("[") -ne $True)) 
        { 
            $h.Add($label, $value) 
        } 
    }

    if($tenantIniFile)
    {
        "Reading Config from " + $tenantIniFile
        Get-Content $tenantIniFile | foreach-object -process { 
            $label,$value = $_ -split '=',2
            if(($label.CompareTo("") -ne 0) -and ($label.StartsWith("[") -ne $True)) 
            { 
                $h.Add($label, $value) 
            } 
        }    
    }

    return $h
}
