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
using EasySaveConsole.Views;

namespace EasySaveConsole.Models
{
    /// <summary>
    /// Enumeration representing the type of backup.
    /// </summary>


    /// <summary>
    /// Model class representing a backup job.
    /// </summary>
    public class Backup_Models
    {
        public string Name { get; set; }
        public string SourceDirectory { get; set; }
        public string TargetDirectory { get; set; }
        public string Type { get; set; }

        private static Backup_Models _instance;
        private static readonly object _lock = new object();
        private Stopwatch stopwatch = new Stopwatch();

        /// <summary>
        /// List of backup tasks.
        /// </summary>
        [JsonIgnore]
        public List<Backup_Models> Tasks { get; } = new List<Backup_Models>();
        private const string SaveFilePath = "tasks.json";
        private Log_ViewModels controller_log = new Log_ViewModels();
        private State_models _state_models = new State_models();
        private ProcessWatcher processWatcher;
        // Instanciation du gestionnaire de langue (ici en français par défaut)
        private readonly LangManager lang ;

        /// <summary>
        /// Constructor for creating a backup job.
        /// </summary>

        public Backup_Models(string name, string sourceDirectory, string targetDirectory, string type, bool loadTasks = false)
        {
            Name = name;
            SourceDirectory = sourceDirectory;
            TargetDirectory = targetDirectory;
            Type = type;

            lang = LangManager.Instance;
            processWatcher = ProcessWatcher.Instance;

            if (loadTasks)  // Load tasks if necessary
            {
                LoadTasks();
            }
        }
        public static Backup_Models Instance
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
                            _instance = new Backup_Models("","","", "Full", true);
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
                    var loadedTasks = JsonConvert.DeserializeObject<List<Backup_Models>>(json);

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
        public string CreateBackupTask(Backup_Models task, string confirmation)
        {          
            if (confirmation == "Y" || confirmation == "O")
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
        public (Backup_Models, string , string) DeleteTask(int taskNumber)
        {
            if (Tasks.Count == 0)
            {
                return (null, "0Task", "-1");
            }
            Backup_Models deletedTask = null;
            if (taskNumber > Tasks.Count) { return (deletedTask, "Invalid", "0"); }
           stopwatch.Start();
           deletedTask = Tasks[taskNumber - 1];
           Tasks.RemoveAt(taskNumber - 1);
           SaveTasks();
           stopwatch.Stop();
           string formattedTime = stopwatch.Elapsed.ToString(@"hh\:mm\:ss\.fff");  // Format elapsed time
           return (deletedTask, "OK", formattedTime);
               
        
        }

        

        /// <summary>
        /// Execute a specific task from number
        /// </summary>
        public (Backup_Models, string) ExecuteSpecificTask(int taskNumber)
        {
            ViewTasks();
            if (Tasks.Count == 0)
            {
                return (null, "0Task");
            }

            if (taskNumber > Tasks.Count) { return (null, "Invalid"); }
           Backup_Models selectedTask = Tasks[taskNumber - 1];
           string e = ExecuteBackup(selectedTask);
           return (selectedTask, e);
           
        }

        /// <summary>
        /// Execute all tasks sequentially
        /// </summary>
        public (List<Backup_Models>, List<string>) ExecuteAllTasks()
        {
            List<Backup_Models> executedTasks = new List<Backup_Models>();
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
        private void CopyDirectoryContent(string sourceDir, string targetDir, Backup_Models task)
        {
            foreach (string file in Directory.GetFiles(sourceDir, "*", SearchOption.TopDirectoryOnly))
            {
                if (processWatcher.IsBusinessApplicationRunning()) 
                {
                    _state_models.StateError(task, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "BusinessApllicationRunning");
                    return; 
                }
                _state_models.StateUpdate(task, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), targetDir);
                string fileName = Path.GetFileName(file);
                string destinationFile = Path.Combine(targetDir, fileName);
                File.Copy(file, destinationFile, true);
            }

            foreach (string directory in Directory.GetDirectories(sourceDir, "*", SearchOption.TopDirectoryOnly))
            {
if (processWatcher.IsBusinessApplicationRunning()) 
                {
                    _state_models.StateError(task, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "BusinessApllicationRunning");
                    return; 
                }                string directoryName = Path.GetFileName(directory);
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
        private void CopyModifiedFiles(string sourceDir, string destDir, Backup_Models task)
        {
            foreach (string sourceFile in Directory.GetFiles(sourceDir, "*", SearchOption.AllDirectories))
            {
                if (processWatcher.IsBusinessApplicationRunning()) 
                {
                    _state_models.StateError(task, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "BusinessApllicationRunning");
                    return; 
                }
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
        private string ExecuteDifferentialBackup(Backup_Models task)
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
        private string PerformFullBackup(Backup_Models task)
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

        private string ExecuteBackup(Backup_Models task)
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