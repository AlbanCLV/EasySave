using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Newtonsoft.Json;
using EasySaveLog;
using EasySaveConsole.Controllers;

namespace EasySaveConsole.Models

{
    public class State_models
    {
        Log_Models LogModels = new Log_Models();
        private readonly string logDirectoryState; // Directory to store log files.
        private static State_models _instance;
        private static readonly object _lock = new object();

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
        /// Constructor to specify the log directory (default is "Logs").
        /// </summary>
        /// <param name="logDirectory">The directory where logs will be saved.</param>


        public State_models(string logDirectoryState = "States")
        {
            this.logDirectoryState = logDirectoryState; // Initializes the log directory.

        }


        public void StateUpdate(BackupJob_Models task, string lasth, string desti)
        {

            // Create a log entry with information about the action, timestamp, task, source, target, file size, and transfer time.
            var StateEntry = new
            {
                Timestanp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                TaskName = "Backup_" + task.Name, // Name of the backup task.
                HeureLastAction = lasth, // Format to show only date and time.
                Etat = "Active",
                TotalFile = CountFilesInDirectory(task.SourceDirectory),
                TotalSize = GetDirectorySize(new DirectoryInfo(task.SourceDirectory)) / 1024.0,
                Progress = $"{((GetDirectorySize(new DirectoryInfo(desti)) / 1024.0) * 100) / (GetDirectorySize(new DirectoryInfo(task.SourceDirectory)) / 1024.0)} %",
                RemainingFiles = CountFilesInDirectory(task.SourceDirectory) - CountFilesInDirectory(desti),
                RemainingSize = (GetDirectorySize(new DirectoryInfo(task.SourceDirectory)) / 1024.0) - (GetDirectorySize(new DirectoryInfo(desti)) / 1024.0),
                CurrentSourceFile = task.SourceDirectory,
                CurrentDestinationFiles = desti
            };

            // Create the log file path based on the current date.
            string StatePath = Path.Combine(logDirectoryState, $"Sates .json");

            // Ensure that the log directory exists, create it if necessary.
            Directory.CreateDirectory(logDirectoryState);

            // Append the log entry to the log file as a JSON object, with proper formatting and a newline.
            File.AppendAllText(StatePath, JsonConvert.SerializeObject(StateEntry, Formatting.Indented) + Environment.NewLine);
        }
        public void StatEnd(BackupJob_Models task, string lasth, string desti)
        {

            // Create a log entry with information about the action, timestamp, task, source, target, file size, and transfer time.
            var StateEntry = new
            {
                Timestanp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                TaskName = "Backup_" + task.Name, // Name of the backup task.
                HeureLastAction = lasth, // Format to show only date and time.
                Etat = "Completed",
                TotalFileSource = CountFilesInDirectory(task.SourceDirectory),
                TotalFileTarget = CountFilesInDirectory(desti),
                TotalSizeSource = GetDirectorySize(new DirectoryInfo(task.SourceDirectory)) / 1024.0,
                TotalSizeTarget = GetDirectorySize(new DirectoryInfo(desti)) / 1024.0,
                Progress = $"100 %",
                RemainingFiles = CountFilesInDirectory(task.SourceDirectory) - CountFilesInDirectory(desti),
                RemainingSize = (GetDirectorySize(new DirectoryInfo(task.SourceDirectory)) / 1024.0) - (GetDirectorySize(new DirectoryInfo(desti)) / 1024.0),
                CurrentSourceFile = task.SourceDirectory,
                CurrentDestinationFiles = desti
            };

            // Create the log file path based on the current date.
            string StatePath = Path.Combine(logDirectoryState, $"Sates .json");

            // Ensure that the log directory exists, create it if necessary.
            Directory.CreateDirectory(logDirectoryState);

            // Append the log entry to the log file as a JSON object, with proper formatting and a newline.
            File.AppendAllText(StatePath, JsonConvert.SerializeObject(StateEntry, Formatting.Indented) + Environment.NewLine);
        }
        public void StateError(BackupJob_Models task, string lasth, string error, string desti)
        {
            var StateEntry = new
            {
                Timestanp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                TaskName = "Backup_" + task.Name, // Name of the backup task.
                HeureLastAction = lasth,
                Etat = "Error",
                Error = error,
            };

            // Create the log file path based on the current date.
            string StatePath = Path.Combine(logDirectoryState, $"Sates .json");

            // Ensure that the log directory exists, create it if necessary.
            Directory.CreateDirectory(logDirectoryState);

            // Append the log entry to the log file as a JSON object, with proper formatting and a newline.
            File.AppendAllText(StatePath, JsonConvert.SerializeObject(StateEntry, Formatting.Indented) + Environment.NewLine);
        }



        private long GetDirectorySize(DirectoryInfo directory)
        {
            if (!directory.Exists)
            {
                LogModels.LogErreurJSON("Error", "try to delete a task", "Folder not found");
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