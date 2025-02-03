using System;
using EasySave.Models;
using EasySave.Views;

namespace EasySave.Controllers
{
    public class BackupJob_Controller
    {
        private BackupJob_Models backupModel;
        private BackupJob_View backupView;

        // Constructeur sans paramètres qui instancie la vue et le modèle
        public BackupJob_Controller()
        {
            backupView = new BackupJob_View();
            // Le modèle est initialisé avec des valeurs vides (ou par défaut) puis les tâches sont chargées
            backupModel = new BackupJob_Models("", "", "", BackupType.Full);
            backupModel.LoadTasks();
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

        private void PauseAndReturn()
        {
            Console.WriteLine("\nPress any key to return to the menu...");
            Console.ReadKey();
        }

        // Méthode pour créer une tâche de sauvegarde
        public void CreateBackupTask()
        {
            BackupJob_Models task = backupView.GetBackupDetails();
            backupModel.CreateBackupTask(task);
            backupView.DisplayMessage("Backup task created successfully.");
        }

        // Méthode pour afficher toutes les tâches de sauvegarde
        public void ViewTasks()
        {
            backupModel.ViewTasks();
        }

        // Méthode pour supprimer une tâche
        public void DeleteTask()
        {
            ViewTasks();
            backupModel.DeleteTask();
            backupView.DisplayMessage("Task deleted successfully.");
        }

        // Exécuter une tâche spécifique
        public void ExecuteSpecificTask()
        {
            backupModel.ExecuteSpecificTask();
        }

        // Exécuter toutes les tâches
        public void ExecuteAllTasks()
        {
            backupModel.ExecuteAllTasks();
        }
    }
}
