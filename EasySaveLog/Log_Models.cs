using System;
using System.IO;
using System.Diagnostics;  // Pour utiliser Stopwatch
using Newtonsoft.Json;
using System.Xml.Linq;

namespace EasySaveLog
{
    /// <summary>
    /// Model for managing log entries related to backup actions.
    /// </summary>
    public class Log_Models
    {
#nullable enable
        private static Log_Models? _instance;  // Instance unique
        private static readonly object _lock = new object(); // Verrou pour éviter les problèmes de multithreading
        private readonly string logDirectory;  // Répertoire des logs
        public string Type_File { get; set; } = "json";

        /// <summary>
        /// Constructor to specify the log directory (default is "Logs").
        /// </summary>
        /// <param name="logDirectory">The directory where logs will be saved.</param>
        public Log_Models(string logDirectory = "Logs")
        {
            this.logDirectory = logDirectory; // Initializes the log directory.
        }
        // Méthode pour récupérer l'instance unique
        public static Log_Models Instance
        {
            get
            {
                lock (_lock)  // Thread-safety
                {
                    if (_instance == null)
                    {
                        _instance = new Log_Models();
                    }
                    return _instance;
                }
            }
        }
        public void LogAction(string name, string source, string target, string time, string act)
        {
            if (Type_File.ToLower() == "json")
                LogActionJSON(name, source, target, time, act);
            else
                LogActionXML(name, source, target, time, act);
        }
        public void LogErreur(string task, string baseAction, string erreur)
        {
            if (Type_File.ToLower() == "json")
                LogErreurJSON(task, baseAction, erreur);
            else
                LogErreurXML(task, baseAction, erreur);
        }
        /// <summary>
        /// Creates a new log entry for a backup action, with details about the task, source, and destination.
        /// </summary>
        /// <param name="task">The backup job task object containing task details.</param>
        /// <param name="act">The action performed (e.g., "Started", "Completed").</param>
        public void LogActionJSON(string name, string source, string target, string time, string act)
        {
            // Initialize variable to store the file size (default is 0).
            long fileSize = 0;

            // calculate the total size of files within it (including subdirectories).
            fileSize = GetDirectorySize(new DirectoryInfo(source));
            double fileSizeInKB = fileSize / 1024.0;
            // Create a log entry with information about the action, timestamp, task, source, target, file size, and transfer time.
            var logEntry = new
            {
                Action = act, // The action performed (e.g., "Backup Started").
                Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), // Format to show only date and time.
                TaskName = "Backup_" + name, // Name of the backup task.
                SourceFile = source, // Path of the source file or directory.
                TargetFile = target, // Path of the target file or directory.
                FileSize = fileSizeInKB, // Size of the source file/directory.
                TimeMS = time // Placeholder for transfer time (currently not used).
            };
            // Create the log file path based on the current date.
            string logPath = Path.Combine(logDirectory, $"{DateTime.Now:yyyy-MM-dd}.json");
            // Ensure that the log directory exists, create it if necessary.
            Directory.CreateDirectory(logDirectory);
            // Append the log entry to the log file as a JSON object, with proper formatting and a newline.
            File.AppendAllText(logPath, JsonConvert.SerializeObject(logEntry, Formatting.Indented) + Environment.NewLine);
        }
        public void LogErreurJSON(string task, string Base, string Erreur)
        {

            // Create a log entry with information about the action, timestamp, task, source, target, file size, and transfer time.
            var logEntry = new
            {
                Action = Base, // The action performed (e.g., "Backup Started").
                Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), // Format to show only date and time.
                TaskName = "Backup_" + task, // Name of the backup task.
                TimeMs = -1, // Placeholder for transfer time (currently not used).
                Error = Erreur // Placeholder for transfer time (currently not used).
            };
            // Create the log file path based on the current date.
            string logPath = Path.Combine(logDirectory, $"{DateTime.Now:yyyy-MM-dd}.json");
            // Ensure that the log directory exists, create it if necessary.
            Directory.CreateDirectory(logDirectory);
            // Append the log entry to the log file as a JSON object, with proper formatting and a newline.
            File.AppendAllText(logPath, JsonConvert.SerializeObject(logEntry, Formatting.Indented) + Environment.NewLine);
        }
        public void LogActionXML(string name, string source, string target, string time, string act)
        {
            // Initialiser la variable pour stocker la taille du fichier (par défaut à 0)
            long fileSize = 0;

            // Calculer la taille totale des fichiers dans le répertoire source
            fileSize = GetDirectorySize(new DirectoryInfo(source));
            double fileSizeInKB = fileSize / 1024.0;

            // Définir le chemin du fichier log XML basé sur la date actuelle
            string logPath = Path.Combine(logDirectory, $"{DateTime.Now:yyyy-MM-dd}.xml");

            // Vérifier si le fichier existe, sinon créer un nouveau document XML
            XDocument xmlDoc;
            if (File.Exists(logPath))
            {
                xmlDoc = XDocument.Load(logPath);
            }
            else
            {
                xmlDoc = new XDocument(new XElement("Logs"));
            }

            // Créer une nouvelle entrée de log
            XElement logEntry = new XElement("Log",
                new XElement("Action", act),
                new XElement("Timestamp", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")),
                new XElement("TaskName", "Backup_" + name),
                new XElement("SourceFile", source),
                new XElement("TargetFile", target),
                new XElement("FileSize", fileSizeInKB),
                new XElement("TimeMs", time)
            );

            // Ajouter l'entrée au document XML
            xmlDoc.Root?.Add(logEntry);

            // S'assurer que le répertoire existe
            Directory.CreateDirectory(logDirectory);

            // Sauvegarder le fichier XML
            xmlDoc.Save(logPath);
        }
        public void LogErreurXML(string task, string baseAction, string erreur)
        {
            // Définir le chemin du fichier log XML basé sur la date actuelle
            string logPath = Path.Combine(logDirectory, $"{DateTime.Now:yyyy-MM-dd}.xml");

            // Vérifier si le fichier existe, sinon créer un nouveau document XML
            XDocument xmlDoc;
            if (File.Exists(logPath))
            {
                xmlDoc = XDocument.Load(logPath);
            }
            else
            {
                xmlDoc = new XDocument(new XElement("Logs"));
            }

            // Créer une nouvelle entrée de log
            XElement logEntry = new XElement("Log",
                new XElement("Action", baseAction),
                new XElement("Timestamp", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")),
                new XElement("TaskName", "Backup_" + task),
                new XElement("TimeMs", -1),
                new XElement("Error", erreur)
            );

            // Ajouter l'entrée au document XML
            xmlDoc.Root?.Add(logEntry);

            // S'assurer que le répertoire existe
            Directory.CreateDirectory(logDirectory);

            // Sauvegarder le fichier XML
            xmlDoc.Save(logPath);
        }


        public void TypeFile(string Input)
        {
            if (Input == "xml" || Input == "json")
            {
                this.Type_File = Input;
            }
        }
        /// <summary>
        /// Calculates the total size of all files in a directory and its subdirectories.
        /// </summary>
        /// <param name="directory">The directory to calculate the size of.</param>
        /// <returns>The total size of all files in the directory in bytes.</returns>
        private long GetDirectorySize(DirectoryInfo directory)
        {
            if (!directory.Exists)
            {
                LogErreurJSON("Error", "Error", "Folder not found");
                Environment.Exit(0);
            }

            long size = 0;

            // Iterate through all files in the directory and subdirectories.
            foreach (var file in directory.GetFiles("*", SearchOption.AllDirectories))
            {
                size += file.Length; // Add the file size to the total.
            }

            return size; // Return the total size.
        }

    }
}