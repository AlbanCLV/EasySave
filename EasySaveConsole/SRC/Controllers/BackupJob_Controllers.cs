// Gère l'exécution des travaux de sauvegarde (lecture des fichiers, copie vers le répertoire cible, etc.).
using System;
using EasySave.Models;  // Pour utiliser BackupJob et autres modèles si nécessaire
using EasySave.Utilities;  // Si tu utilises des utilitaires comme JsonHelper ou PathValidator
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;


namespace EasySave.Controllers

{
    public class BackupJobController
    {
        private readonly List<BackupJobModel> tasks = new List<BackupJobModel>();
        private const string SaveFilePath = "tasks.json";

        private readonly LogController logger = new LogController();
        private readonly StateController stateManager = new StateController();

        public BackupJobController()
        {
            LoadTasks();
        }

        public void SaveTasks()
        {
            JsonHelper.SaveToJson(SaveFilePath, tasks);
        }

        public void LoadTasks()
        {
            var loadedTasks = JsonHelper.LoadFromJson<List<BackupJobModel>>(SaveFilePath);
            if (loadedTasks != null)
            {
                tasks.AddRange(loadedTasks);
            }
        }

        public void CreateBackupTask(string name, string source, string destination, BackupType type)
        {
            if (tasks.Count >= 5)
            {
                throw new Exception("You cannot create more than 5 backup tasks.");
            }

            var task = new BackupJobModel(name, source, destination, type);
            tasks.Add(task);
            SaveTasks();
        }

        public void ViewTasks()
        {
            if (tasks.Count == 0)
            {
                Console.WriteLine("No backup tasks created yet.");
                return;
            }

            Console.WriteLine("Existing Backup Tasks:");
            foreach (var task in tasks)
            {
                Console.WriteLine($"- {task.Name} ({task.Type}) | Source: {task.SourceDirectory} -> Target: {task.TargetDirectory}");
            }
        }

        public void ExecuteBackup(string taskName)
        {
            var task = tasks.Find(t => t.Name == taskName);
            if (task == null)
            {
                Console.WriteLine("Task not found.");
                return;
            }

            if (task.Type == BackupType.Full)
            {
                PerformFullBackup(task);
            }
            else
            {
                ExecuteDifferentialBackup(task);
            }
        }

        public void ExecuteAllTasks()
        {
            if (tasks.Count == 0)
            {
                Console.WriteLine("No tasks available to execute.");
                return;
            }

            foreach (var task in tasks)
            {
                ExecuteBackup(task.Name);
            }
        }

        public void DeleteTask(string taskName)
        {
            var task = tasks.Find(t => t.Name == taskName);
            if (task == null)
            {
                Console.WriteLine("Task not found.");
                return;
            }

            tasks.Remove(task);
            SaveTasks();
            Console.WriteLine($"Backup task '{taskName}' deleted successfully.");
        }

        private void PerformFullBackup(BackupJobModel task)
        {
            Console.WriteLine($"Executing full backup: {task.Name}");

            string targetPath = PathHelper.GetUniqueDirectoryName(task.TargetDirectory, task.SourceDirectory);
            Directory.CreateDirectory(targetPath);

            CopyDirectoryContent(task.SourceDirectory, targetPath);
        }

        private void ExecuteDifferentialBackup(BackupJobModel task)
        {
            Console.WriteLine($"Executing differential backup: {task.Name}");

            string targetPath = Path.Combine(task.TargetDirectory, Path.GetFileName(task.SourceDirectory));

            if (!Directory.Exists(targetPath))
            {
                Console.WriteLine($"No previous backup found for '{task.SourceDirectory}'. Performing full backup.");
                PerformFullBackup(task);
                return;
            }

            RemoveDeletedFilesAndFolders(task.SourceDirectory, targetPath);
            CopyModifiedFiles(task.SourceDirectory, targetPath);
        }

        private void CopyModifiedFiles(string sourceDir, string destDir)
        {
            foreach (string sourceFile in Directory.GetFiles(sourceDir, "*", SearchOption.AllDirectories))
            {
                string relativePath = PathHelper.GetRelativePath(sourceDir, sourceFile);
                string destFile = Path.Combine(destDir, relativePath);

                if (!File.Exists(destFile) || File.GetLastWriteTime(sourceFile) > File.GetLastWriteTime(destFile))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(destFile));
                    File.Copy(sourceFile, destFile, true);
                }
            }
        }

        private void RemoveDeletedFilesAndFolders(string sourceDir, string destDir)
        {
            foreach (string destFile in Directory.GetFiles(destDir, "*", SearchOption.AllDirectories))
            {
                string relativePath = PathHelper.GetRelativePath(destDir, destFile);
                string sourceFile = Path.Combine(sourceDir, relativePath);

                if (!File.Exists(sourceFile))
                {
                    File.Delete(destFile);
                }
            }
        }

        private void CopyDirectoryContent(string sourceDir, string targetDir)
        {
            foreach (string file in Directory.GetFiles(sourceDir, "*", SearchOption.AllDirectories))
            {
                string relativePath = PathHelper.GetRelativePath(sourceDir, file);
                string destinationFile = Path.Combine(targetDir, relativePath);

                Directory.CreateDirectory(Path.GetDirectoryName(destinationFile));
                File.Copy(file, destinationFile, true);
            }
        }
    }
}
