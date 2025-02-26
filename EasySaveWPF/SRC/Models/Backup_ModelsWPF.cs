using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using EasySaveWPF.ViewModelsWPF;
using Newtonsoft.Json;
using EasySaveLog;
using EasySaveWPF;
using EasySaveWPF.ModelsWPF;
using EasySaveConsole.Models;
using System.Windows;
using System.Diagnostics;
using System.Reactive.Concurrency;

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
        public int Progress { get; set; } 

        private static Backup_ModelsWPF _instance;
        private static readonly object _lock = new object();
        /// <summary>
        /// List of backup tasks.
        /// </summary>
        [JsonIgnore]
        private readonly Log_Models logger = new Log_Models();
        private readonly State_modelsWPF state = new State_modelsWPF();
        public const string SaveFilePath = "tasks.json";
        private Log_ViewModels controller_log = new Log_ViewModels();
        public List<Backup_ModelsWPF> Tasks { get; private set; } = new List<Backup_ModelsWPF>();
        private Cryptage_ModelsWPF cryptage;
        private Stopwatch stopwatch = new Stopwatch();
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

            cryptage = Cryptage_ModelsWPF.Instance;

            if (loadTasks)  // Load tasks if necessary
            {
                LoadTasks();
            }
        }
        public static Backup_ModelsWPF Instance
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
                            _instance = new Backup_ModelsWPF("", "", "", "Full", true);
                        }
                    }
                }
                return _instance;
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
                return "OK";
            }
            catch (Exception ex)
            {
                return "KO";
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
            Tasks.Add(task);
            return SaveTasks();

        }
        public string DeleteTaskWPF(Backup_ModelsWPF task)
        {
            Tasks.Remove(task);
            SaveTasks();
            return "task delete";
        }
        public (string, string) ExecuteSpecificTasks(Backup_ModelsWPF task)
        {
            return Task.Run(() =>
            {
                if (task.Type == "Full")
                    return PerformFullBackup(task);
                else if (task.Type == "Differential")
                    return ExecuteDifferentialBackup(task);

                return ("KO", "-1");
            }).Result;
        }


        public (string, string) PerformFullBackup(Backup_ModelsWPF task)
        {
            if (!Directory.Exists(task.SourceDirectory))
            {
                controller_log.LogBackupErreur(task.Name, "execute_task_attempt", "source_directory_not_exist", "-1");
                return ("KO SOURCE","-1");
            }

            string sourceDirectoryName = Path.GetFileName(task.SourceDirectory.TrimEnd(Path.DirectorySeparatorChar));
            string targetPath = GetUniqueDirectoryName(task.TargetDirectory, sourceDirectoryName);
            Directory.CreateDirectory(targetPath);
            return ("OK", CopyDirectoryContent(task.SourceDirectory, targetPath, task));
            
        }
        public (string, string) ExecuteDifferentialBackup(Backup_ModelsWPF task)
        {
            if (!Directory.Exists(task.SourceDirectory))
            {
                string t = controller_log.Get_Type_File();
                controller_log.LogBackupErreur(task.Name, "execute_task_attempt", "source_directory_not_exist", "-1");
                return ("KO SOURCE", "-1");
            }
            string sourceDirectoryName = Path.GetFileName(task.SourceDirectory.TrimEnd(Path.DirectorySeparatorChar));
            string targetPath = Path.Combine(task.TargetDirectory, sourceDirectoryName);
            if (!Directory.Exists(targetPath))
            {
                PerformFullBackup(task);
            }
            return ("OK" ,CopyModifiedFiles(task.SourceDirectory, targetPath, task));

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
        private string CopyDirectoryContent(string sourceDir, string targetDir, Backup_ModelsWPF task)
        {
            long totalEncryptionTime = 0; // Temps total d'encryption

            // Mise à jour de l'état de la tâche
            state.StateUpdate(task, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), targetDir);

            // Récupérer tous les fichiers du répertoire courant
            string[] files = Directory.GetFiles(sourceDir, "*", SearchOption.TopDirectoryOnly);

            // Séparer les fichiers prioritaires et non prioritaires
            var priorityFiles = new List<string>();
            var nonPriorityFiles = new List<string>();

            foreach (string file in files)
            {
                if (PriorityManager.IsPriority(file))
                    priorityFiles.Add(file);
                else
                    nonPriorityFiles.Add(file);
            }

            // Copier d'abord les fichiers prioritaires
            foreach (string file in priorityFiles)
            {
                string fileName = Path.GetFileName(file);
                string destinationFile = Path.Combine(targetDir, fileName);
                File.Copy(file, destinationFile, true);

                // Si le cryptage est activé, traiter le fichier
                if (Cryptage_ModelsWPF.EncryptEnabled)
                {
                    if (Cryptage_ModelsWPF.EncryptAll)
                    {
                        stopwatch.Restart();
                        var result = cryptage.ProcessFile(destinationFile, true);
                        stopwatch.Stop();
                        if (!result.Item3)
                            return "KO";
                        totalEncryptionTime += stopwatch.ElapsedMilliseconds;
                    }
                    else
                    {
                        string fileExtension = Path.GetExtension(file).ToLower();
                        if (Cryptage_ModelsWPF.SelectedExtensions.Contains(fileExtension))
                        {
                            stopwatch.Restart();
                            var result = cryptage.ProcessFile(destinationFile, true);
                            stopwatch.Stop();
                            if (!result.Item3)
                                return "KO";
                            totalEncryptionTime += stopwatch.ElapsedMilliseconds;
                        }
                    }
                }
            }

            // Ensuite copier les fichiers non prioritaires
            foreach (string file in nonPriorityFiles)
            {
                string fileName = Path.GetFileName(file);
                string destinationFile = Path.Combine(targetDir, fileName);
                File.Copy(file, destinationFile, true);

                if (Cryptage_ModelsWPF.EncryptEnabled)
                {
                    if (Cryptage_ModelsWPF.EncryptAll)
                    {
                        stopwatch.Restart();
                        var result = cryptage.ProcessFile(destinationFile, true);
                        stopwatch.Stop();
                        if (!result.Item3)
                            return "KO";
                        totalEncryptionTime += stopwatch.ElapsedMilliseconds;
                    }
                    else
                    {
                        string fileExtension = Path.GetExtension(file).ToLower();
                        if (Cryptage_ModelsWPF.SelectedExtensions.Contains(fileExtension))
                        {
                            stopwatch.Restart();
                            var result = cryptage.ProcessFile(destinationFile, true);
                            stopwatch.Stop();
                            if (!result.Item3)
                                return "KO";
                            totalEncryptionTime += stopwatch.ElapsedMilliseconds;
                        }
                    }
                }
            }

            // Traiter les sous-dossiers de manière récursive
            foreach (string directory in Directory.GetDirectories(sourceDir, "*", SearchOption.TopDirectoryOnly))
            {
                string directoryName = Path.GetFileName(directory);
                string destinationSubDir = Path.Combine(targetDir, directoryName);
                Directory.CreateDirectory(destinationSubDir);
                totalEncryptionTime += Convert.ToInt64(CopyDirectoryContent(directory, destinationSubDir, task));
            }

            state.SatetEnd(task, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), targetDir);

            return totalEncryptionTime.ToString();
        }


        private string CopyModifiedFiles(string sourceDir, string destDir, Backup_ModelsWPF task)
        {
            long totalEncryptionTime = 0;
            Stopwatch stopwatch = new Stopwatch();

            state.StateUpdate(task, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), destDir);

            // Récupérer tous les fichiers (dans tous les sous-dossiers)
            var allFiles = Directory.GetFiles(sourceDir, "*", SearchOption.AllDirectories);

            // Séparer en deux listes : fichiers prioritaires et non prioritaires
            var priorityFiles = new List<string>();
            var nonPriorityFiles = new List<string>();

            foreach (string file in allFiles)
            {
                if (PriorityManager.IsPriority(file))
                    priorityFiles.Add(file);
                else
                    nonPriorityFiles.Add(file);
            }

            // Copier d'abord les fichiers prioritaires
            foreach (string sourceFile in priorityFiles)
            {
                string relativePath = PathHelper.GetRelativePath(sourceDir, sourceFile);
                string destFile = Path.Combine(destDir, relativePath);
                Directory.CreateDirectory(Path.GetDirectoryName(destFile));
                if (!File.Exists(destFile) || File.GetLastWriteTime(sourceFile) > File.GetLastWriteTime(destFile))
                {
                    File.Copy(sourceFile, destFile, true);

                    if (Cryptage_ModelsWPF.EncryptEnabled)
                    {
                        stopwatch.Restart();
                        var result = cryptage.ProcessFile(destFile, true);
                        stopwatch.Stop();
                        if (!result.Item3)
                            return "KO";
                        totalEncryptionTime += stopwatch.ElapsedMilliseconds;
                    }
                }
            }

            // Ensuite copier les fichiers non prioritaires
            foreach (string sourceFile in nonPriorityFiles)
            {
                string relativePath = PathHelper.GetRelativePath(sourceDir, sourceFile);
                string destFile = Path.Combine(destDir, relativePath);
                Directory.CreateDirectory(Path.GetDirectoryName(destFile));
                if (!File.Exists(destFile) || File.GetLastWriteTime(sourceFile) > File.GetLastWriteTime(destFile))
                {
                    File.Copy(sourceFile, destFile, true);

                    if (Cryptage_ModelsWPF.EncryptEnabled)
                    {
                        stopwatch.Restart();
                        var result = cryptage.ProcessFile(destFile, true);
                        stopwatch.Stop();
                        if (!result.Item3)
                            return "KO";
                        totalEncryptionTime += stopwatch.ElapsedMilliseconds;
                    }
                }
            }

            state.SatetEnd(task, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), destDir);
            return totalEncryptionTime.ToString();
        }


        public (List<Backup_ModelsWPF>, List<string>, List<string>) ExecuteAllTasks(List<Backup_ModelsWPF> tasks)
        {
            List<Backup_ModelsWPF> executedTasks = new List<Backup_ModelsWPF>();
            List<string> logMessages = new List<string>();
            List<string> TimeEncrypt = new List<string>();

            var tasksList = new List<Task>();

            foreach (var task in tasks)
            {
                tasksList.Add(Task.Run(() =>
                {
                    (string log, string time) = ExecuteSpecificTasks(task);

                    lock (executedTasks) // Empêcher les conflits d'accès
                    {
                        executedTasks.Add(task);
                        logMessages.Add(log);
                        TimeEncrypt.Add(time);
                    }
                }));
            }

            Task.WaitAll(tasksList.ToArray()); // Attendre que toutes les tâches se terminent

            return (executedTasks, logMessages, TimeEncrypt);
        }

    }
}