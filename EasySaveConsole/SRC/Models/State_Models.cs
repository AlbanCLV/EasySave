using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Newtonsoft.Json;
using EasySaveLog;
using EasySaveConsole.ViewModels;

namespace EasySaveConsole.Models
{
    /// <summary>
    /// Singleton class responsible for managing the state of backup tasks and logging their progress.
    /// </summary>
    public class State_models
    {
        Log_Models LogModels = new Log_Models();
        private readonly string logDirectoryState; // Directory to store log files.
        private readonly LangManager lang;
        private static State_models _instance;
        private static readonly object _lock = new object();

        /// <summary>
        /// Gets the singleton instance of the State_models class.
        /// </summary>
        public static State_models Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                            _instance = new State_models();
                    }
                }
                return _instance;
            }
        }

        /// <summary>
        /// Constructor to specify the log directory (default is "States").
        /// </summary>
        /// <param name="logDirectoryState">The directory where logs will be saved.</param>
        public State_models(string logDirectoryState = "States")
        {
            this.logDirectoryState = logDirectoryState; // Initializes the log directory.
            lang = LangManager.Instance;
        }

        /// <summary>
        /// Updates the backup state and logs its progress.
        /// </summary>
        /// <param name="task">The backup task being monitored.</param>
        /// <param name="lasth">The timestamp of the last action.</param>
        /// <param name="desti">The destination directory for the backup.</param>
        public void StateUpdate(Backup_Models task, string lasth, string desti)
        {
            var StateEntry = new
            {
                Timestanp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                TaskName = "Backup_" + task.Name,
                HeureLastAction = lasth,
                Etat = "Active",
                TotalFile = CountFilesInDirectory(task.SourceDirectory),
                TotalSize = GetDirectorySize(new DirectoryInfo(task.SourceDirectory)) / 1024.0,
                Progress = $"{((GetDirectorySize(new DirectoryInfo(desti)) / 1024.0) * 100) / (GetDirectorySize(new DirectoryInfo(task.SourceDirectory)) / 1024.0)} %",
                RemainingFiles = CountFilesInDirectory(task.SourceDirectory) - CountFilesInDirectory(desti),
                RemainingSize = (GetDirectorySize(new DirectoryInfo(task.SourceDirectory)) / 1024.0) - (GetDirectorySize(new DirectoryInfo(desti)) / 1024.0),
                CurrentSourceFile = task.SourceDirectory,
                CurrentDestinationFiles = desti
            };

            string StatePath = Path.Combine(logDirectoryState, $"Sates {DateTime.Now:yyyy-MM-dd}.json");
            Directory.CreateDirectory(logDirectoryState);
            File.AppendAllText(StatePath, JsonConvert.SerializeObject(StateEntry, Formatting.Indented) + Environment.NewLine);
        }

        /// <summary>
        /// Marks a backup task as completed and logs the final state.
        /// </summary>
        public void StatEnd(Backup_Models task, string lasth, string desti)
        {
            var StateEntry = new
            {
                Timestanp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                TaskName = "Backup_" + task.Name,
                HeureLastAction = lasth,
                Etat = "Completed",
                TotalFileSource = CountFilesInDirectory(task.SourceDirectory),
                TotalFileTarget = CountFilesInDirectory(desti),
                TotalSizeSource = GetDirectorySize(new DirectoryInfo(task.SourceDirectory)) / 1024.0,
                TotalSizeTarget = GetDirectorySize(new DirectoryInfo(desti)) / 1024.0,
                Progress = "100 %",
                RemainingFiles = 0,
                RemainingSize = 0,
                CurrentSourceFile = task.SourceDirectory,
                CurrentDestinationFiles = desti
            };

            string StatePath = Path.Combine(logDirectoryState, $"Sates {DateTime.Now:yyyy-MM-dd}.json");
            Directory.CreateDirectory(logDirectoryState);
            File.AppendAllText(StatePath, JsonConvert.SerializeObject(StateEntry, Formatting.Indented) + Environment.NewLine);
        }

        /// <summary>
        /// Logs an error that occurred during a backup task.
        /// </summary>
        public void StateError(Backup_Models task, string lasth, string error)
        {
            var StateEntry = new
            {
                Timestanp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                TaskName = "Backup_" + task.Name,
                HeureLastAction = lasth,
                Etat = "Error",
                Error = error,
            };

            string StatePath = Path.Combine(logDirectoryState, $"Sates {DateTime.Now:yyyy-MM-dd}.json");
            Directory.CreateDirectory(logDirectoryState);
            File.AppendAllText(StatePath, JsonConvert.SerializeObject(StateEntry, Formatting.Indented) + Environment.NewLine);
        }

        /// <summary>
        /// Calculates the total size of a directory including subdirectories.
        /// </summary>
        private long GetDirectorySize(DirectoryInfo directory)
        {
            if (!directory.Exists)
            {
                LogModels.LogErreurJSON("Error", "try to delete a task", "Folder not found", "-1");
                Environment.Exit(0);
            }

            long size = 0;
            foreach (var file in directory.GetFiles("*", SearchOption.AllDirectories))
            {
                size += file.Length;
            }

            return size;
        }

        /// <summary>
        /// Counts the number of files in a specified directory.
        /// </summary>
        static int CountFilesInDirectory(string path)
        {
            try
            {
                if (Directory.Exists(path))
                {
                    string[] files = Directory.GetFiles(path, "*", SearchOption.AllDirectories);
                    return files.Length;
                }
                else
                {
                    Console.WriteLine("Le dossier n'existe pas.");
                    return 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Une erreur est survenue : {ex.Message}");
                return 0;
            }
        }
    }
}
