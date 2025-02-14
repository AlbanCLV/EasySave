using System;
using System.IO;
using System.Xml.Linq;
using Newtonsoft.Json;

namespace EasySave.Log
{
    /// <summary>
    /// Log_Models est une classe singleton qui gère la journalisation des actions et erreurs
    /// des tâches de sauvegarde, au format JSON (ou XML).
    /// </summary>
    public class Log_Models
    {
        private static Log_Models _instance;
        private static readonly object _lock = new object();
        private readonly string logDirectory; // Répertoire où les logs seront sauvegardés

        public string Type_File { get; set; } = "json"; // Format de log par défaut

        // Constructeur privé pour empêcher l'instanciation externe.
        private Log_Models(string logDirectory = "Logs")
        {
            this.logDirectory = logDirectory;
        }

        /// <summary>
        /// Accède à l'instance unique de Log_Models.
        /// </summary>
        public static Log_Models Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                        _instance = new Log_Models();
                    return _instance;
                }
            }
        }

        /// <summary>
        /// Enregistre une action de sauvegarde au format JSON, incluant le statut d'encryption.
        /// </summary>
        public void LogAction(string name, string source, string target, string time, string action)
        {
            long fileSize = GetDirectorySize(new DirectoryInfo(source));
            double fileSizeInKB = fileSize / 1024.0;
            var logEntry = new
            {
                Action = action,
                Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                TaskName = "Backup_" + name,
                SourceFile = source,
                TargetFile = target,
                FileSizeKB = fileSizeInKB,
                TransferTimeMs = time,
            };

            string logPath = Path.Combine(logDirectory, $"{DateTime.Now:yyyy-MM-dd}.json");
            Directory.CreateDirectory(logDirectory);
            File.AppendAllText(logPath, JsonConvert.SerializeObject(logEntry, Formatting.Indented) + Environment.NewLine);
        }

        public void LogErreur(string name, string baseAction, string error)
        {
            var logEntry = new
            {
                Action = baseAction,
                Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                TaskName = "Backup_" + name,
                TransferTimeMs = -1,
                Error = error
            };

            string logPath = Path.Combine(logDirectory, $"{DateTime.Now:yyyy-MM-dd}.json");
            Directory.CreateDirectory(logDirectory);
            File.AppendAllText(logPath, JsonConvert.SerializeObject(logEntry, Formatting.Indented) + Environment.NewLine);
        }

        /// <summary>
        /// Méthode publique pour enregistrer une erreur au format JSON.
        /// </summary>
        public void LogErreurJSON(string name, string baseAction, string error)
        {
            LogErreur(name, baseAction, error);
        }

        public void TypeFile(string input)
        {
            if (input.ToLower() == "xml" || input.ToLower() == "json")
                this.Type_File = input;
        }

        private long GetDirectorySize(DirectoryInfo directory)
        {
            if (!directory.Exists)
            {
                LogErreur("Error", "GetDirectorySize", "Folder not found");
                Environment.Exit(0);
            }

            long size = 0;
            foreach (var file in directory.GetFiles("*", System.IO.SearchOption.AllDirectories))
                size += file.Length;
            return size;
        }
    }
}
