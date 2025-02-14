using System;
using System.IO;
using System.Xml.Linq;
using Newtonsoft.Json;

namespace EasySaveLog
{
    /// <summary>
    /// Log_Models est une classe singleton qui gère la journalisation des actions et erreurs
    /// des tâches de sauvegarde, au format JSON (et potentiellement XML).
    /// </summary>
    public class Log_Models
    {
        private static Log_Models _instance;
        private static readonly object _lock = new object();
        private readonly string logDirectory; // Répertoire où les logs seront sauvegardés

        // Format de log par défaut ("json" ou "xml")
        public string Type_File { get; set; } = "json";

        /// <summary>
        /// Constructeur privé pour empêcher l'instanciation externe.
        /// </summary>
        /// <param name="logDirectory">Le répertoire de sauvegarde des logs (par défaut "Logs").</param>
        private Log_Models(string logDirectory = "Logs")
        {
            this.logDirectory = logDirectory;
        }

        /// <summary>
        /// Accède à l'instance unique de Log_Models.
        /// Utilisez Log_Models.Instance pour obtenir l'instance.
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
        /// Enregistre une action de sauvegarde au format JSON.
        /// </summary>
        /// <param name="name">Nom de la tâche.</param>
        /// <param name="source">Répertoire source.</param>
        /// <param name="target">Répertoire de destination.</param>
        /// <param name="time">Durée de l'opération.</param>
        /// <param name="action">Action réalisée (par ex. "Create Task").</param>
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
                TransferTimeMs = time
            };

            string logPath = Path.Combine(logDirectory, $"{DateTime.Now:yyyy-MM-dd}.json");
            Directory.CreateDirectory(logDirectory);
            File.AppendAllText(logPath, JsonConvert.SerializeObject(logEntry, Formatting.Indented) + Environment.NewLine);
        }

        /// <summary>
        /// Enregistre une erreur de sauvegarde au format JSON.
        /// </summary>
        /// <param name="name">Nom de la tâche.</param>
        /// <param name="baseAction">Action de base ayant provoqué l'erreur.</param>
        /// <param name="error">Message d'erreur.</param>
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
        /// Elle est appelée par Log_Controller.
        /// </summary>
        public void LogErreurJSON(string name, string baseAction, string error)
        {
            LogErreur(name, baseAction, error);
        }

        /// <summary>
        /// Définit le format de log à utiliser (xml ou json).
        /// </summary>
        /// <param name="input">Le format désiré.</param>
        public void TypeFile(string input)
        {
            if (input.ToLower() == "xml" || input.ToLower() == "json")
                this.Type_File = input;
        }

        /// <summary>
        /// Calcule la taille totale de tous les fichiers d'un répertoire (y compris les sous-répertoires).
        /// </summary>
        /// <param name="directory">Le répertoire cible.</param>
        /// <returns>La taille totale en octets.</returns>
        private long GetDirectorySize(DirectoryInfo directory)
        {
            if (!directory.Exists)
            {
                LogErreur("Error", "GetDirectorySize", "Folder not found");
                Environment.Exit(0);
            }

            long size = 0;
            foreach (var file in directory.GetFiles("*", SearchOption.AllDirectories))
                size += file.Length;
            return size;
        }
    }
}
