using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;
using EasySaveWPF.ModelsWPF;
using Newtonsoft.Json;
using EasySaveLog;

namespace EasySaveWPF.ModelsWPF
{
    /// <summary>
    /// Manages the state logging for backup tasks.
    /// </summary>
    public class State_modelsWPF
    {
        private readonly Log_Models LogModels = new Log_Models();
        private readonly string logDirectoryState; // Directory to store log files.

        /// <summary>
        /// Initializes a new instance of the <see cref="State_modelsWPF"/> class.
        /// </summary>
        /// <param name="logDirectoryState">The directory where state logs will be saved.</param>
        public State_modelsWPF(string logDirectoryState = "States")
        {
            this.logDirectoryState = logDirectoryState;
        }

        /// <summary>
        /// Updates the state of an ongoing backup task.
        /// </summary>
        /// <param name="task">The backup task being executed.</param>
        /// <param name="lasth">The last action timestamp.</param>
        /// <param name="desti">The destination directory.</param>
        public void StateUpdate(Backup_ModelsWPF task, string lasth, string desti)
        {
            var StateEntry = new
            {
                Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
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

            string StatePath = Path.Combine(logDirectoryState, $"States_{task.Name}.json");
            Directory.CreateDirectory(logDirectoryState);
            File.AppendAllText(StatePath, JsonConvert.SerializeObject(StateEntry, Formatting.Indented) + Environment.NewLine);
        }

        /// <summary>
        /// Marks a backup task as completed and logs its final state.
        /// </summary>
        /// <param name="task">The backup task.</param>
        /// <param name="lasth">The last action timestamp.</param>
        /// <param name="desti">The destination directory.</param>
        public void StateEnd(Backup_ModelsWPF task, string lasth, string desti)
        {
            var StateEntry = new
            {
                Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
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

            string StatePath = Path.Combine(logDirectoryState, $"States_{task.Name}.json");
            Directory.CreateDirectory(logDirectoryState);
            File.AppendAllText(StatePath, JsonConvert.SerializeObject(StateEntry, Formatting.Indented) + Environment.NewLine);
        }

        /// <summary>
        /// Logs an error state for a backup task.
        /// </summary>
        /// <param name="task">The backup task.</param>
        /// <param name="lasth">The last action timestamp.</param>
        /// <param name="error">The error message.</param>
        /// <param name="desti">The destination directory.</param>
        public void StateError(Backup_ModelsWPF task, string lasth, string error, string desti)
        {
            var StateEntry = new
            {
                Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                TaskName = "Backup_" + task.Name,
                HeureLastAction = lasth,
                Etat = "Error",
                Error = error
            };

            string StatePath = Path.Combine(logDirectoryState, $"States_{task.Name}.json");
            Directory.CreateDirectory(logDirectoryState);
            File.AppendAllText(StatePath, JsonConvert.SerializeObject(StateEntry, Formatting.Indented) + Environment.NewLine);
        }

        /// <summary>
        /// Calculates the total size of a directory, including subdirectories.
        /// </summary>
        /// <param name="directory">The directory to measure.</param>
        /// <returns>The total size in bytes.</returns>
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
        /// Counts the number of files in a directory, including subdirectories.
        /// </summary>
        /// <param name="path">The directory path.</param>
        /// <returns>The number of files found.</returns>
        private static int CountFilesInDirectory(string path)
        {
            try
            {
                if (Directory.Exists(path))
                {
                    return Directory.GetFiles(path, "*", SearchOption.AllDirectories).Length;
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
