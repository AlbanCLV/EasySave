// D�finit une entr�e de journal, avec les informations sur chaque action (horodatage, chemin des fichiers, taille, temps de transfert, etc.).

using System;
using System.IO;
using EasySave.Models;
using Newtonsoft.Json;

namespace EasySave
{
    public class Log_Models
    {
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
