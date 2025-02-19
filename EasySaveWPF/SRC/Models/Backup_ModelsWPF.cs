using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using EasySaveWPF.ViewModelsWPF;
using Newtonsoft.Json;
using EasySaveLog;
using EasySaveWPF;
using EasySave.Models;
using EasySaveConsole.Models;

namespace EasySaveWPF.ModelsWPF
{
    /// <summary>
    /// Enumeration representing the type of backup.
    /// </summary>


    /// <summary>
    /// Model class representing a backup job.
    /// </summary>
    public class Backup_ModelsWPF
    {
        public string Name { get; set; }
        public string SourceDirectory { get; set; }
        public string TargetDirectory { get; set; }
        public string Type { get; set; }

        /// <summary>
        /// List of backup tasks.
        /// </summary>
        [JsonIgnore]
        private readonly Log_Models logger = new Log_Models();
        private readonly State_modelsWPF state = new State_modelsWPF();
        public const string SaveFilePath = "tasks.json";
        private Log_ViewModels controller_log = new Log_ViewModels();
        public List<Backup_ModelsWPF> Tasks { get; private set; } = new List<Backup_ModelsWPF>();

        // Instanciation du gestionnaire de langue (ici en français par défaut)

        /// <summary>
        /// Constructor for creating a backup job.
        /// </summary>

        public Backup_ModelsWPF(string name, string sourceDirectory, string targetDirectory, string type, bool loadTasks = false)
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
        public string SaveTasks()
        {
            try
            {
                string json = JsonConvert.SerializeObject(Tasks, Formatting.Indented);
                File.WriteAllText(SaveFilePath, json);
                return "The task has been created and saved.";
            }
            catch (Exception ex)
            {
                return $"Error Saving Task(s): {ex.Message}";
            }
        }

        /// <summary>
        /// Loads tasks from a JSON file.
        /// </summary>
        public string LoadTasks()
        {
            try
            {
                if (File.Exists(SaveFilePath))
                {
                    string json = File.ReadAllText(SaveFilePath);
                    var loadedTasks = JsonConvert.DeserializeObject<List<Backup_ModelsWPF>>(json);
                    if (loadedTasks != null)
                    {
                        Tasks.Clear();
                        Tasks.AddRange(loadedTasks);
                    }
                }
                return "OK";
            }
            catch (Exception ex)
            {
                return $"Error Loading Task(s): {ex.Message}";
            }
        }


        /// <summary>
        /// Creates a new backup task.
        /// </summary>
        public string CreateBackupTask(Backup_ModelsWPF task)
        {
            if (Tasks.Count >= 5)
            {
                string t = controller_log.Get_Type_File();
                controller_log.LogBackupErreur(task.Name, "create_task_attempt", "max_tasks_error", "-1");  // Log the action
                return "Max Tasks Error";
            }
            Tasks.Add(task);
            return SaveTasks();

        }
        public string DeleteTaskWPF(Backup_ModelsWPF task)
        {
            Tasks.Remove(task);
            SaveTasks();
            return "task delete";
        }
        public string ExecuteSpecificTasks(Backup_ModelsWPF task)
        {
            if (task.Type == "Full")
            {
                return PerformFullBackup(task);
            }
            else if (task.Type == "Differential")
            {
                return ExecuteDifferentialBackup(task);
            }
            return "";
        }

        public string PerformFullBackup(Backup_ModelsWPF task)
        {
            if (!Directory.Exists(task.SourceDirectory))
            {
                controller_log.LogBackupErreur(task.Name, "execute_task_attempt", "source_directory_not_exist", "-1");
                return $"Le dossier Source n'est pas disponible ou n'éxiste pas : {task.SourceDirectory} ";
            }

            string sourceDirectoryName = Path.GetFileName(task.SourceDirectory.TrimEnd(Path.DirectorySeparatorChar));
            string targetPath = GetUniqueDirectoryName(task.TargetDirectory, sourceDirectoryName);
            Directory.CreateDirectory(targetPath);
            CopyDirectoryContent(task.SourceDirectory, targetPath, task);
            return $"La tache à Bien été éxécuter : {task.Name}";
        }
        public string ExecuteDifferentialBackup(Backup_ModelsWPF task)
        {
            if (!Directory.Exists(task.SourceDirectory))
            {
                string t = controller_log.Get_Type_File();
                controller_log.LogBackupErreur(task.Name, "execute_task_attempt", "source_directory_not_exist", "-1");
                return $"Le dossier Source n'est pas disponible ou n'éxiste pas : {task.SourceDirectory} ";
            }
            string sourceDirectoryName = Path.GetFileName(task.SourceDirectory.TrimEnd(Path.DirectorySeparatorChar));
            string targetPath = Path.Combine(task.TargetDirectory, sourceDirectoryName);
            if (!Directory.Exists(targetPath))
            {
                PerformFullBackup(task);
            }
            CopyModifiedFiles(task.SourceDirectory, targetPath, task);

            return $"La tache à Bien été éxécuter : {task.Name}";
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
        private void CopyDirectoryContent(string sourceDir, string targetDir, Backup_ModelsWPF task)
        {
            foreach (string file in Directory.GetFiles(sourceDir, "*", SearchOption.TopDirectoryOnly))
            {
                state.StateUpdate(task, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), targetDir);
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
            state.SatetEnd(task, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), targetDir);

        }
        private void CopyModifiedFiles(string sourceDir, string destDir, Backup_ModelsWPF task)
        {
            foreach (string sourceFile in Directory.GetFiles(sourceDir, "*", SearchOption.AllDirectories))
            {
                state.StateUpdate(task, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), destDir);
                // Get relative path of source directory
                string relativePath = PathHelper.GetRelativePath(sourceDir, sourceFile);
                string destFile = Path.Combine(destDir, relativePath);

                if (!File.Exists(destFile) || File.GetLastWriteTime(sourceFile) > File.GetLastWriteTime(destFile))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(destFile));
                    File.Copy(sourceFile, destFile, true);
                }
            }
            state.SatetEnd(task, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), destDir);

        }

        public List<Backup_ModelsWPF> ExecuteAllTasks(List<Backup_ModelsWPF> tasks)
        {
            List<Backup_ModelsWPF> executedTasks = new List<Backup_ModelsWPF>();

            foreach (var task in tasks)
            {
                ExecuteSpecificTasks(task);
                executedTasks.Add(task);
            }
            return executedTasks;
        }
    }
}