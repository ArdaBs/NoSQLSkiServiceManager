namespace NoSQLSkiServiceManager.Services
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Timers;

    public class MongoBackupManager : IHostedService
    {
        private readonly Timer _backupTimer;
        private readonly string _toolsDirectory;
        private readonly string _databaseName;
        private readonly string _backupDirectory;

        public MongoBackupManager(string databaseName, string toolsDirectory, string backupDirectory)
        {
            _databaseName = databaseName;
            _toolsDirectory = toolsDirectory;
            _backupDirectory = backupDirectory;

            // 24h timer for backup, time is in ms
            _backupTimer = new Timer(86400000);
            _backupTimer.Elapsed += OnBackupTimerElapsed;
            _backupTimer.AutoReset = true;
        }

        public void Start()
        {
            _backupTimer.Start();
        }

        public void Stop()
        {
            _backupTimer.Stop();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("MongoBackupManager wird gestartet...");
            Start();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("MongoBackupManager wird gestoppt...");
            Stop();
            return Task.CompletedTask;
        }

        private void OnBackupTimerElapsed(object sender, ElapsedEventArgs e)
        {
            CreateBackup();
        }

        private void CreateBackup()
        {
            var mongodumpPath = GetToolPath("mongodump.exe");
            var backupFolderPath = GetBackupPath();
            var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            var backupFilePath = Path.Combine(backupFolderPath, $"backup-{timestamp}.gz");

            if (!File.Exists(mongodumpPath))
            {
                Console.WriteLine($"Fehler: 'mongodump.exe' wurde nicht gefunden im Verzeichnis: {mongodumpPath}");
                return;
            }

            var startInfo = new ProcessStartInfo
            {
                FileName = mongodumpPath,
                Arguments = $"--archive=\"{backupFilePath}\" --gzip",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            };

            try
            {
                using (var process = new Process { StartInfo = startInfo })
                {
                    process.Start();
                    var result = process.StandardOutput.ReadToEnd();
                    process.WaitForExit();

                    Console.WriteLine("Backup erfolgreich abgeschlossen.");
                    Console.WriteLine(result);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler beim Ausführen von mongodump: {ex.Message}");
            }
        }


        private string GetToolPath(string toolName)
        {
            var binPath = AppDomain.CurrentDomain.BaseDirectory;

            var relativeToolPath = Path.Combine("..", "..", "..", "MongoTools", toolName);

            var toolPath = Path.GetFullPath(Path.Combine(binPath, relativeToolPath));

            return toolPath;
        }

        private string GetBackupPath()
        {
            var binPath = AppDomain.CurrentDomain.BaseDirectory;

            var relativeBackupPath = Path.Combine("..", "..", "..", "MongoDbBackups");

            var backupPath = Path.GetFullPath(Path.Combine(binPath, relativeBackupPath));

            Directory.CreateDirectory(backupPath);

            return backupPath;
        }

    }

}
