using System;
using EasySave.Models;
using EasySave.Views;

namespace EasySave.Controllers
{
    public class BackupJob_Controller
    {
        private BackupJob_Models backupModel;
        private BackupJob_View backupView;

        public BackupJob_Controller(BackupJob_View view, BackupJob_Models model)
        {
            backupView = view;
            backupModel = model;
            backupModel.LoadTasks();  // Charger les tâches existantes à partir du fichier
        }

        // Créer une tâche de sauvegarde
        public void CreateBackupTask()
        {
            BackupJob_Models task = backupView.GetBackupDetails();
            backupModel.CreateBackupTask(task);
            backupView.DisplayMessage("Backup task created successfully.");
        }

        // Afficher toutes les tâches de sauvegarde
        public void ViewTasks()
        {
            backupModel.ViewTasks();
        }

        // Supprimer une tâche
        public void DeleteTask()
        {
            ViewTasks();
            backupModel.DeleteTask();
            backupView.DisplayMessage("Task deleted successfully.");
        }

        public void ExecuteSpecificTask()
        {
            backupModel.ExecuteSpecificTask();
        }

        // Exécuter toutes les tâches de sauvegarde
        public void ExecuteAllTasks()
        {
            backupModel.ExecuteAllTasks();
        }
    }
}
