using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using EasySave.Models;
using EasySave.Views;
using EasySaveLog;
using EasySave.Utilities;  // Assurez-vous d'inclure le namespace pour LangManager


namespace EasySave.Controllers
{
    /// <summary>
    /// Controller class for managing interactions between the BackupJob model and view.
    /// This class handles the creation, execution, and management of backup tasks.
    /// </summary>
    public class BackupJob_Controller
    {
        // Singleton instance
        private static BackupJob_Controller _instance;

        // Lock object for thread safety
        private static readonly object _lock = new object();
        private BackupJob_Models backupModel;
        private BackupJob_View backupView;
        private Log_Controller controller_log;
        private State_Controller controller_state;
        private Stopwatch stopwatch = new Stopwatch();

        /// <summary>
        /// Private constructor to prevent direct instantiation.
        /// </summary>
        public BackupJob_Controller()
        {
            backupView = new BackupJob_View();
            backupModel = new BackupJob_Models("", "", "", BackupType.Full, true); // Load tasks
            controller_log = new Log_Controller();
            controller_state = new State_Controller();
            backupModel.LoadTasks();
        }
        /// <summary>
        /// Gets the singleton instance of the BackupJob_Controller class.
        /// </summary>
        public static BackupJob_Controller Instance
        {
            get
            {
                // Use double-check locking to ensure thread-safety
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new BackupJob_Controller();
                        }
                    }
                }
                return _instance;
            }
        }
        /// <summary>
        /// Displays the language selection screen and returns the chosen language.
        /// </summary>
        public string DisplayLangue()
        {
          return backupView.DisplayLangue();
          

        }

        /// <summary>
        /// Pauses execution and waits for the user to press a key before returning to the menu.
        /// This is used for user interaction after an operation is complete.
        /// </summary>
        private void PauseAndReturn()
        {
            backupView.DisplayMessage("PressKeyToReturn");
            Console.ReadKey();
        }

        public void Choice_Type_File_Log()
        {

            string Type_Now = controller_log.Get_Type_File();
            backupView.Get_Type_Log(Type_Now);
            string reponse = backupView.Type_File_Log();
            controller_log.Type_File_Log(reponse);
            backupModel.Type_Log(reponse);

            PauseAndReturn();  
        }

        /// <summary>
        /// Creates a backup task based on the user's input.
        /// Records the time taken to create the task and logs the action.
        /// </summary>
        public void CreateBackupTask()
        {
            BackupJob_Models task = backupView.GetBackupDetails();  // Get task details from user
            stopwatch.Start();
            string cond = backupModel.CreateBackupTask(task);  // Create the backup task
            stopwatch.Stop();
            string formattedTime = stopwatch.Elapsed.ToString(@"hh\:mm\:ss\.fff");  // Format elapsed time
            string t = controller_log.Get_Type_File();
            if (cond == "ok")
            {
                controller_log.LogBackupAction(task.Name, task.SourceDirectory, task.TargetDirectory, formattedTime, "Create Task");  // Log the action
            }
            else if (cond == "ko")
            {
                controller_log.LogBackupErreur(task.Name, "create_task_attempt", "The user entered 'no' during the confirmation.");  // Log the action
            }
        }

        /// <summary>
        /// Deletes a backup task.
        /// The user is prompted to view tasks before deleting one, and the action is logged.
        /// </summary>
        public void DeleteTask()
        {
            Console.Clear();
            ViewTasks();  // Display current tasks for the user to select from

            stopwatch.Start();
            BackupJob_Models task = backupModel.DeleteTask();  // Delete the selected task
            if (task == null) { return; }
            stopwatch.Stop();

            string formattedTime = stopwatch.Elapsed.ToString(@"hh\:mm\:ss\.fff");  // Format elapsed time
            string t = controller_log.Get_Type_File();
            controller_log.LogBackupAction(task.Name, task.SourceDirectory, task.TargetDirectory, formattedTime, "Delete Task");  // Log the action
            PauseAndReturn();  // Wait for user input before returning to the menu
        }

        /// <summary>
        /// Displays all available backup tasks.
        /// Allows the user to review existing tasks.
        /// </summary>
        public void ViewTasks()
        {
            backupModel.ViewTasks();  // Display all tasks
            Console.ReadKey();  // Wait for user input before proceeding
        }

        /// <summary>
        /// Executes a specific backup task selected by the user.
        /// The task execution time is logged, and a message is displayed based on the type of backup.
        /// </summary>
        public void ExecuteSpecificTask()
        {
            Console.Clear();  // Clear the screen for a clean display
            backupView.DisplayMessage("ExecuteSpecificTask");  // Inform the user that the task will be executed
            stopwatch.Start();
            BackupJob_Models task = backupModel.ExecuteSpecificTask();  // Execute the selected backup task
            if (task == null) { return; }
            stopwatch.Stop();

            string formattedTime = stopwatch.Elapsed.ToString(@"hh\:mm\:ss\.fff");  // Format elapsed time
            string logType = task.Type == BackupType.Full ? "Executing Full Backup" : "Executing Differential Backup";  // Determine log type based on task type
            string t = controller_log.Get_Type_File();
            controller_log.LogBackupAction(task.Name, task.SourceDirectory, task.TargetDirectory, formattedTime, "execute specific Task");  // Log the action
            PauseAndReturn();  // Wait for user input before returning to the menu
        }

        /// <summary>
        /// Executes all backup tasks sequentially.
        /// Logs each task's execution and the total time taken.
        /// </summary>
        public void ExecuteAllTasks()
        {
            Console.Clear();  // Clear the screen for a clean display
            backupView.DisplayMessage("ExecuteAllTasks");  // Notify the user that all tasks will be executed
            stopwatch.Start();

            List<BackupJob_Models> executedTasks = backupModel.ExecuteAllTasks();  // Execute all tasks
            stopwatch.Stop();
            if (executedTasks == null) { return; }
            string formattedTime = stopwatch.Elapsed.ToString(@"hh\:mm\:ss\.fff");  // Format elapsed time
            string t = controller_log.Get_Type_File();
            foreach (var task in executedTasks)
            {
               controller_log.LogBackupAction(task.Name, task.SourceDirectory, task.TargetDirectory, formattedTime, "Execute all Task");  // Log the action
            }
            PauseAndReturn();  // Wait for user input before returning to the menu
        }

        /// <summary>
        /// Handles invalid menu option input from the user.
        /// Displays an error message to guide the user back to a valid choice.
        /// </summary>
        public void ErreurChoix()
        {
            backupView.DisplayMessage("InvalidOption");  // Display error message for invalid input
        }

        /// <summary>
        /// Displays the main menu for the user to interact with.
        /// Allows the user to choose different operations.
        /// </summary>
        public void DisplayMainMenu()
        {
            backupView.DisplayMainMenu();  // Show the main menu to the user
        }
    }
}
