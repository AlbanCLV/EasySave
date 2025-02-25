using System;
using System.IO;
using System.Diagnostics;  // Pour utiliser Stopwatch
using Newtonsoft.Json;
using System.Xml.Linq;
using System.Security.Cryptography;

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
        public void LogAction(string name, string source, string target, string time, string act, string encryptTime)
        {
            if (Type_File.ToLower() == "json")
                LogActionJSON(name, source, target, time, act, encryptTime);
            else
                LogActionXML(name, source, target, time, act, encryptTime);
        }
        public void LogErreur(string task, string baseAction, string erreur, string encryptTime)
        {
            if (Type_File.ToLower() == "json")
                LogErreurJSON(task, baseAction, erreur, encryptTime);
            else
                LogErreurXML(task, baseAction, erreur, encryptTime);
        }
        /// <summary>
        /// Creates a new log entry for a backup action, with details about the task, source, and destination.
        /// </summary>
        /// <param name="task">The backup job task object containing task details.</param>
        /// <param name="act">The action performed (e.g., "Started", "Completed").</param>
        public void LogActionJSON(string name, string source, string target, string time, string act, string encryptTime)
        {
           
          
            var logEntry = new object(); // Déclaration de la variable

            if (act == "View Task" || act == "DisplayExistingApplications")
            {
                logEntry = new
                {
                    Action = act, // The action performed (e.g., "Backup Started").
                    Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), // Format to show only date and time.
                };
            }
            else if (act == "Create Task" || act == "Deleting_task")
            {
                long fileSize = 0;            // calculate the total size of files within it (including subdirectories).
                fileSize = GetDirectorySize(new DirectoryInfo(source));
                double fileSizeInKB = fileSize / 1024.0;
                logEntry = new
                {
                    Action = act, // The action performed (e.g., "Backup Started").
                    Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), // Format to show only date and time.
                    TaskName =  name, // Name of the backup task.
                    SourceFile = source, // Path of the source file or directory.
                    TargetFile = target, // Path of the target file or directory.
                    Type = encryptTime,
                    FileSize = fileSizeInKB, // Size of the source file/directory.
                    TimeMS = time, // Placeholder for transfer time (currently not used).

                };
            }
            else if (act == "ChooseFileLog")
            {
                logEntry = new
                {
                    Action = act, // The action performed (e.g., "Backup Started").
                    Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), // Format to show only date and time.
                    Old_log_files = source,
                    New_log_files = target
                };
            }
            else if (act == "execute specific Task" || act == "execute ALL Task")
            {
                // Initialize variable to store the file size (default is 0).
                long fileSize = 0;            // calculate the total size of files within it (including subdirectories).
                fileSize = GetDirectorySize(new DirectoryInfo(source));
                double fileSizeInKB = fileSize / 1024.0;
                // Create a log entry with information about the action, timestamp, task, source, target, file size, and transfer time.
                logEntry = new
                {
                    Action = act, // The action performed (e.g., "Backup Started").
                    Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), // Format to show only date and time.
                    TaskName = "Backup_" + name, // Name of the backup task.
                    SourceFile = source, // Path of the source file or directory.
                    TargetFile = target, // Path of the target file or directory.
                    FileSize = fileSizeInKB, // Size of the source file/directory.
                    TimeMS = time, // Placeholder for transfer time (currently not used).
                    encryptTime = encryptTime, // Placeholder for transfer time (currently not used).

                };
            }
            else if (act == "FolderDecrypted")
            {
               
                logEntry = new
                {
                    Action = act, // The action performed (e.g., "Backup Started").
                    Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), // Format to show only date and time.
                    TimeMS = time, // Placeholder for transfer time (currently not used).

                };
            }
            else if (act == "AddBusinessApplication"|| act == "RemoveBusinessApplication")
            {
                logEntry = new
                {
                    Action = act, // The action performed (e.g., "Backup Started").
                    AppName = name,
                    Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), // Format to show only date and time.
                    TimeMS = time, // Placeholder for transfer time (currently not used).

                };
            }
            else if (act == "change language")
            {
                logEntry = new
                {
                    Action = act, // The action performed (e.g., "Backup Started").
                    Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), // Format to show only date and time.
                    New_Language_files = source,
                };
            }
            else if (act == "AddApplication" || act == "RemoveApplication")
            {
                logEntry = new
                {
                    Action = act, // The action performed (e.g., "Backup Started").
                    Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), // Format to show only date and time.
                    Application = name,
                };

            }
            else if (act == "isRunning" || act == "Is CLose")
            {
                logEntry = new
                {
                    Action = act, // The action performed (e.g., "Backup Started").
                    Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), // Format to show only date and time.
                    Application = name,
                };
            }
            else if (act == "Pause" || act == "Resume" || act =="Stop") 
            {
                logEntry = new
                {
                    Action = act, // The action performed (e.g., "Backup Started").
                    Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), // Format to show only date and time.
                };
            }
            // Create the log file path based on the current date.
            string logPath = Path.Combine(logDirectory, $"{DateTime.Now:yyyy-MM-dd}.json");
            // Ensure that the log directory exists, create it if necessary.
            Directory.CreateDirectory(logDirectory);
            // Append the log entry to the log file as a JSON object, with proper formatting and a newline.
            File.AppendAllText(logPath, JsonConvert.SerializeObject(logEntry, Formatting.Indented) + Environment.NewLine);
        }
        public void LogErreurJSON(string task, string Base, string Erreur, string encryptTime)
        {
            var logEntry = new object(); // Déclaration de la variable

            if (Base == "create_task_attempt"|| Base == "View_task_attempt" || Base == "delete_task_attempt")
            {
                logEntry = new
                {
                    Action = Base, // The action performed (e.g., "Backup Started").
                    Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), // Format to show only date and time.
                    TaskName = task, // Name of the backup task.
                    TimeMs = -1, // Placeholder for transfer time (currently not used).
                    Error = Erreur // Placeholder for transfer time (currently not used).
                };
            }
            else if (Base == "ChoiceMetier")
            {
                logEntry = new
                {
                    Action = Base, // The action performed (e.g., "Backup Started").
                    Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), // Format to show only date and time.
                    TimeMs = -1, // Placeholder for transfer time (currently not used).
                    Error = Erreur // Placeholder for transfer time (currently not used).
                };
            }
            else if (Base == "Execute_Task_attempt")
            {
                logEntry = new
                {
                    Action = Base, // The action performed (e.g., "Backup Started").
                    Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), // Format to show only date and time.
                    TaskName = task, // Name of the backup task.
                    TimeMs = -1, // Placeholder for transfer time (currently not used).
                    EncryptTime = encryptTime,
                    Error = Erreur // Placeholder for transfer time (currently not used).
                };
            }
            else if (Base == "Decrypt Folder")
            {
                logEntry = new
                {
                    Action = Base, // The action performed (e.g., "Backup Started").
                    Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), // Format to show only date and time.
                    TimeMs = -1, // Placeholder for transfer time (currently not used).
                    Error = Erreur // Placeholder for transfer time (currently not used).
                };
            }
            // Create a log entry with information about the action, timestamp, task, source, target, file size, and transfer time.

                // Create the log file path based on the current date.
            string logPath = Path.Combine(logDirectory, $"{DateTime.Now:yyyy-MM-dd}.json");
            // Ensure that the log directory exists, create it if necessary.
            Directory.CreateDirectory(logDirectory);
            // Append the log entry to the log file as a JSON object, with proper formatting and a newline.
            File.AppendAllText(logPath, JsonConvert.SerializeObject(logEntry, Formatting.Indented) + Environment.NewLine);
        }
        public void LogActionXML(string name, string source, string target, string time, string act, string encryptTime)
        {
            XElement logEntry = new XElement("LogEntry",
                new XElement("Action", "Unknown"),
                new XElement("Timestamp", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))
            );

            if (act == "View Task" || act == "DisplayExistingApplications")
            {
                logEntry = new XElement("LogEntry",
                    new XElement("Action", act),
                    new XElement("Timestamp", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))
                );
            }

            else if (act == "Create Task" || act == "Deleting_task")
            {
                long fileSize = GetDirectorySize(new DirectoryInfo(source));
                double fileSizeInKB = fileSize / 1024.0;

                logEntry = new XElement("LogEntry",
                    new XElement("Action", act),
                    new XElement("Timestamp", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")),
                    new XElement("TaskName", name),
                    new XElement("SourceFile", source),
                    new XElement("TargetFile", target),
                    new XElement("Type", encryptTime),
                    new XElement("FileSizeKB", fileSizeInKB),
                    new XElement("TimeMS", time)
                );
            }
            else if (act == "ChooseFileLog")
            {
                logEntry = new XElement("LogEntry",
                    new XElement("Action", act),
                    new XElement("Timestamp", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")),
                    new XElement("OldLogFiles", source),
                    new XElement("NewLogFiles", target)
                );
            }
            else if (act == "change language")
            {
                logEntry = new XElement("LogEntry",
                    new XElement("Action", act),
                    new XElement("Timestamp", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")),
                    new XElement("OldLogFiles", source)
                );
            }
            else if (act == "execute specific Task" || act == "execute ALL Task")
            {
                long fileSize = GetDirectorySize(new DirectoryInfo(source));
                double fileSizeInKB = fileSize / 1024.0;

                logEntry = new XElement("LogEntry",
                    new XElement("Action", act),
                    new XElement("Timestamp", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")),
                    new XElement("TaskName", "Backup_" + name),
                    new XElement("SourceFile", source),
                    new XElement("TargetFile", target),
                    new XElement("FileSizeKB", fileSizeInKB),
                    new XElement("TimeMS", time),
                    new XElement("EncryptTime", encryptTime)
                );
            }
            else if (act == "FolderDecrypted")
            {
                logEntry = new XElement("LogEntry",
                    new XElement("Action", act),
                    new XElement("Timestamp", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")),
                    new XElement("TimeMS", time)
                );
            }
            else if (act == "AddBusinessApplication" || act == "RemoveBusinessApplication")
            {
                logEntry = new XElement("LogEntry",
                    new XElement("Action", act),
                    new XElement("AppName", name),
                    new XElement("Timestamp", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")),
                    new XElement("TimeMS", time)
                );
            }

            string logPath = Path.Combine(logDirectory, $"{DateTime.Now:yyyy-MM-dd}.xml");
            Directory.CreateDirectory(logDirectory);

            if (!File.Exists(logPath))
            {
                new XDocument(new XElement("Logs", logEntry)).Save(logPath);
            }
            else
            {
                XDocument doc = XDocument.Load(logPath);
                doc.Root.Add(logEntry);
                doc.Save(logPath);
            }
        }

        public void LogErreurXML(string task, string Base, string Erreur, string encryptTime)
        {
            XElement logEntry = new XElement("LogEntry",
                new XElement("Action", "Unknown"),
                new XElement("Timestamp", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")),
                new XElement("Error", "No error specified")
            );

            if (Base == "create_task_attempt" || Base == "View_task_attempt" || Base == "delete_task_attempt")
            {
                logEntry = new XElement("LogEntry",
                    new XElement("Action", Base),
                    new XElement("Timestamp", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")),
                    new XElement("TaskName", task),
                    new XElement("TimeMS", -1),
                    new XElement("Error", Erreur)
                );
            }
            else if (Base == "ChoiceMetier")
            {
                logEntry = new XElement("LogEntry",
                    new XElement("Action", Base),
                    new XElement("Timestamp", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")),
                    new XElement("TimeMS", -1),
                    new XElement("Error", Erreur)
                );
            }
            else if (Base == "Execute_Task_attempt")
            {
                logEntry = new XElement("LogEntry",
                    new XElement("Action", Base),
                    new XElement("Timestamp", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")),
                    new XElement("TaskName", task),
                    new XElement("TimeMS", -1),
                    new XElement("EncryptTime", encryptTime),
                    new XElement("Error", Erreur)
                );
            }
            else if (Base == "Decrypt Folder")
            {
                logEntry = new XElement("LogEntry",
                    new XElement("Action", Base),
                    new XElement("Timestamp", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")),
                    new XElement("TimeMS", -1),
                    new XElement("Error", Erreur)
                );
            }

            string logPath = Path.Combine(logDirectory, $"{DateTime.Now:yyyy-MM-dd}.xml");
            Directory.CreateDirectory(logDirectory);

            if (!File.Exists(logPath))
            {
                new XDocument(new XElement("Logs", logEntry)).Save(logPath);
            }
            else
            {
                XDocument doc = XDocument.Load(logPath);
                doc.Root.Add(logEntry);
                doc.Save(logPath);
            }
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
                LogErreurJSON("Error", "Error", "Folder not found", "-1");
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