using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private Log_Controller controller_log;
        private State_Controller controller_state;
        Stopwatch stopwatch = new Stopwatch();

        /// <summary>
        /// Initializes the controller with a specified language.
        /// </summary>
        /// <param name="language">Language code (e.g., "en" or "fr").</param>
        public BackupJob_Controller()
        {
            backupView = new View();
            backupModel = new BackupJob_Models("", "", "", BackupType.Full, true); // Load tasks
            controller_log = new Log_Controller();
            controller_state = new State_Controller();
            backupModel.LoadTasks();
            backupView.DisplayMainMenu();
        }

        public string DisplayLangue()
        {
            return backupView.DisplayLangue();

        }

        /// <summary>
        /// Pauses execution and waits for the user to press a key before returning to the menu.
        /// TEST DOCUMENTATION
        /// </summary>
        private void PauseAndReturn()
        {
            backupView.DisplayMessage("PressKeyToReturn");
            Console.ReadKey();
        }

        /// <summary>
        /// Creates a backup task.
        /// </summary>
        public void CreateBackupTask()
        {
            BackupJob_Models task = backupView.GetBackupDetails();
            stopwatch.Start();
            backupModel.CreateBackupTask(task);
            stopwatch.Stop();

            string formattedTime = stopwatch.Elapsed.ToString(@"hh\:mm\:ss\.fff");
            backupView.DisplayMessage("TaskCreated");
            controller_log.LogBackupAction(task, formattedTime, "Create Task");
            PauseAndReturn();
        }

        /// <summary>
        /// Deletes a backup task.
        /// </summary>
        public void DeleteTask()
        {
            backupView.DisplayMessage("DeleteTask");
            ViewTasks();

            stopwatch.Start();
            BackupJob_Models task = backupModel.DeleteTask();
            stopwatch.Stop();

            string formattedTime = stopwatch.Elapsed.ToString(@"hh\:mm\:ss\.fff");
            controller_log.LogBackupAction(task, formattedTime, "Delete Task");
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
            backupView.DisplayMessage("ExecuteSpecificTask");
            stopwatch.Start();
            BackupJob_Models task = backupModel.ExecuteSpecificTask();
            stopwatch.Stop();

            string formattedTime = stopwatch.Elapsed.ToString(@"hh\:mm\:ss\.fff");
            string logType = task.Type == BackupType.Full ? "Executing Full Backup" : "Executing Differential Backup";
            controller_log.LogBackupAction(task, formattedTime, logType);
            PauseAndReturn();
        }

        /// <summary>
        /// Executes all backup tasks.
        /// </summary>
        public void ExecuteAllTasks()
        {
            backupView.DisplayMessage("ExecuteAllTasks");
            stopwatch.Start();

            List<BackupJob_Models> executedTasks = backupModel.ExecuteAllTasks();
            stopwatch.Stop();

            string formattedTime = stopwatch.Elapsed.ToString(@"hh\:mm\:ss\.fff");
            foreach (var task in executedTasks)
            {
                controller_log.LogBackupAction(task, formattedTime, "Executing All Backup");
            }

            PauseAndReturn();
        }

        /// <summary>
        /// Handles invalid menu option input.
        /// </summary>
        public void ErreurChoix()
        {
            backupView.DisplayMessage("InvalidOption");
        }

        /// <summary>
        /// Displays the main menu.
        /// </summary>
        public void DisplayMainMenu()
        {
            backupView.DisplayMainMenu();
        }
    }
}
