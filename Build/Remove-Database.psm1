function Remove-Database {
    [Diagnostics.CodeAnalysis.SuppressMessageAttribute("PSUseShouldProcessForStateChangingFunctions", "")]
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
        $sqlQueries | ForEach-Object {
            $cmd = $conn.CreateCommand()
            $cmd.CommandText = $_
            $cmd.ExecuteScalar()
            Write-Output "    Executed: $_" -ForegroundColor Gray
        }
    }

    finally{
        if($conn) {
            $conn.Close()
            $conn.Dispose()
        }
    }
}

