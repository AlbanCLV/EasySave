using System;
using EasySave.Models;
using EasySave.Views;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;


namespace EasySave.Controllers
{
    /// <summary>
    /// Controller class for managing interactions between the BackupJob model and view.
    /// TEST COMMIT in sln
    /// </summary>
    public class BackupJob_Controller
    {
        private BackupJob_Models backupModel;
        private BackupJob_View backupView;
        private Log_Controller controller_log;
        Stopwatch stopwatch = new Stopwatch();

        /// <summary>
        /// Default constructor that initializes the view and model.
        /// </summary>
        public BackupJob_Controller()
        {
            backupView = new BackupJob_View();
            // The model is initialized with empty (or default) values, then tasks are loaded
            backupModel = new BackupJob_Models("", "", "", BackupType.Full);
            controller_log = new Log_Controller();
            backupModel.LoadTasks();
            backupView.DisplayMainMenu();
        }
      

        /// <summary>
        /// Pauses execution and waits for the user to press a key before returning to the menu.
        /// TEST DOCUMENTATION
        /// </summary>
        private void PauseAndReturn()
        {
            Console.WriteLine("\nPress any key to return to the menu...");
            Console.ReadKey();
        }

        /// <summary>
        /// Creates a backup task.
        /// </summary>l
        public void CreateBackupTask()
        {
            BackupJob_Models task = backupView.GetBackupDetails();
            stopwatch.Start();
            backupModel.CreateBackupTask(task);
            stopwatch.Stop();
            TimeSpan time = stopwatch.Elapsed;
            string formattedTime = time.ToString(@"hh\:mm\:ss\.fff");
            backupView.DisplayMessage("Backup task created successfully.");
            controller_log.LogBackupAction(task, formattedTime, "Create Tasks");
            PauseAndReturn();
  
        }

        /// <summary>
        /// Deletes a backup task.
        /// </summary>
        public void DeleteTask()
        {
            backupView.DisplayMessage("Delete a backup task \n");
            ViewTasks();
            stopwatch.Start();
            BackupJob_Models task = backupModel.DeleteTask();
            stopwatch.Stop();
            TimeSpan time = stopwatch.Elapsed;
            string formattedTime = time.ToString(@"hh\:mm\:ss\.fff");
            controller_log.LogBackupAction(task, formattedTime, "Delete a Tasks");
            PauseAndReturn();
        }

        /// <summary>
        /// Displays all backup tasks.
        /// </summary>
        public void ViewTasks()
        {
            backupModel.ViewTasks();
            Console.ReadKey();
        }

        /// <summary>
        /// Executes a specific backup task.
        /// </summary>
        public void ExecuteSpecificTask()
        {
            Console.Clear();
            backupView.DisplayMessage("Execute a Backup Task\n");
            stopwatch.Start();
            BackupJob_Models task = backupModel.ExecuteSpecificTask();
            stopwatch.Stop();
            TimeSpan time = stopwatch.Elapsed;
            string formattedTime = time.ToString(@"hh\:mm\:ss\.fff");
            if (task.Type == BackupType.Full)
            {
                controller_log.LogBackupAction(task, formattedTime, "Executing Full backup");
            }else if (task.Type == BackupType.Differential)
            {
                controller_log.LogBackupAction(task, formattedTime, "Executing differential backup");
            }
            PauseAndReturn();
        }
        public void ExecuteAllTasks()
        {
            Console.Clear();
            backupView.DisplayMessage("Execute all Backup Task\n");
            stopwatch.Start();
            List<BackupJob_Models> executedTasks = backupModel.ExecuteAllTasks();
            stopwatch.Stop();
            TimeSpan time = stopwatch.Elapsed;
            string formattedTime = time.ToString(@"hh\:mm\:ss\.fff");
            foreach (var task in executedTasks)
            {
                controller_log.LogBackupAction(task, formattedTime, "Execute All Backup");
            }
            PauseAndReturn();

        }


        /// <summary>
        /// Executes all backup tasks.
        /// </summary>

        public void ErreurChoix()
        {
            backupView.DisplayMessage("Invalid option.Please try again.");
        }
        public void DisplayMainMenu()
        {
            backupView.DisplayMainMenu();
        }
    }
}
