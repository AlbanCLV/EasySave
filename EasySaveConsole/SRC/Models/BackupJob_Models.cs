using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Threading;
using Newtonsoft.Json;
using EasySaveLog;
using System.Threading.Tasks;
using EasySaveConsole.ViewModels;
using EasySave;

namespace EasySaveConsole.Models
{
    /// <summary>
    /// Enumeration representing the type of backup.
    /// </summary>


    /// <summary>
    /// Model class representing a backup job.
    /// </summary>
    public class BackupJob_Models
    {
        public string Name { get; set; }
        public string SourceDirectory { get; set; }
        public string TargetDirectory { get; set; }
        public string Type { get; set; }

        private static BackupJob_Models _instance;
        private static readonly object _lock = new object();
        private Stopwatch stopwatch = new Stopwatch();

        /// <summary>
        /// List of backup tasks.
        /// </summary>
        [JsonIgnore]
        public List<BackupJob_Models> Tasks { get; } = new List<BackupJob_Models>();
        private const string SaveFilePath = "tasks.json";
        private Log_ViewModels controller_log = new Log_ViewModels();
        private State_models _state_models = new State_models();

        // Instanciation du gestionnaire de langue (ici en français par défaut)
        private readonly LangManager lang ;

        /// <summary>
        /// Constructor for creating a backup job.
        /// </summary>

        public BackupJob_Models(string name, string sourceDirectory, string targetDirectory, string type, bool loadTasks = false)
        {
            Name = name;
            SourceDirectory = sourceDirectory;
            TargetDirectory = targetDirectory;
            Type = type;

            lang = LangManager.Instance;


            if (loadTasks)  // Load tasks if necessary
            {
                LoadTasks();
            }
        }
        public static BackupJob_Models Instance
        {
            get
            {
                // Use double-check locking to ensure thread-safety
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new BackupJob_Models("","","", "Full", true);
                        }
                    }
                }
                return _instance;
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
        public string CreateBackupTask(BackupJob_Models task)
        {
            
            // Resume and confirmation
            Console.Clear();
            Console.WriteLine(lang.Translate("task_summary"));
            Console.WriteLine($"{lang.Translate("task_name")}: {task.Name}");
            Console.WriteLine($"{lang.Translate("source_directory")}: {task.SourceDirectory}");
            Console.WriteLine($"{lang.Translate("target_directory")}: {task.TargetDirectory}");
            Console.WriteLine($"{lang.Translate("backup_type")}: {(task.Type == "Full" ? lang.Translate("full_backup") : lang.Translate("differential_backup"))}");
            Console.WriteLine($"\n{lang.Translate("confirm_task_creation")}");

            string confirmation = Console.ReadLine()?.ToUpper();
            if (confirmation == "Y")
            {
                Tasks.Add(task);
                SaveTasks();
                return "OK";
            }
            else
            {
                Console.WriteLine(lang.Translate("task_creation_canceled"));
                return "KO";
            }
        }

        /// <summary>
        /// Displays all backup tasks.
        /// </summary>
        public string ViewTasks()
        {
            if (Tasks.Count == 0)
            {
                Console.WriteLine(lang.Translate("no_backup_tasks"));
                return "OTask";
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
            return "OK";

        }

        /// <summary>
        /// Deletes a backup task.
        /// </summary>
        public (BackupJob_Models, string , string) DeleteTask()
        {
            if (Tasks.Count == 0)
            {
                Console.WriteLine(lang.Translate("no_tasks_to_delete"));
                return (null, "0Task", "-1");
            }
            BackupJob_Models deletedTask = null;

            while (true)
            {
                Console.Write(lang.Translate("enter_task_number_to_delete"));
                if (int.TryParse(Console.ReadLine(), out int taskNumber) && taskNumber > 0 && taskNumber <= Tasks.Count)
                {
                    stopwatch.Start();
                    deletedTask = Tasks[taskNumber - 1];
                    Console.WriteLine(string.Format(lang.Translate("deleting_task"), deletedTask.Name));
                    Tasks.RemoveAt(taskNumber - 1);
                    Console.WriteLine(lang.Translate("task_deleted_successfully"));
                    SaveTasks();
                    stopwatch.Stop();
                    string formattedTime = stopwatch.Elapsed.ToString(@"hh\:mm\:ss\.fff");  // Format elapsed time
                    return (deletedTask, "OK", formattedTime);
                }
                else
                {
                    Console.WriteLine(lang.Translate("invalid_task_number"));
                    return (null, "invalid", "-1");
                }
            }
        }

        /// <summary>
        /// Execute a specific task from number
        /// </summary>
        public (BackupJob_Models, string) ExecuteSpecificTask()
        {
            ViewTasks();
            if (Tasks.Count == 0)
            {
                Console.WriteLine(lang.Translate("no_tasks_to_execute"));
                return (null, "0Task");
            }
            Console.Write(lang.Translate("enter_task_number_to_execute"));
            if (int.TryParse(Console.ReadLine(), out int taskNumber) && taskNumber > 0 && taskNumber <= Tasks.Count)
            {
                BackupJob_Models selectedTask = Tasks[taskNumber - 1];
                string e = ExecuteBackup(selectedTask);
                return (selectedTask, e);
            }
             else
             {
                Console.WriteLine(lang.Translate("invalid_task_number"));
                return (null, "Invalid");
             }
           
        }

        /// <summary>
        /// Execute all tasks sequentially
        /// </summary>
        public (List<BackupJob_Models>, List<string>) ExecuteAllTasks()
        {
            List<BackupJob_Models> executedTasks = new List<BackupJob_Models>();
            List<string> logMessages = new List<string>();

            foreach (var task in Tasks)
            {
                string log = ExecuteBackup(task);
                executedTasks.Add(task);
                logMessages.Add(log);  // Store log message
            }

            return (executedTasks, logMessages);  // Return both lists
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
                _state_models.StateUpdate(task, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), targetDir);
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
            _state_models.StatEnd(task, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), targetDir);

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
                _state_models.StateUpdate(task, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), destDir);
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
            _state_models.StatEnd(task, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), destDir);

        }

        /// <summary>
        /// Execute single backup task (differential or full).
        /// </summary>
        private string ExecuteDifferentialBackup(BackupJob_Models task)
        {
            Console.WriteLine(string.Format(lang.Translate("executing_differential_backup"), task.Name));

            if (!Directory.Exists(task.SourceDirectory))
            {
                Console.WriteLine(string.Format(lang.Translate("source_directory_not_exist"), task.SourceDirectory));
                return "Source";
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
                    return "KO";
                }
            }
            // Copy only new or modified files in destination directory
            CopyModifiedFiles(task.SourceDirectory, targetPath, task);

            Console.WriteLine($"Differential backup '{task.Name}' completed successfully.");
            return "OK";
        }

        /// <summary>
        /// Execute full backup.
        /// </summary>
        private string PerformFullBackup(BackupJob_Models task)
        {
            Console.WriteLine(string.Format(lang.Translate("executing_full_backup"), task.Name));

            if (!Directory.Exists(task.SourceDirectory))
            {
                Console.WriteLine(string.Format(lang.Translate("source_directory_not_exist"), task.SourceDirectory));
                return "Source";
            }

            string sourceDirectoryName = Path.GetFileName(task.SourceDirectory.TrimEnd(Path.DirectorySeparatorChar));
            string targetPath = GetUniqueDirectoryName(task.TargetDirectory, sourceDirectoryName);
            Directory.CreateDirectory(targetPath);
            CopyDirectoryContent(task.SourceDirectory, targetPath, task);
            Console.WriteLine(string.Format(lang.Translate("full_backup_completed"), task.Name));
            return "OK";
        }

        private string ExecuteBackup(BackupJob_Models task)
        {
            if (task.Type == "Full")
            {
                return PerformFullBackup(task);
            }
            else if (task.Type == "Differential")
            {
                return ExecuteDifferentialBackup(task);
            }
            else return "KO";
        }
    }
}