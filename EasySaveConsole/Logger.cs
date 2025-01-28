using System;
using System.IO;
using Newtonsoft.Json;

namespace EasySave
{
    public class Logger
    {
        public void LogAction(BackupTask task, string sourceFile, string targetFile)
        {
            var logEntry = new
            {
                Timestamp = DateTime.Now,
                TaskName = task.Name,
                SourceFile = sourceFile,
                TargetFile = targetFile,
                FileSize = new FileInfo(sourceFile).Length,
                TransferTimeMs = 0 // Placeholder for transfer time
            };

            string logPath = Path.Combine("Logs", $"{DateTime.Now:yyyy-MM-dd}.json");
            Directory.CreateDirectory("Logs");
            File.AppendAllText(logPath, JsonConvert.SerializeObject(logEntry, Formatting.Indented) + Environment.NewLine);
        }
    }
}
