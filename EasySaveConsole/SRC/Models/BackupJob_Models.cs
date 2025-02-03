using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;  // Assurez-vous que cette directive est présente
using Terminal.Gui;

namespace EasySave.Models
{
    public enum BackupType
    {
        Full,
        Differential
    }

    public class BackupJob_Models
    {
        public string Name { get; set; }
        public string SourceDirectory { get; set; }
        public string TargetDirectory { get; set; }
        public BackupType Type { get; set; }


        // Liste des tâches de sauvegarde
        [JsonIgnore]  // Ignore la liste des tâches lors de la sérialisation/désérialisation
        public List<BackupJob_Models> Tasks { get; } = new List<BackupJob_Models>();
        private readonly Log_Models logger = new Log_Models();
        private readonly State_Models stateManager = new State_Models();
        private const string SaveFilePath = "tasks.json";
        public BackupJob_Models(string name, string sourceDirectory, string targetDirectory, BackupType type, bool loadTasks = false)
        {
            Name = name;
            SourceDirectory = sourceDirectory;
            TargetDirectory = targetDirectory;
            Type = type;

            if (loadTasks)  // Charge les tâches seulement si nécessaire
            {
                LoadTasks();
            }
        }


        // Method to save tasks to a JSON file
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

        // Charger la liste des tâches depuis le fichier JSON
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
                        Tasks.Clear(); // Évite les doublons
                        Tasks.AddRange(loadedTasks);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading tasks: {ex.Message}");
            }
        }


        // Créer une nouvelle tâche de sauvegarde
        public void CreateBackupTask(BackupJob_Models task)
        {
            if (Tasks.Count >= 5)
            {
                Console.WriteLine("You cannot create more than 5 backup tasks.");
                return;
            }
            // Étape 5 : Résumé et confirmation
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
       

        // Afficher toutes les tâches
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

        // Method to delete a specific task
        public void DeleteTask()
        {
            ViewTasks(); // Display all tasks

            if (Tasks.Count == 0) return;

            // Prompt the user for the task number to delete
            Console.Write("Enter the number of the task to delete: ");
            if (int.TryParse(Console.ReadLine(), out int taskNumber) && taskNumber > 0 && taskNumber <= Tasks.Count)
            {
                // Delete the selected task
                Console.WriteLine($"Deleting task '{Tasks[taskNumber - 1].Name}'...");
                Tasks.RemoveAt(taskNumber - 1);
                Console.WriteLine("Task deleted successfully.");

                // Save the updated task list
                SaveTasks();
            }
            else
            {
                Console.WriteLine("Invalid task number.");
            }
        }

        public void ExecuteSpecificTask()
        {
            ViewTasks(); // Display all tasks

            if (Tasks.Count == 0)
            {
                Console.WriteLine("No tasks available to execute.");
                return;
            }

            // Prompt the user for the task number to execute
            Console.Write("Enter the number of the task to execute: ");
            if (int.TryParse(Console.ReadLine(), out int taskNumber) && taskNumber > 0 && taskNumber <= Tasks.Count)
            {
                // Execute the selected task
                ExecuteBackup(Tasks[taskNumber - 1]);
            }
            else
            {
                Console.WriteLine("Invalid task number.");
            }
        }


        public void ExecuteAllTasks()
        {
            foreach (var task in Tasks)
            {
                ExecuteBackup(task); // Execute each task
            }
        }
        private string GetUniqueDirectoryName(string destinationPath, string sourceDirectoryName)
        {
            string uniqueName = sourceDirectoryName;
            int counter = 1;

            // Boucle pour trouver un nom unique
            while (Directory.Exists(Path.Combine(destinationPath, uniqueName)))
            {
                uniqueName = $"{sourceDirectoryName} ({counter})";
                counter++;
            }

            return Path.Combine(destinationPath, uniqueName);
        }
        private void CopyDirectoryContent(string sourceDir, string targetDir)
        {
            // Copier tous les fichiers du répertoire source
            foreach (string file in Directory.GetFiles(sourceDir, "*", SearchOption.TopDirectoryOnly))
            {
                string fileName = Path.GetFileName(file);
                string destinationFile = Path.Combine(targetDir, fileName);
                File.Copy(file, destinationFile, true); // Écraser si le fichier existe
            }

            // Copier tous les sous-dossiers du répertoire source
            foreach (string directory in Directory.GetDirectories(sourceDir, "*", SearchOption.TopDirectoryOnly))
            {
                string directoryName = Path.GetFileName(directory);
                string destinationSubDir = Path.Combine(targetDir, directoryName);
                Directory.CreateDirectory(destinationSubDir);
                CopyDirectoryContent(directory, destinationSubDir); // Appel récursif pour copier les sous-dossiers
            }
        }
        private void CopyModifiedFiles(string sourceDir, string destDir)
        {
            foreach (string sourceFile in Directory.GetFiles(sourceDir, "*", SearchOption.AllDirectories))
            {
                // Obtenir le chemin relatif par rapport au répertoire source
                string relativePath = PathHelper.GetRelativePath(sourceDir, sourceFile);

                // Construire le chemin complet dans le répertoire cible
                string destFile = Path.Combine(destDir, relativePath);

                // Vérifie si le fichier n'existe pas ou s'il a été modifié
                if (!File.Exists(destFile) || File.GetLastWriteTime(sourceFile) > File.GetLastWriteTime(destFile))
                {
                    // Créer les sous-dossiers nécessaires dans le répertoire cible
                    Directory.CreateDirectory(Path.GetDirectoryName(destFile));

                    // Copier le fichier et écraser s'il existe
                    File.Copy(sourceFile, destFile, true);

                    Console.WriteLine($"File updated or added: {destFile}");
                }
            }
        }
        // Private method to execute a single backup task
        private void ExecuteDifferentialBackup(BackupJob_Models task)
        {
            Console.WriteLine($"Executing differential backup: {task.Name}");

            // Vérifie si le répertoire source existe
            if (!Directory.Exists(task.SourceDirectory))
            {
                Console.WriteLine($"Source directory '{task.SourceDirectory}' does not exist.");
                return;
            }

            // Vérifie si le répertoire de destination existe
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
            // Copier uniquement les fichiers nouveaux ou modifiés directement dans le répertoire cible
            CopyModifiedFiles(task.SourceDirectory, targetPath);

            Console.WriteLine($"Differential backup '{task.Name}' completed successfully.");
        }
        private void PerformFullBackup(BackupJob_Models task)
        {
            Console.WriteLine($"Executing full backup: {task.Name}");

            // Vérifie si le répertoire source existe
            if (!Directory.Exists(task.SourceDirectory))
            {
                Console.WriteLine($"Source directory '{task.SourceDirectory}' does not exist.");
                return;
            }

            // Créer un répertoire unique pour la sauvegarde
            string sourceDirectoryName = Path.GetFileName(task.SourceDirectory.TrimEnd(Path.DirectorySeparatorChar));
            string targetPath = GetUniqueDirectoryName(task.TargetDirectory, sourceDirectoryName);

            // Crée le répertoire cible
            Directory.CreateDirectory(targetPath);

            // Copier tout le contenu du répertoire source dans le répertoire cible
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
