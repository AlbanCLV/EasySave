using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using EasySaveWPF.ModelsWPF;
using EasySaveWPF;
using EasySaveLog;
using EasySaveConsole.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.ComponentModel;
using EasySaveWPF.Views;
using System.Windows;


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
        private Log_ViewModels LogViewModels;
        private Stopwatch stopwatch = new Stopwatch();
        private CryptageWPF Cryptage;

        /// <summary>
        /// Private constructor to prevent direct instantiation.
        /// </summary>
        public Backup_VueModelsWPF()
        {
            backupModel = Backup_ModelsWPF.Instance; // Load tasks
            LogViewModels = Log_ViewModels.Instance;
            Cryptage = CryptageWPF.Instance;
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
        public (string, string) CreateBackupTaskWPF(string n, string s, string d, string t)
        {
            Backup_ModelsWPF task = new Backup_ModelsWPF(n, s, d, t); // Get task details from user
            stopwatch.Start();
            string r = backupModel.CreateBackupTask(task);  // Create the backup task
            stopwatch.Stop();
            string formattedTime = stopwatch.Elapsed.ToString(@"hh\:mm\:ss\.fff");  // Format elapsed time

            return (r, formattedTime);
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
            LogViewModels.LogBackupAction(task.Name, task.SourceDirectory, task.TargetDirectory, formattedTime, "Deleting_task", task.Type);  // Log the action
            return r;
        }
        public (string, string, string) ExecuteSpecificTasks(Backup_ModelsWPF task, CancellationToken token)
        {
            if (!string.IsNullOrEmpty(ProcessWatcherWPF.Instance.GetRunningBusinessApps()))
            {
                // Si des applications métiers sont en cours, arrêter l'exécution
                System.Windows.MessageBox.Show($"Les applications suivantes sont en cours : {ProcessWatcherWPF.Instance.GetRunningBusinessApps()}. Veuillez fermer ces applications avant de continuer.",
                                                 "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                return ("KO", "KO", "KO");
            }
            stopwatch.Start();
            (string r , string timeencrypt)= backupModel.ExecuteSpecificTasks(task, token);
            stopwatch.Stop();
            string formattedTime = stopwatch.Elapsed.ToString(@"hh\:mm\:ss\.fff");  // Format elapsed time
            return (r, formattedTime, timeencrypt);
        }
       
        public void SetFichierLog(string type)
        {
            LogViewModels.Type_File_Log(type);
        }

    }
}
