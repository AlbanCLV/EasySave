using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using EasySave.Controllers;
using EasySave.Utilities; // Ajout de la référence au gestionnaire de langue
using Newtonsoft.Json;
using EasySaveLog;

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
        private const string SaveFilePath = "tasks.json";
        private Log_Controller controller_log = new Log_Controller();
        private State_Controller controller_State = new State_Controller();

        // Instanciation du gestionnaire de langue (ici en français par défaut)
        private readonly LangManager lang;

        /// <summary>
        /// Constructor for creating a backup job.
        /// </summary>
        public BackupJob_Models(string name, string sourceDirectory, string targetDirectory, BackupType type, bool loadTasks = false)
        {
            Name = name;
            SourceDirectory = sourceDirectory;
            TargetDirectory = targetDirectory;
            Type = type;

            lang = new LangManager(Program.SelectedLanguage);


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
                string json = JsonConvert.SerializeObject(Tasks, Formatting.Indented);
                File.WriteAllText(SaveFilePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{lang.Translate("error_saving_tasks")}: {ex.Message}");
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
                Console.WriteLine($"{lang.Translate("error_loading_tasks")}: {ex.Message}");
            }
        }

        /// <summary>
        /// Creates a new backup task.
        /// </summary>
        public void CreateBackupTask(BackupJob_Models task)
        {
            if (Tasks.Count >= 5)
            {
                Console.WriteLine(lang.Translate("max_tasks_error"));
                controller_log.LogBackupErreur(task.Name, lang.Translate("create_task_attempt"), lang.Translate("max_tasks_error"));
                Console.WriteLine(lang.Translate("app_closing_in"));
                for (int i = 5; i > 0; i--)
                {
                    Console.WriteLine(i);
                    Thread.Sleep(1000);
                }
                Environment.Exit(0);
                return;
            }
            // Resume and confirmation
            Console.Clear();
            Console.WriteLine(lang.Translate("task_summary"));
            Console.WriteLine($"{lang.Translate("task_name")}: {task.Name}");
            Console.WriteLine($"{lang.Translate("source_directory")}: {task.SourceDirectory}");
            Console.WriteLine($"{lang.Translate("target_directory")}: {task.TargetDirectory}");
            Console.WriteLine($"{lang.Translate("backup_type")}: {(task.Type == BackupType.Full ? lang.Translate("full_backup") : lang.Translate("differential_backup"))}");
            Console.WriteLine($"\n{lang.Translate("confirm_task_creation")}");

            string confirmation = Console.ReadLine()?.ToUpper();
            if (confirmation == "Y")
            {
                Tasks.Add(task);
                SaveTasks();
            }
            else
            {
                Console.WriteLine(lang.Translate("task_creation_canceled"));
            }
        }

        /// <summary>
        /// Displays all backup tasks.
        /// </summary>
        public void ViewTasks()
        {
            if (Tasks.Count == 0)
            {
                Console.WriteLine(lang.Translate("no_backup_tasks"));
                return;
            }

            Console.Clear();
            Console.WriteLine(lang.Translate("existing_backup_tasks"));
            for (int i = 0; i < Tasks.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {lang.Translate("task_details")}:");
                Console.WriteLine($"{lang.Translate("task_name")}: {Tasks[i].Name}");
                Console.WriteLine($"{lang.Translate("source_directory")}: {Tasks[i].SourceDirectory}");
                Console.WriteLine($"{lang.Translate("target_directory")}: {Tasks[i].TargetDirectory}");
                Console.WriteLine($"{lang.Translate("backup_type")}: {Tasks[i].Type}");
                Console.WriteLine();
            }
        }

        /// <summary>
        /// Deletes a backup task.
        /// </summary>
        public BackupJob_Models DeleteTask()
        {
            ViewTasks();

            if (Tasks.Count == 0)
            {
                Console.WriteLine(lang.Translate("no_tasks_to_delete"));
                controller_log.LogBackupErreur("Error", lang.Translate("delete_task_attempt"), lang.Translate("no_tasks_to_delete"));
                Environment.Exit(0);
                return null;
            }

            BackupJob_Models deletedTask = null;

            while (true)
            {
                Console.Write(lang.Translate("enter_task_number_to_delete"));
                if (int.TryParse(Console.ReadLine(), out int taskNumber) && taskNumber > 0 && taskNumber <= Tasks.Count)
                {
                    deletedTask = Tasks[taskNumber - 1];
                    Console.WriteLine(string.Format(lang.Translate("deleting_task"), deletedTask.Name));
                    Tasks.RemoveAt(taskNumber - 1);
                    Console.WriteLine(lang.Translate("task_deleted_successfully"));
                    SaveTasks();
                    break;
                }
                else
                {
                    Console.WriteLine(lang.Translate("invalid_task_number"));
                    controller_log.LogBackupErreur("Error", lang.Translate("delete_task_attempt"), lang.Translate("invalid_task_number"));
                }
            }
            return deletedTask;
        }

        /// <summary>
        /// Execute a specific task from number
        /// </summary>
        public BackupJob_Models ExecuteSpecificTask()
        {
            ViewTasks();
            if (Tasks.Count == 0)
            {
                Console.WriteLine(lang.Translate("no_tasks_to_execute"));
                controller_log.LogBackupErreur("error", lang.Translate("execute_task_attempt"), lang.Translate("no_tasks_to_execute"));
                Environment.Exit(0);
                return null;
            }
            Console.Write(lang.Translate("enter_task_number_to_execute"));
            while (true)
            {
                if (int.TryParse(Console.ReadLine(), out int taskNumber) && taskNumber > 0 && taskNumber <= Tasks.Count)
                {
                    BackupJob_Models selectedTask = Tasks[taskNumber - 1];
                    ExecuteBackup(selectedTask);
                    return selectedTask;
                }
                else
                {
                    Console.WriteLine(lang.Translate("invalid_task_number"));
                    controller_log.LogBackupErreur("Error", lang.Translate("execute_task_attempt"), lang.Translate("invalid_task_number"));
                    Console.Write(lang.Translate("enter_task_number_to_execute"));
                }
            }
        }

        /// <summary>
        /// Execute all tasks sequentially
        /// </summary>
        public List<BackupJob_Models> ExecuteAllTasks()
        {
            List<BackupJob_Models> executedTasks = new List<BackupJob_Models>();

            foreach (var task in Tasks)
            {
                ExecuteBackup(task);
                executedTasks.Add(task);
            }

            return executedTasks;
        }

        private string GetUniqueDirectoryName(string destinationPath, string sourceDirectoryName)
        {
            string uniqueName = sourceDirectoryName;
            int counter = 1;

            while (Directory.Exists(Path.Combine(destinationPath, uniqueName)))
            {
                uniqueName = $"{sourceDirectoryName} ({counter})";
                counter++;
            }

            return Path.Combine(destinationPath, uniqueName);
        }
        private void CopyDirectoryContent(string sourceDir, string targetDir, BackupJob_Models task)
        {
            foreach (string file in Directory.GetFiles(sourceDir, "*", SearchOption.TopDirectoryOnly))
            {
                controller_State.StateUpdate(task, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), targetDir);
                string fileName = Path.GetFileName(file);
                string destinationFile = Path.Combine(targetDir, fileName);
                File.Copy(file, destinationFile, true);
            }

            foreach (string directory in Directory.GetDirectories(sourceDir, "*", SearchOption.TopDirectoryOnly))
            {
                string directoryName = Path.GetFileName(directory);
                string destinationSubDir = Path.Combine(targetDir, directoryName);
                Directory.CreateDirectory(destinationSubDir);
                CopyDirectoryContent(directory, destinationSubDir, task); // Recursive call to copy all subfolders
            }
            controller_State.StatEnd(task, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), targetDir);

        }

        /// <summary>
        /// Copies only new or modified files.
        /// </summary>
        /// <param name="sourceDir">Source directory from which files will be copied</param>
        /// <param name="destDir">Destination directory where files will be copied</param>
        private void CopyModifiedFiles(string sourceDir, string destDir, BackupJob_Models task)
        {
            foreach (string sourceFile in Directory.GetFiles(sourceDir, "*", SearchOption.AllDirectories))
            {
                controller_State.StateUpdate(task, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), destDir);
                // Get relative path of source directory
                string relativePath = PathHelper.GetRelativePath(sourceDir, sourceFile);
                string destFile = Path.Combine(destDir, relativePath);

                if (!File.Exists(destFile) || File.GetLastWriteTime(sourceFile) > File.GetLastWriteTime(destFile))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(destFile));
                    File.Copy(sourceFile, destFile, true);
                    Console.WriteLine(string.Format(lang.Translate("file_updated_or_added"), destFile));
                }
            }
            controller_State.StatEnd(task, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), destDir);

        }

        /// <summary>
        /// Execute single backup task (differential or full).
        /// </summary>
        private void ExecuteDifferentialBackup(BackupJob_Models task)
        {
            Console.WriteLine(string.Format(lang.Translate("executing_differential_backup"), task.Name));

            if (!Directory.Exists(task.SourceDirectory))
            {
                Console.WriteLine(string.Format(lang.Translate("source_directory_not_exist"), task.SourceDirectory));
                controller_log.LogBackupErreur(task.Name, lang.Translate("execute_task_attempt"), string.Format(lang.Translate("source_directory_not_exist"), task.SourceDirectory));
                Environment.Exit(0);
                return;
            }

            string sourceDirectoryName = Path.GetFileName(task.SourceDirectory.TrimEnd(Path.DirectorySeparatorChar));
            string targetPath = Path.Combine(task.TargetDirectory, sourceDirectoryName);

            if (!Directory.Exists(targetPath))
            {
                Console.WriteLine(string.Format(lang.Translate("destination_no_backup"), sourceDirectoryName));
                Console.Write(lang.Translate("full_backup_instead_prompt"));
                string input = Console.ReadLine()?.ToUpper();

                if (input == "Y")
                {
                    PerformFullBackup(task);
                }
                else
                {
                    Console.WriteLine(lang.Translate("differential_backup_canceled"));
                }
                return;
            }
            // Copy only new or modified files in destination directory
            CopyModifiedFiles(task.SourceDirectory, targetPath, task);

            Console.WriteLine($"Differential backup '{task.Name}' completed successfully.");
        }

        /// <summary>
        /// Execute full backup.
        /// </summary>
        private void PerformFullBackup(BackupJob_Models task)
        {
            Console.WriteLine(string.Format(lang.Translate("executing_full_backup"), task.Name));

            if (!Directory.Exists(task.SourceDirectory))
            {
                Console.WriteLine(string.Format(lang.Translate("source_directory_not_exist"), task.SourceDirectory));
                controller_log.LogBackupErreur(task.Name, lang.Translate("execute_task_attempt"), string.Format(lang.Translate("source_directory_not_exist"), task.SourceDirectory));
                Environment.Exit(0);
                return;
            }

            string sourceDirectoryName = Path.GetFileName(task.SourceDirectory.TrimEnd(Path.DirectorySeparatorChar));
            string targetPath = GetUniqueDirectoryName(task.TargetDirectory, sourceDirectoryName);
            Directory.CreateDirectory(targetPath);
            CopyDirectoryContent(task.SourceDirectory, targetPath, task);
            Console.WriteLine(string.Format(lang.Translate("full_backup_completed"), task.Name));
        }

        private void ExecuteBackup(BackupJob_Models task)
        {
            if (task.Type == BackupType.Full)
            {
                PerformFullBackup(task);
            }
            else if (task.Type == BackupType.Differential)
            {
                ExecuteDifferentialBackup(task);
            }
        }
    }
}