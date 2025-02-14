using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using EasySaveWPF.ModelsWPF;
using EasySaveWPF;
using EasySaveLog;
using EasySave.Models;

namespace EasySaveWPF.ViewModelsWPF
{
    /// <summary>
    /// Controller class for managing interactions between the BackupJob model and view.
    /// This class handles the creation, execution, and management of backup tasks.
    /// </summary>
    public class Backup_VueModelsWPF
    {
        // Singleton instance
        private static Backup_VueModelsWPF _instance;

        // Lock object for thread safety
        private static readonly object _lock = new object();
        private Backup_ModelsWPF backupModel;
        private Log_Controller controller_log;
        private Stopwatch stopwatch = new Stopwatch();

        /// <summary>
        /// Private constructor to prevent direct instantiation.
        /// </summary>
        public Backup_VueModelsWPF()
        {
            backupModel = new Backup_ModelsWPF("", "", "", "Full", true); // Load tasks
            controller_log = new Log_Controller();
            backupModel.LoadTasks();
        }
        /// <summary>
        /// Gets the singleton instance of the Backup_VueModelsWPF class.
        /// </summary>
        public static Backup_VueModelsWPF Instance
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
                            _instance = new Backup_VueModelsWPF();
                        }
                    }
                }
                return _instance;
            }
        }
        

        /// <summary>
        /// Creates a backup task based on the user's input.
        /// Records the time taken to create the task and logs the action.
        /// </summary>
        public string CreateBackupTaskWPF(string n, string s, string d, string t)
        {
            Backup_ModelsWPF task = new Backup_ModelsWPF(n, s, d, t); // Get task details from user
            stopwatch.Start();
            string r = backupModel.CreateBackupTask(task);  // Create the backup task
            if (r == "Max Tasks Error") { return r; }
            stopwatch.Stop();
            string formattedTime = stopwatch.Elapsed.ToString(@"hh\:mm\:ss\.fff");  // Format elapsed time
            controller_log.LogBackupAction(task.Name, task.SourceDirectory, task.TargetDirectory, formattedTime, "Create Task");  // Log the action
            return r;
        }
        public List<Backup_ModelsWPF> ViewTasksWPF()
        {
            backupModel.LoadTasks(); // Recharger depuis le fichier
            return backupModel.Tasks;
        }
        public string DeleteBackupTaskWPF(Backup_ModelsWPF task)
        {
            stopwatch.Start();
            string r = backupModel.DeleteTaskWPF(task);
            stopwatch.Stop();
            string formattedTime = stopwatch.Elapsed.ToString(@"hh\:mm\:ss\.fff");  // Format elapsed time
            controller_log.LogBackupAction(task.Name, task.SourceDirectory, task.TargetDirectory, formattedTime, "Delete Task");  // Log the action
            return r;
        }

        public string ExecuteSpecificTasks(Backup_ModelsWPF task)
        {
            stopwatch.Start();
            string r = backupModel.ExecuteSpecificTasks(task);
            stopwatch.Stop();
            string formattedTime = stopwatch.Elapsed.ToString(@"hh\:mm\:ss\.fff");  // Format elapsed time
            controller_log.LogBackupAction(task.Name, task.SourceDirectory, task.TargetDirectory, formattedTime, "execute specific Task");  // Log the action

            return r;
        }


        public string ExecuteALlTask(List<Backup_ModelsWPF> taskList)
        {
            stopwatch.Start();

            List<Backup_ModelsWPF> executedTasks = backupModel.ExecuteAllTasks(taskList);  // Execute all tasks
            stopwatch.Stop();

            string formattedTime = stopwatch.Elapsed.ToString(@"hh\:mm\:ss\.fff");  // Format elapsed time
            string t = controller_log.Get_Type_File();
            foreach (var task in executedTasks)
            {
                controller_log.LogBackupAction(task.Name, task.SourceDirectory, task.TargetDirectory, formattedTime, "Execute all Task");  // Log the action
            }

            return "Toutes les taches ont été éxécuter";

        }


    }
}
