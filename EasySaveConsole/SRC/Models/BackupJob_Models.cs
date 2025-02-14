using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Newtonsoft.Json;
using EasySave.Utilities;
using EasySave.Log;
using EasySave.Controllers;

namespace EasySave.Models
{
    public enum BackupType
    {
        Full,
        Differential
    }

    /// <summary>
    /// Représente une tâche de sauvegarde et gère la liste globale des tâches.
    /// Intègre la logique de sauvegarde complète et différentielle avec chiffrement.
    /// </summary>
    public class BackupJob_Models
    {
        public string Name { get; set; }
        public string SourceDirectory { get; set; }
        public string TargetDirectory { get; set; }
        public BackupType Type { get; set; }

        private static readonly string ConfigFilePath = "business_apps.txt";

        /// <summary>
        /// List of backup tasks.
        /// </summary>
        [JsonIgnore]
        public List<BackupJob_Models> Tasks { get; } = new List<BackupJob_Models>();
        private readonly Log_Models logger = new Log_Models();
        private const string SaveFilePath = "tasks.json";
        private readonly LangManager lang = LangManager.Instance;

        public BackupJob_Models(string name, string sourceDirectory, string targetDirectory, BackupType type)
        {
            Name = name;
            SourceDirectory = sourceDirectory;
            TargetDirectory = targetDirectory;
            Type = type;
        }

        public static void LoadTasks()
        {
            try
            {
                if (File.Exists(SaveFilePath))
                {
                    string json = File.ReadAllText(SaveFilePath);
                    var loadedTasks = JsonConvert.DeserializeObject<List<BackupJob_Models>>(json);
                    if (loadedTasks != null)
                        _tasks = loadedTasks;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{LangManager.Instance.Translate("error_loading_tasks")}: {ex.Message}");
            }
        }

        public static void SaveTasks()
        {
            try
            {
                string json = JsonConvert.SerializeObject(_tasks, Formatting.Indented);
                File.WriteAllText(SaveFilePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{LangManager.Instance.Translate("error_saving_tasks")}: {ex.Message}");
            }
        }

        public static string CreateBackupTask(BackupJob_Models task)
        {
            if (_tasks.Count >= 5)
            {
                Console.WriteLine(LangManager.Instance.Translate("max_tasks_error"));
                for (int i = 5; i > 0; i--)
                {
                    Console.WriteLine(i);
                    Thread.Sleep(1000);
                }
                Environment.Exit(0);
            }
            Console.Clear();
            Console.WriteLine(LangManager.Instance.Translate("task_summary"));
            Console.WriteLine($"{LangManager.Instance.Translate("task_name")}: {task.Name}");
            Console.WriteLine($"{LangManager.Instance.Translate("source_directory")}: {task.SourceDirectory}");
            Console.WriteLine($"{LangManager.Instance.Translate("target_directory")}: {task.TargetDirectory}");
            Console.WriteLine($"{LangManager.Instance.Translate("backup_type")}: {(task.Type == BackupType.Full ? LangManager.Instance.Translate("full_backup") : LangManager.Instance.Translate("differential_backup"))}");
            Console.WriteLine($"\n{LangManager.Instance.Translate("confirm_task_creation")}");
            string confirmation = Console.ReadLine()?.ToUpper();
            if (confirmation == "Y")
            {
                _tasks.Add(task);
                SaveTasks();
                return "ok";
            }
            else
            {
                Console.WriteLine(LangManager.Instance.Translate("task_creation_canceled"));
                return "ko";
            }
        }

        public static void ViewTasks()
        {
            if (_tasks.Count == 0)
            {
                Console.WriteLine(LangManager.Instance.Translate("no_backup_tasks"));
                return;
            }
            Console.Clear();
            Console.WriteLine(LangManager.Instance.Translate("existing_backup_tasks"));
            for (int i = 0; i < _tasks.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {LangManager.Instance.Translate("task_details")}:");
                Console.WriteLine($"{LangManager.Instance.Translate("task_name")}: {_tasks[i].Name}");
                Console.WriteLine($"{LangManager.Instance.Translate("source_directory")}: {_tasks[i].SourceDirectory}");
                Console.WriteLine($"{LangManager.Instance.Translate("target_directory")}: {_tasks[i].TargetDirectory}");
                Console.WriteLine($"{LangManager.Instance.Translate("backup_type")}: {_tasks[i].Type}");
                Console.WriteLine();
            }
        }

        public static BackupJob_Models DeleteTask()
        {
            if (_tasks.Count == 0)
            {
                Console.WriteLine(lang.Translate("no_tasks_to_delete"));
                controller_log.LogBackupErreur("Error", lang.Translate("delete_task_attempt"), lang.Translate("no_tasks_to_delete"));
                Environment.Exit(0);
                return null;
            }
            BackupJob_Models deletedTask = null;
            while (true)
            {
                Console.Write(LangManager.Instance.Translate("enter_task_number_to_delete"));
                if (int.TryParse(Console.ReadLine(), out int taskNumber) && taskNumber > 0 && taskNumber <= _tasks.Count)
                {
                    deletedTask = _tasks[taskNumber - 1];
                    Console.WriteLine(string.Format(LangManager.Instance.Translate("deleting_task"), deletedTask.Name));
                    _tasks.RemoveAt(taskNumber - 1);
                    Console.WriteLine(LangManager.Instance.Translate("task_deleted_successfully"));
                    SaveTasks();
                    break;
                }
                else
                {
                    Console.WriteLine(LangManager.Instance.Translate("invalid_task_number"));
                }
            }
            return deletedTask;
        }

        #region Exécution des Tâches

        public static void ExecuteAllTasks()
        {
            if (_tasks.Count == 0)
            {
                Console.WriteLine(LangManager.Instance.Translate("no_tasks_to_execute"));
                return;
            }
            foreach (var task in _tasks)
            {
                ExecuteBackup(task);
            }
            Console.WriteLine(LangManager.Instance.Translate("all_tasks_executed"));
        }

        public static void ExecuteSpecificTask()
        {
            ViewTasks();
            if (_tasks.Count == 0)
            {
                Console.WriteLine(LangManager.Instance.Translate("no_tasks_to_execute"));
                return;
            }
            Console.Write(LangManager.Instance.Translate("enter_task_number_to_execute"));
            string input = Console.ReadLine();
            if (int.TryParse(input, out int taskNumber) && taskNumber > 0 && taskNumber <= _tasks.Count)
            {
                ExecuteBackup(_tasks[taskNumber - 1]);
                Console.WriteLine(LangManager.Instance.Translate("task_executed_successfully"));
            }
            else
            {
                Console.WriteLine(LangManager.Instance.Translate("invalid_task_number"));
            }
        }

        private static void ExecuteBackup(BackupJob_Models task)
        {
            if (task.Type == BackupType.Full)
                PerformFullBackup(task);
            else if (task.Type == BackupType.Differential)
                ExecuteDifferentialBackup(task);
        }

        private static void PerformFullBackup(BackupJob_Models task)
        {
            Console.WriteLine(string.Format(LangManager.Instance.Translate("executing_full_backup"), task.Name));
            if (!Directory.Exists(task.SourceDirectory))
            {
                Console.WriteLine(string.Format(LangManager.Instance.Translate("source_directory_not_exist"), task.SourceDirectory));
                return;
            }
            if (!Directory.Exists(task.TargetDirectory))
            {
                Directory.CreateDirectory(task.TargetDirectory);
            }
            CopyDirectory(task.SourceDirectory, task.TargetDirectory);
            Console.WriteLine(string.Format(LangManager.Instance.Translate("full_backup_completed"), task.Name));
            State_Controller.Instance.StatEnd(task, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), task.TargetDirectory);
        }

        private static void ExecuteDifferentialBackup(BackupJob_Models task)
        {
            Console.WriteLine(string.Format(LangManager.Instance.Translate("executing_differential_backup"), task.Name));
            if (!Directory.Exists(task.TargetDirectory))
            {
                Console.WriteLine(string.Format(LangManager.Instance.Translate("destination_directory_missing"), task.TargetDirectory));
                Console.Write(LangManager.Instance.Translate("prompt_full_backup_instead"));
                string input = Console.ReadLine()?.ToUpper();
                if (input == "Y")
                {
                    PerformFullBackup(task);
                }
                else
                {
                    Console.WriteLine(LangManager.Instance.Translate("differential_backup_canceled"));
                    return;
                }
            }
            else
            {
                CopyModifiedFiles(task.SourceDirectory, task.TargetDirectory);
                Console.WriteLine(string.Format(LangManager.Instance.Translate("differential_backup_completed"), task.Name));
                State_Controller.Instance.StatEnd(task, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), task.TargetDirectory);
            }
        }

        private static void CopyDirectory(string sourceDir, string destDir)
        {
            DirectoryInfo sourceDirectory = new DirectoryInfo(sourceDir);
            if (!sourceDirectory.Exists)
                throw new DirectoryNotFoundException("Source directory not found: " + sourceDir);

            DirectoryInfo[] dirs = sourceDirectory.GetDirectories();
            Directory.CreateDirectory(destDir);
            foreach (FileInfo file in sourceDirectory.GetFiles())
            {
                string targetFilePath = Path.Combine(destDir, file.Name);
                file.CopyTo(targetFilePath, true);
                // Chiffrer le fichier copié
                EncryptionUtility.ProcessFile(targetFilePath, true);
            }
            foreach (DirectoryInfo subdir in dirs)
            {
                string newDestinationDir = Path.Combine(destDir, subdir.Name);
                CopyDirectory(subdir.FullName, newDestinationDir);
            }
        }

        private static void CopyModifiedFiles(string sourceDir, string destDir)
        {
            DirectoryInfo sourceDirectory = new DirectoryInfo(sourceDir);
            if (!sourceDirectory.Exists)
                throw new DirectoryNotFoundException("Source directory not found: " + sourceDir);

        public void AddBusinessApplication(string appName)
        {
            if (string.IsNullOrWhiteSpace(appName)) return;

            List<string> existingApps = File.ReadAllLines(ConfigFilePath)
            .Select(line => line.Trim().ToLower()) // Normalisation
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .ToList();

            // Vérifier si l'application existe déjà
            if (existingApps.Contains(appName.ToLower())) // Comparaison insensible à la casse
            {
                Console.WriteLine($"{appName} est déjà dans la liste des logiciels métier.");
            }
            else
            {
                File.AppendAllText(ConfigFilePath, appName + Environment.NewLine);
                Console.WriteLine($"{appName} ajouté à la liste des logiciels métier.");
            }
            Console.WriteLine("\nAppuyez sur une touche pour revenir au menu...");
            Console.ReadKey(); // Pause pour laisser le temps à l'utilisateur de voir le message
        }

        public void DisplayExistingApplications()
        {
            if (File.Exists(ConfigFilePath))
            {
                List<string> existingApps = File.ReadAllLines(ConfigFilePath)
                    .Select(line => line.Trim())
                    .Where(line => !string.IsNullOrWhiteSpace(line))
                    .ToList();

                Console.WriteLine("\n📋 Applications métier déjà enregistrées :");
                if (existingApps.Count > 0)
                {
                    foreach (var app in existingApps)
                    {
                        Console.WriteLine("- " + app);
                    }
                }
                else
                {
                    Console.WriteLine("(Aucune application enregistrée pour l'instant.)");
                }
                Console.WriteLine();
            }
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

        #endregion
    }
}
