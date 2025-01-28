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
        // List to store all the backup tasks
        private readonly List<BackupJob_Models> tasks = new List<BackupJob_Models>();

        // File path for saving and loading tasks
        private const string SaveFilePath = "tasks.json";

        // Constructor that loads tasks from the JSON file when the application starts
        public BackupJob_Services()
        {
            LoadTasks(); // Load previously saved tasks
        }

        // Method to save tasks to a JSON file
        public void SaveTasks()
        {
            try
            {
                // Convert tasks list to a JSON-formatted string
                string json = JsonConvert.SerializeObject(tasks, Formatting.Indented);

                // Write the JSON string to the file
                File.WriteAllText(SaveFilePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving tasks: {ex.Message}");
            }
        }

        // Method to load tasks from a JSON file
        public void LoadTasks()
        {
            try
            {
                // Check if the save file exists
                if (File.Exists(SaveFilePath))
                {
                    // Read the JSON data from the file
                    string json = File.ReadAllText(SaveFilePath);

                    // Deserialize the JSON data back into a list of tasks
                    var loadedTasks = JsonConvert.DeserializeObject<List<BackupJob_Models>>(json);

                    if (loadedTasks != null)
                    {
                        // Add the loaded tasks to the current task list
                        tasks.AddRange(loadedTasks);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading tasks: {ex.Message}");
            }
        }

        // Method to create a new backup task
        public void CreateBackupTask()
        {
            // Check if the maximum limit of 5 tasks is reached
            if (tasks.Count >= 5)
            {
                Console.WriteLine("You cannot create more than 5 backup tasks.");
                return; // Exit the method if the limit is reached
            }

            // Prompt the user for task details
            Console.Write("Enter task name: ");
            string name = Console.ReadLine();

            Console.Write("Enter source directory: ");
            string source = Console.ReadLine();

            Console.Write("Enter target directory: ");
            string target = Console.ReadLine();

            Console.Write("Enter type (1 = Full, 2 = Differential): ");
            int typeInput = int.Parse(Console.ReadLine() ?? "1");
            BackupType type = typeInput == 1 ? BackupType.Full : BackupType.Differential;

            // Add the new task to the list
            tasks.Add(new BackupJob_Models(name, source, target, type));
            Console.WriteLine($"Backup task '{name}' created successfully.");

            // Save the updated task list
            SaveTasks();
        }

        // Method to display all existing backup tasks
        public void ViewTasks()
        {
            if (tasks.Count == 0)
            {
                Console.WriteLine("No backup tasks created yet.");
                return;
            }

            // Display details of each task
            Console.WriteLine("Existing Backup Tasks:");
            for (int i = 0; i < tasks.Count; i++)
            {
                Console.WriteLine($"{i + 1}. Name: {tasks[i].Name}, Source: {tasks[i].SourceDirectory}, Target: {tasks[i].TargetDirectory}, Type: {tasks[i].Type}");
            }
        }

        // Method to delete a specific task
        public void DeleteTask()
        {
            ViewTasks(); // Display all tasks

            if (tasks.Count == 0) return;

            // Prompt the user for the task number to delete
            Console.Write("Enter the number of the task to delete: ");
            if (int.TryParse(Console.ReadLine(), out int taskNumber) && taskNumber > 0 && taskNumber <= tasks.Count)
            {
                // Delete the selected task
                Console.WriteLine($"Deleting task '{tasks[taskNumber - 1].Name}'...");
                tasks.RemoveAt(taskNumber - 1);
                Console.WriteLine("Task deleted successfully.");

                // Save the updated task list
                SaveTasks();
            }
            else
            {
                Console.WriteLine("Invalid task number.");
            }
        }

        // Method to execute a specific task
        public void ExecuteSpecificTask()
        {
            ViewTasks(); // Display all tasks

            if (tasks.Count == 0)
            {
                Console.WriteLine("No tasks available to execute.");
                return;
            }

            // Prompt the user for the task number to execute
            Console.Write("Enter the number of the task to execute: ");
            if (int.TryParse(Console.ReadLine(), out int taskNumber) && taskNumber > 0 && taskNumber <= tasks.Count)
            {
                // Execute the selected task
                ExecuteBackup(tasks[taskNumber - 1]);
            }
            else
            {
                Console.WriteLine("Invalid task number.");
            }
        }

        // Method to execute all tasks sequentially
        public void ExecuteAllTasks()
        {
            foreach (var task in tasks)
            {
                ExecuteBackup(task); // Execute each task
            }
        }

        // Private method to execute a single backup task
        private void ExecuteBackup(BackupJob_Models task)
        {
            Console.WriteLine($"Executing backup: {task.Name}");

            // Check if the source directory exists
            if (!Directory.Exists(task.SourceDirectory))
            {
                Console.WriteLine($"Source directory '{task.SourceDirectory}' does not exist.");
                return;
            }

            // Get all files in the source directory, including subdirectories
            var files = Directory.GetFiles(task.SourceDirectory, "*", SearchOption.AllDirectories);
            long totalSize = 0;

            // Calculate the total size of files to be backed up
            foreach (var file in files)
            {
                totalSize += new FileInfo(file).Length;
            }

            long transferredSize = 0;
            int remainingFiles = files.Length;

            foreach (var file in files)
            {
                // Calculate the relative path for the target directory
                string relativePath = PathHelper.GetRelativePath(task.SourceDirectory, file);
                string targetPath = Path.Combine(task.TargetDirectory, relativePath);

                // Create necessary directories in the target path
                Directory.CreateDirectory(Path.GetDirectoryName(targetPath));

                // Copy the file to the target path
                File.Copy(file, targetPath, true);

                // Update the progress
                transferredSize += new FileInfo(file).Length;
                remainingFiles--;

            }

            Console.WriteLine($"Backup '{task.Name}' completed successfully.");
        }
    }
}
