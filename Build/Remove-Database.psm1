function Remove-Database {
    Param (
        [string]$sqlserver,
        [string]$dbName
    )
    
    "Drop db $dbName."

    $sqlConnectionString = "Server='$sqlserver';Database='master';Integrated Security=True"

    $sqlQueries = `
        "if exists(select * from sys.databases where name = '$dbName') 
        begin
            ALTER DATABASE [$dbName] SET SINGLE_USER WITH ROLLBACK IMMEDIATE
            drop DATABASE [$dbName]
        end"
    
    $conn = new-object System.Data.SqlClient.SQLConnection($sqlConnectionString)
    try {
        $conn.Open()
         $sqlQueries
        $sqlQueries | % {
            $cmd = $conn.CreateCommand()
            $cmd.CommandText = $_
            $cmd.ExecuteScalar()
            Write-Host "    Executed: $_" -ForegroundColor Gray
        }
    }

    finally{
        if($conn) {
            $conn.Close()
            $conn.Dispose()
        }
    }
}

