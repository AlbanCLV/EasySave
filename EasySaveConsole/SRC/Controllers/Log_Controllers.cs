// Gère l'enregistrement des journaux dans des fichiers JSON.

using System;
using EasySave.Models;  // Pour utiliser LogEntry ou autres modèles si nécessaire
using System.IO;
using Newtonsoft.Json;
using EasySave.Utilities;


namespace EasySave.Controllers
{
    public class LogController
    {
        private const string LogPath = "Logs/logs.json";

        public void LogAction(BackupJobModel task, string sourceFile, string targetFile)
        {
            var logEntry = new
            {
                Timestamp = DateTime.Now,
                TaskName = task.Name,
                SourceFile = sourceFile,
                TargetFile = targetFile,
                FileSize = new FileInfo(sourceFile).Length
            };

            JsonHelper.SaveToJson(LogPath, logEntry);
        }
    }
}