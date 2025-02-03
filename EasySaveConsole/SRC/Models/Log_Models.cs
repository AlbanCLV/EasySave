// Définit une entrée de journal, avec les informations sur chaque action (horodatage, chemin des fichiers, taille, temps de transfert, etc.).

using System;
using System.IO;
using EasySave.Models;
using Newtonsoft.Json;

namespace EasySave
{
    /// <summary>
    /// Model for logs
    /// </summary>
    public class Log_Models
    {
        /// <summary>
        /// Create a new log entry from task infos, source and destination
        /// </summary>
        /// <param name="task">BackupJob_Models task object</param>
        /// <param name="sourceFile">sourceFile path</param>
        /// <param name="targetFile">targetFile path</param>
        public void LogAction(BackupJob_Models task, string sourceFile, string targetFile)
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
