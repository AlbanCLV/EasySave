// Gère l'exécution des travaux de sauvegarde (lecture des fichiers, copie vers le répertoire cible, etc.).
using System;
using EasySave.Models;  // Pour utiliser BackupJob et autres modèles si nécessaire
using EasySave.Utilities;  // Si tu utilises des utilitaires comme JsonHelper ou PathValidator
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;


namespace EasySave.Services

{
    public class BackupJob_Services
    {
        private readonly List<BackupJob_Models> tasks = new List<BackupJob_Models>();
        private const string SaveFilePath = "tasks.json";

        public BackupJob_Services()
        {
            LoadTasks();
        }

        public void SaveTasks()
        {
            try
            {
                string json = JsonConvert.SerializeObject(tasks, Formatting.Indented);
                File.WriteAllText(SaveFilePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving tasks: {ex.Message}");
            }
        }

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
                        tasks.AddRange(loadedTasks);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading tasks: {ex.Message}");
            }
        }

        public void CreateBackupTask(BackupJob_Models task)
        {
            Console.Clear();
            Console.WriteLine("Create a Backup Task\n");
            if (tasks.Count >= 5)
            {
                Console.WriteLine("You cannot create more than 5 backup tasks.");
                return;
            }
            tasks.Add(task);
            SaveTasks();
        }

        public void ViewTasks()
        {
            Console.Clear();
            if (tasks.Count == 0)
            {
                Console.WriteLine("No backup tasks created yet.");
                return;
            }

            Console.WriteLine("Existing Backup Tasks:");
            for (int i = 0; i < tasks.Count; i++)
            {
                Console.WriteLine($"{i + 1}. Name: {tasks[i].Name}, Source: {tasks[i].SourceDirectory}, Target: {tasks[i].TargetDirectory}, Type: {tasks[i].Type}");
            }
        }

        public void DeleteTask()
        {
            ViewTasks();
            if (tasks.Count == 0) return;

            Console.Write("Enter the number of the task to delete: ");
            if (int.TryParse(Console.ReadLine(), out int taskNumber) && taskNumber > 0 && taskNumber <= tasks.Count)
            {
                Console.WriteLine($"Deleting task '{tasks[taskNumber - 1].Name}'...");
                tasks.RemoveAt(taskNumber - 1);
                Console.WriteLine("Task deleted successfully.");
                SaveTasks();
            }
            else
            {
                Console.WriteLine("Invalid task number.");
            }
        }

        public void ExecuteSpecificTask()
        {
            Console.WriteLine("Execute a Backup Task\n");
            if (tasks.Count == 0)
            {
                Console.WriteLine("No tasks available to execute.");
                return;
            }

            Console.Write("Enter the number of the task to execute: ");
            if (int.TryParse(Console.ReadLine(), out int taskNumber) && taskNumber > 0 && taskNumber <= tasks.Count)
            {
                ExecuteBackup(tasks[taskNumber - 1]);
            }
            else
            {
                Console.WriteLine("Invalid task number.");
            }
        }

        public void ExecuteAllTasks()
        {
            foreach (var task in tasks)
            {
                ExecuteBackup(task);
            }
        }

        private void ExecuteBackup(BackupJob_Models task)
        {
            Console.WriteLine($"Executing backup: {task.Name}");

            if (!Directory.Exists(task.SourceDirectory))
            {
                Console.WriteLine($"Source directory '{task.SourceDirectory}' does not exist.");
                return;
            }

            var files = Directory.GetFiles(task.SourceDirectory, "*", SearchOption.AllDirectories);
            long totalSize = 0;

            foreach (var file in files)
            {
                totalSize += new FileInfo(file).Length;
            }

            long transferredSize = 0;
            int remainingFiles = files.Length;

            foreach (var file in files)
            {
                string relativePath = PathHelper.GetRelativePath(task.SourceDirectory, file);
                string targetPath = Path.Combine(task.TargetDirectory, relativePath);
                Directory.CreateDirectory(Path.GetDirectoryName(targetPath));
                File.Copy(file, targetPath, true);
                transferredSize += new FileInfo(file).Length;
                remainingFiles--;
            }

            Console.WriteLine($"Backup '{task.Name}' completed successfully.");
        }
    }
}
