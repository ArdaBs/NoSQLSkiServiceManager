$backupDir = "C:\JetStreamAPI_Backups"
$dbName = "JetStreamAPI"

Write-Host "Searching for the latest backup for restoration..."
$latestBackup = Get-ChildItem -Path $backupDir | Where-Object { $_.PSIsContainer } | Sort-Object CreationTime -Descending | Select-Object -First 1
$latestBackupDir = $latestBackup.FullName

if ($latestBackupDir) {
    Write-Host "Latest backup found: $latestBackupDir"
    Write-Host "Starting restoration of database: $dbName"
    mongorestore --db $dbName $latestBackupDir\$dbName

    if ($?) {
        Write-Host "Restoration completed successfully."
    } else {
        Write-Host "Restoration failed."
    }
} else {
    Write-Host "No backup found for restoration."
}
