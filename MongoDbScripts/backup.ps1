$date = Get-Date -Format "yyyyMMdd_HHmm"
$backupDir = "C:\JetStreamAPI_Backups\$date"
$dbName = "JetStreamAPI"

Write-Host "Starting backup for database: $dbName"
if (-not (Test-Path $backupDir)) {
    New-Item -ItemType Directory -Force -Path $backupDir
    mongodump --db $dbName --out $backupDir
    Write-Host "Backup completed successfully. Stored in: $backupDir"
} else {
    Write-Host "Backup directory already exists. Backup not created."
}
