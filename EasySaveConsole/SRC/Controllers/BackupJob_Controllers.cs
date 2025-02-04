using System;
using EasySave.Models;
using EasySave.Views;

namespace EasySave.Controllers
{
    /// <summary>
    /// Controller class for managing interactions between the BackupJob model and view.
    /// </summary>
    public class BackupJob_Controller
    {
        private BackupJob_Models backupModel;
        private BackupJob_View backupView;

        /// <summary>
        /// Default constructor that initializes the view and model.
        /// </summary>
        public BackupJob_Controller()
        {
            backupView = new BackupJob_View();
            // The model is initialized with empty (or default) values, then tasks are loaded
            backupModel = new BackupJob_Models("", "", "", BackupType.Full);
            backupModel.LoadTasks();
            backupView.DisplayMainMenu();
        }

        // Méthode principale qui gère le menu et la navigation
        public void Run()
        {
            while (true)
            {
                backupView.DisplayMainMenu();
                string input = Console.ReadLine();

                switch (input)
                {
                    case "1": // Créer une tâche de sauvegarde
                        CreateBackupTask();
                        PauseAndReturn();
                        break;
                    case "2": // Exécuter une tâche spécifique
                        Console.Clear();
                        Console.WriteLine("Execute a Backup Task\n");
                        ExecuteSpecificTask();
                        PauseAndReturn();
                        break;
                    case "3": // Exécuter toutes les tâches
                        ExecuteAllTasks();
                        PauseAndReturn();
                        break;
                    case "4": // Voir toutes les tâches
                        ViewTasks();
                        PauseAndReturn();
                        break;
                    case "5": // Supprimer une tâche
                        Console.Write("Delete a backup task \n");
                        DeleteTask();
                        PauseAndReturn();
                        break;
                    case "6": // Quitter l'application
                        return;
                    default:
                        backupView.DisplayMessage("Invalid option. Please try again.");
                        break;
                }
            }
        }

        /// <summary>
        /// Pauses execution and waits for the user to press a key before returning to the menu.
        /// </summary>
        private void PauseAndReturn()
        {
            Console.WriteLine("\nPress any key to return to the menu...");
            Console.ReadKey();
        }

        /// <summary>
        /// Creates a backup task.
        /// </summary>
        public void CreateBackupTask()
        {
            backupView.DisplayMainMenu();
            BackupJob_Models task = backupView.GetBackupDetails();
            backupModel.CreateBackupTask(task);
            backupView.DisplayMessage("Backup task created successfully.");
            PauseAndReturn();
        }

        /// <summary>
        /// Displays all backup tasks.
        /// </summary>
        public void ViewTasks()
        {
            backupModel.ViewTasks();
            PauseAndReturn();
        }

        /// <summary>
        /// Deletes a backup task.
        /// </summary>
        public void DeleteTask()
        {
            ViewTasks();
            backupModel.DeleteTask();
            backupView.DisplayMessage("Task deleted successfully.");
            PauseAndReturn();
        }

        /// <summary>
        /// Executes a specific backup task.
        /// </summary>
        public void ExecuteSpecificTask()
        {
            backupModel.ExecuteSpecificTask();
            PauseAndReturn();
        }

        /// <summary>
        /// Executes all backup tasks.
        /// </summary>
        public void ExecuteAllTasks()
        {
            backupModel.ExecuteAllTasks();
            PauseAndReturn();
        }
        public void ErreurChoixMenu()
        {
            backupView.DisplayMessage("Invalid option. Please try again.");
        }
    }
}
