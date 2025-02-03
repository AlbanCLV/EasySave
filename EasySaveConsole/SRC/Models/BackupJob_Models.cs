using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;  // Assurez-vous que cette directive est présente
using Terminal.Gui;

namespace EasySave.Models
{
    /// <summary>
    /// Enumeration representing the type of backup.
    /// </summary>
    public enum BackupType
    {
        Full,
        Differential
    }

    /// <summary>
    /// Model class representing a backup job.
    /// </summary>
    public class BackupJob_Models
    {
        public string Name { get; set; }
        public string SourceDirectory { get; set; }
        public string TargetDirectory { get; set; }
        public BackupType Type { get; set; }


        /// <summary>
        /// List of backup tasks.
        /// </summary>
        [JsonIgnore]
        public List<BackupJob_Models> Tasks { get; } = new List<BackupJob_Models>();
        private readonly Log_Models logger = new Log_Models();
        private readonly State_Models stateManager = new State_Models();
        private const string SaveFilePath = "tasks.json";

        /// <summary>
        /// Constructor for creating a backup job.
        /// </summary>
        public BackupJob_Models(string name, string sourceDirectory, string targetDirectory, BackupType type, bool loadTasks = false)
        {
            Name = name;
            SourceDirectory = sourceDirectory;
            TargetDirectory = targetDirectory;
            Type = type;

            if (loadTasks)  // Load tasks if necessary
            {
                LoadTasks();
            }
        }


        /// <summary>
        /// Saves tasks to a JSON file.
        /// </summary>
        public void SaveTasks()
        {
            try
            {
                // Convert tasks list to a JSON-formatted string
                string json = JsonConvert.SerializeObject(Tasks, Formatting.Indented);

                // Write the JSON string to the file
                File.WriteAllText(SaveFilePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving tasks: {ex.Message}");
            }
        }

        /// <summary>
        /// Loads tasks from a JSON file.
        /// </summary>
        public void LoadTasks()
        {
            try
            {
                if (File.Exists(SaveFilePath))
                {
                    string json = File.ReadAllText(SaveFilePath);
                    var loadedTasks = JsonConvert.DeserializeObject<List<BackupJob_Models>>(json);

                    if (loadedTasks != null)
                    {
                        Tasks.Clear();
                        Tasks.AddRange(loadedTasks);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading tasks: {ex.Message}");
            }
        }


        /// <summary>
        /// Creates a new backup task.
        /// </summary>
        public void CreateBackupTask(BackupJob_Models task)
        {
            if (Tasks.Count >= 5)
            {
                Console.WriteLine("You cannot create more than 5 backup tasks.");
                return;
            }
            // Resume and confirmation
            Console.Clear();
            Console.WriteLine("=== Task Summary ===");
            Console.WriteLine($"Task Name: {task.Name}");
            Console.WriteLine($"Source Directory: {task.SourceDirectory}");
            Console.WriteLine($"Destination Directory: {task.TargetDirectory}");
            Console.WriteLine($"Backup Type: {(task.Type == BackupType.Full ? "Full" : "Differential")}");
            Console.WriteLine("\nConfirm task creation? (Y/N): ");

            string confirmation = Console.ReadLine()?.ToUpper();
            if (confirmation == "Y")
            {
                Tasks.Add(task);
                Console.WriteLine("Task created successfully!");
                SaveTasks();
            }
            else
            {
                Console.WriteLine("Task creation canceled. Returning to menu...");
            }
        }


        /// <summary>
        /// Displays all backup tasks.
        /// </summary>
        public void ViewTasks()
        {
            if (Tasks.Count == 0)
            {
                Console.WriteLine("No backup tasks created yet.");
                return;
            }

            Console.Clear();
            Console.WriteLine("Existing Backup Tasks:");
            for (int i = 0; i < Tasks.Count; i++)
            {
                Console.WriteLine($"{i + 1}. Name: {Tasks[i].Name}. \nSource: {Tasks[i].SourceDirectory}. \nTarget: {Tasks[i].TargetDirectory}. \nType: {Tasks[i].Type}.  \n");
            }
        }

        /// <summary>
        /// Deletes a backup task.
        /// </summary>
        public void DeleteTask()
        {
            ViewTasks();
            if (Tasks.Count == 0) return;
            Console.Write("Enter the number of the task to delete: ");
            if (int.TryParse(Console.ReadLine(), out int taskNumber) && taskNumber > 0 && taskNumber <= Tasks.Count)
            {
                Console.WriteLine($"Deleting task '{Tasks[taskNumber - 1].Name}'...");
                Tasks.RemoveAt(taskNumber - 1);
                Console.WriteLine("Task deleted successfully.");
                SaveTasks();
            }
            else
            {
                Console.WriteLine("Invalid task number.");
            }
        }

        /// <summary>
        /// Execute a specific task from number
        /// </summary>
        public void ExecuteSpecificTask()
        {
            ViewTasks();
            if (Tasks.Count == 0)
            {
                Console.WriteLine("No tasks available to execute.");
                return;
            }
            Console.Write("Enter the number of the task to execute: ");
            if (int.TryParse(Console.ReadLine(), out int taskNumber) && taskNumber > 0 && taskNumber <= Tasks.Count)
            {
                ExecuteBackup(Tasks[taskNumber - 1]);
            }
            else
            {
                Console.WriteLine("Invalid task number.");
            }
        }

        /// <summary>
        /// Execute all tasks sequentially
        /// </summary>
        public void ExecuteAllTasks()
        {
            foreach (var task in Tasks)
            {
                ExecuteBackup(task);
            }
        }
        private string GetUniqueDirectoryName(string destinationPath, string sourceDirectoryName)
        {
            string uniqueName = sourceDirectoryName;
            int counter = 1;

            // loop to find a unique name
            while (Directory.Exists(Path.Combine(destinationPath, uniqueName)))
            {
                uniqueName = $"{sourceDirectoryName} ({counter})";
                counter++;
            }

            return Path.Combine(destinationPath, uniqueName);
        }
        private void CopyDirectoryContent(string sourceDir, string targetDir)
        {
            // Copy all files from source directory
            foreach (string file in Directory.GetFiles(sourceDir, "*", SearchOption.TopDirectoryOnly))
            {
                string fileName = Path.GetFileName(file);
                string destinationFile = Path.Combine(targetDir, fileName);
                File.Copy(file, destinationFile, true); // Overwrite if file already exist
            }

            // Copy folders of source directory
            foreach (string directory in Directory.GetDirectories(sourceDir, "*", SearchOption.TopDirectoryOnly))
            {
                string directoryName = Path.GetFileName(directory);
                string destinationSubDir = Path.Combine(targetDir, directoryName);
                Directory.CreateDirectory(destinationSubDir);
                CopyDirectoryContent(directory, destinationSubDir); // Recursive call to copy all subfolders
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceDir">Source directory from which files will be copied</param>
        /// <param name="destDir">Destination directory where files will be copied</param>
        private void CopyModifiedFiles(string sourceDir, string destDir)
        {
            foreach (string sourceFile in Directory.GetFiles(sourceDir, "*", SearchOption.AllDirectories))
            {
                // Get relative path of source directory
                string relativePath = PathHelper.GetRelativePath(sourceDir, sourceFile);

                // Build full path in destination directory
                string destFile = Path.Combine(destDir, relativePath);

                // Verify if file was modified or already exist
                if (!File.Exists(destFile) || File.GetLastWriteTime(sourceFile) > File.GetLastWriteTime(destFile))
                {
                    // Create sub folders in destination directory
                    Directory.CreateDirectory(Path.GetDirectoryName(destFile));

                    // Copy file or overwrite it
                    File.Copy(sourceFile, destFile, true);

                    Console.WriteLine($"File updated or added: {destFile}");
                }
            }
        }
        /// <summary>
        /// Execute single backup task
        /// </summary>
        /// <param name="task">BackupJob_Models object task</param>
        private void ExecuteDifferentialBackup(BackupJob_Models task)
        {
            Console.WriteLine($"Executing differential backup: {task.Name}");

            // Verify if source exist
            if (!Directory.Exists(task.SourceDirectory))
            {
                Console.WriteLine($"Source directory '{task.SourceDirectory}' does not exist.");
                return;
            }

            // Verify if directory exist
            string sourceDirectoryName = Path.GetFileName(task.SourceDirectory.TrimEnd(Path.DirectorySeparatorChar));
            string targetPath = Path.Combine(task.TargetDirectory, sourceDirectoryName);

            if (!Directory.Exists(targetPath))
            {
                Console.WriteLine($"The destination directory does not contain a backup of '{sourceDirectoryName}'.");
                Console.Write("Do you want to perform a full backup instead? (Y/N): ");
                string input = Console.ReadLine()?.ToUpper();

                if (input == "Y")
                {
                    PerformFullBackup(task);
                }
                else
                {
                    Console.WriteLine("Differential backup canceled.");
                }
                return;
            }
            // Copy only new or modified files in destination directory
            CopyModifiedFiles(task.SourceDirectory, targetPath);

            Console.WriteLine($"Differential backup '{task.Name}' completed successfully.");
        }

        /// <summary>
        /// Do a full backup Job
        /// </summary>
        /// <param name="task">BackupJob_Models task object</param>
        private void PerformFullBackup(BackupJob_Models task)
        {
            Console.WriteLine($"Executing full backup: {task.Name}");

            // Verify if source exist
            if (!Directory.Exists(task.SourceDirectory))
            {
                Console.WriteLine($"Source directory '{task.SourceDirectory}' does not exist.");
                return;
            }

            // Create unqiue folder for backup
            string sourceDirectoryName = Path.GetFileName(task.SourceDirectory.TrimEnd(Path.DirectorySeparatorChar));
            string targetPath = GetUniqueDirectoryName(task.TargetDirectory, sourceDirectoryName);

            // Create destination folder
            Directory.CreateDirectory(targetPath);

            // Copy source files into destination directory
            CopyDirectoryContent(task.SourceDirectory, targetPath);
            Console.WriteLine($"Full backup '{task.Name}' completed successfully.");
        }

        private void ExecuteBackup(BackupJob_Models task)
        {
            if (task.Type == BackupType.Full)
            {
                Console.WriteLine($"Performing full backup: {task.Name}");
                PerformFullBackup(task);
            }
            else if (task.Type == BackupType.Differential)
            {
                ExecuteDifferentialBackup(task);
            }
        }
    }
}
