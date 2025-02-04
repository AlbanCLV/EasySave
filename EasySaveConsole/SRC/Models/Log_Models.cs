using System;
using System.IO;
using System.Diagnostics;  // Pour utiliser Stopwatch
using EasySave.Models;
using Newtonsoft.Json;

namespace EasySave
{
    /// <summary>
    /// Model for managing log entries related to backup actions.
    /// </summary>
    public class Log_Models
    {
        private readonly string logDirectory; // Directory to store log files.

        /// <summary>
        /// Constructor to specify the log directory (default is "Logs").
        /// </summary>
        /// <param name="logDirectory">The directory where logs will be saved.</param>
        public Log_Models(string logDirectory = "Logs")
        {
            this.logDirectory = logDirectory; // Initializes the log directory.
        }

        /// <summary>
        /// Creates a new log entry for a backup action, with details about the task, source, and destination.
        /// </summary>
        /// <param name="task">The backup job task object containing task details.</param>
        /// <param name="act">The action performed (e.g., "Started", "Completed").</param>
        public void LogAction(BackupJob_Models task, string time ,String act)
        {
            // Initialize variable to store the file size (default is 0).
            long fileSize = 0;

            // calculate the total size of files within it (including subdirectories).
            fileSize = GetDirectorySize(new DirectoryInfo(task.SourceDirectory));
            double fileSizeInKB = fileSize / 1024.0;
            // Create a log entry with information about the action, timestamp, task, source, target, file size, and transfer time.
            var logEntry = new
            {
                Action = act, // The action performed (e.g., "Backup Started").
                Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), // Format to show only date and time.
                TaskName = "Backup_" + task.Name, // Name of the backup task.
                SourceFile = task.SourceDirectory, // Path of the source file or directory.
                TargetFile = task.TargetDirectory, // Path of the target file or directory.
                FileSize = fileSizeInKB, // Size of the source file/directory.
                TransferTimeMs = time // Placeholder for transfer time (currently not used).
            };

            // Create the log file path based on the current date.
            string logPath = Path.Combine(logDirectory, $"{DateTime.Now:yyyy-MM-dd}.json");

            // Ensure that the log directory exists, create it if necessary.
            Directory.CreateDirectory(logDirectory);

            // Append the log entry to the log file as a JSON object, with proper formatting and a newline.
            File.AppendAllText(logPath, JsonConvert.SerializeObject(logEntry, Formatting.Indented) + Environment.NewLine);
        }

        /// <summary>
        /// Calculates the total size of all files in a directory and its subdirectories.
        /// </summary>
        /// <param name="directory">The directory to calculate the size of.</param>
        /// <returns>The total size of all files in the directory in bytes.</returns>
        private long GetDirectorySize(DirectoryInfo directory)
        {
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
