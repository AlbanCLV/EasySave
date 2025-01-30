using System;
using EasySave.Models;
using EasySave.Utilities;
using EasySave.Controllers;
using EasySave.Views;

namespace EasySave
{
    /// <summary>
    /// class Programm
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            // Initialisation des contrôleurs et des vues
            BackupJobController backupController = new BackupJobController();
            BackupJob_View backupView = new BackupJob_View();

            while (true)
            {
                // Afficher le menu principal
                backupView.DisplayMainMenu();
                string input = Console.ReadLine();

                switch (input)
                {
                    case "1": // Créer une nouvelle tâche de sauvegarde
                        var task = backupView.GetBackupDetails();
                        try
                        {
                            backupController.CreateBackupTask(task.Name, task.SourceDirectory, task.TargetDirectory, task.Type);
                            backupView.DisplayMessage("Backup task created successfully.");
                        }
                        catch (Exception ex)
                        {
                            backupView.DisplayMessage($"Error: {ex.Message}");
                        }
                        PauseAndReturn();
                        break;

                    case "2": // Exécuter une tâche de sauvegarde spécifique
                        Console.Clear();
                        backupController.ViewTasks(); // Affiche les tâches disponibles
                        string taskName = backupView.GetTaskName();
                        backupController.ExecuteBackup(taskName);
                        PauseAndReturn();
                        break;

                    case "3": // Exécuter toutes les tâches de sauvegarde
                        Console.Clear();
                        backupController.ExecuteAllTasks();
                        PauseAndReturn();
                        break;

                    case "4": // Afficher toutes les tâches de sauvegarde
                        Console.Clear();
                        backupController.ViewTasks();
                        PauseAndReturn();
                        break;

                    case "5": // Supprimer une tâche de sauvegarde
                        Console.Clear();
                        backupController.ViewTasks();
                        string taskToDelete = backupView.GetTaskName();
                        backupController.DeleteTask(taskToDelete);
                        backupView.DisplayMessage("Backup task deleted successfully.");
                        PauseAndReturn();
                        break;

                    case "6": // Quitter l'application
                        Console.Clear();
                        Console.WriteLine("Exiting EasySave...");
                        return;

                    default: // Option invalide
                        Console.Clear();
                        backupView.DisplayMessage("Invalid option. Please try again.");
                        PauseAndReturn();
                        break;
                }
            }
        }

        // Méthode pour afficher un message et attendre que l'utilisateur appuie sur une touche
        static void PauseAndReturn()
        {
            Console.WriteLine("\nPress any key to return to the menu...");
            Console.ReadKey();
        }
    }
}

