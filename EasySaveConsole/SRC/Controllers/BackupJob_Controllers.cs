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
            backupView.DisplayMainMenu();
        }

        

        private void PauseAndReturn()
        {
            Console.WriteLine("\nPress any key to return to the menu...");
            Console.ReadKey();
        }

        // Méthode pour créer une tâche de sauvegarde
        public void CreateBackupTask()
        {
            backupView.DisplayMainMenu();
            BackupJob_Models task = backupView.GetBackupDetails();
            backupModel.CreateBackupTask(task);
            backupView.DisplayMessage("Backup task created successfully.");
            PauseAndReturn();
        }

        // Méthode pour afficher toutes les tâches de sauvegarde
        public void ViewTasks()
        {
            backupModel.ViewTasks();
            PauseAndReturn();
        }

        // Méthode pour supprimer une tâche
        public void DeleteTask()
        {
            ViewTasks();
            backupModel.DeleteTask();
            backupView.DisplayMessage("Task deleted successfully.");
            PauseAndReturn();
        }

        // Exécuter une tâche spécifique
        public void ExecuteSpecificTask()
        {
            backupModel.ExecuteSpecificTask();
            PauseAndReturn();
        }

        // Exécuter toutes les tâches
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
