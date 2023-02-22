Add-Type -AssemblyName System.Web

Write-Host "Launch Cosmos Emulator"

$path = "C:\Program Files\Azure Cosmos DB Emulator\CosmosDB.Emulator.exe"

$fileVersion = (Get-Item $path).VersionInfo.FileVersion
Write-Host "Installed version: $fileVersion"

Start-Process -FilePath $path -ArgumentList "/NoUi", "/NoExplorer"

$Verb = "GET"
$ResourceType = "dbs";
$dateTime = [DateTime]::UtcNow.ToString("ddd, dd MMM yyyy HH:mm:ss \G\M\T")
$keyType="master";
$tokenVersion="1.0";
$MasterKey = 'C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==';
$cryptoSHA256 = New-Object System.Security.Cryptography.HMACSHA256
$cryptoSHA256.Key = [System.Convert]::FromBase64String($MasterKey)
$payLoad = "$($verb.ToLowerInvariant())`n$($resourceType.ToLowerInvariant())`n`n$($dateTime.ToLowerInvariant())`n`n"
$hashedPayLoad = $cryptoSHA256.ComputeHash([System.Text.Encoding]::UTF8.GetBytes($payLoad))
$signature = [System.Convert]::ToBase64String($hashedPayLoad);
$authHeader = [System.Web.HttpUtility]::UrlEncode("type=$keyType&ver=$tokenVersion&sig=$signature")
$reqHeader = @{authorization=$authHeader; "x-ms-version"="2017-02-22";"x-ms-date"=$dateTime}
$queryUri = "https://localhost:8081/dbs"
$contentType = "application/json"
$count = 1;
$noOfAttempts = 10;

while($count -le $noOfAttempts)
{
    Sleep -s 10

    try
    {
        $result = Invoke-RestMethod -Method $Verb -Uri $queryUri -Headers $reqHeader -ContentType $contentType 
        $count = $noOfAttempts;
        Write-Host "Emulator launched."
        exit 0
    }
    catch [System.Net.Sockets.SocketException] , [System.Net.WebException]
    {
        Write-Host "launching emulator in attempt $count"        
    }
    catch 
    {        
        $count = $noOfAttempts;
        throw "Uri: $($queryUri) `nError: $($_.Exception)"
    }

    $count++;   
}

Write-Host "Failed to launch emulator in $count attempts"
exit 1 
