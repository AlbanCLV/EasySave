using System;
using System.Diagnostics;
using EasySave.Models;
using EasySave.Views;
using EasySave.Log;
using EasySaveLog;
using EasySave.Utilities;

namespace EasySave.Controllers
{
    /// <summary>
    /// Contrôleur principal qui gère la logique (création, exécution, suppression) des tâches.
    /// Intègre également les logs et la mise à jour de l'état.
    /// </summary>
    public class BackupJob_Controller
    {
        private static BackupJob_Controller _instance;
        private static readonly object _lock = new object();
        private BackupJob_View backupView;
        private Log_Controller controller_log;
        private State_Controller controller_state;
        private Stopwatch stopwatch = new Stopwatch();

        private BackupJob_Controller()
        {
            backupView = new BackupJob_View();
            BackupJob_Models.LoadTasks();
            controller_log = new Log_Controller();
            controller_state = State_Controller.Instance;
        }

        public static BackupJob_Controller Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                            _instance = new BackupJob_Controller();
                    }
                }
                return _instance;
            }
        }

        public string DisplayLangue() => backupView.DisplayLangue();

        public void PauseAndReturn()
        {
            backupView.DisplayMessage("PressKeyToReturn");
            Console.ReadKey();
        }

        public void Choice_Type_File_Log()
        {
            try
            {
                string typeNow = controller_log.Get_Type_File();
                backupView.Get_Type_Log(typeNow);
                string reponse = backupView.Type_File_Log();
                controller_log.Type_File_Log(reponse);
                PauseAndReturn();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur lors du choix du type de log : " + ex.Message);
            }
        }

        public void ConfigureEncryption()
        {
            BackupJob_Models task = backupView.GetBackupDetails();
            stopwatch.Start();
            string cond = backupModel.CreateBackupTask(task);
            stopwatch.Stop();
            string formattedTime = stopwatch.Elapsed.ToString(@"hh\:mm\:ss\.fff");
            string t = controller_log.Get_Type_File();
            if (cond == "ok")
            {
                try
                {
                    controller_log.LogBackupAction(task.Name, task.SourceDirectory, task.TargetDirectory, formattedTime, "Create Task");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Erreur lors du log: " + ex.Message);
                }
            }
            else if (cond == "ko")
            {
                try
                {
                    controller_log.LogBackupErreur(task.Name, "create_task_attempt", "The user entered 'no' during the confirmation.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Erreur lors du log: " + ex.Message);
                }
            }
        }

        public void DeleteTask()
        {
            Console.Clear();
            ViewTasks();
            stopwatch.Start();
            BackupJob_Models task = backupModel.DeleteTask();
            if (task == null) { return; }
            stopwatch.Stop();
            string formattedTime = stopwatch.Elapsed.ToString(@"hh\:mm\:ss\.fff");
            string t = controller_log.Get_Type_File();
            controller_log.LogBackupAction(task.Name, task.SourceDirectory, task.TargetDirectory, formattedTime, "Delete Task");
            PauseAndReturn();
        }

        public void ViewTasks()
        {
            try
            {
                BackupJob_Models.ViewTasks();
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur lors de l'affichage des tâches : " + ex.Message);
            }
        }

        public void ExecuteSpecificTask()
        {
            Console.Clear();
            backupView.DisplayMessage("ExecuteSpecificTask");
            // Configure encryption before executing the backup
            ConfigureEncryption();
            stopwatch.Start();
            var tasks = backupModel.ExecuteSpecificTask();
            if (tasks == null || tasks.Count == 0) { return; }
            stopwatch.Stop();
            string formattedTime = stopwatch.Elapsed.ToString(@"hh\:mm\:ss\.fff");
            foreach (var task in tasks)
            {
                try
                {
                    string logType = task.Type == BackupType.Full ? "Executing Full Backup" : "Executing Differential Backup";
                    controller_log.LogBackupAction(task.Name, task.SourceDirectory, task.TargetDirectory, formattedTime, "execute specific Task");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Erreur lors du log: " + ex.Message);
                }
            }
        }

        public void ExecuteAllTasks()
        {
            Console.Clear();
            backupView.DisplayMessage("ExecuteAllTasks");
            // Configure encryption before executing the backup
            ConfigureEncryption();
            stopwatch.Start();
            var executedTasks = backupModel.ExecuteAllTasks();
            stopwatch.Stop();
            if (executedTasks == null) { return; }
            string formattedTime = stopwatch.Elapsed.ToString(@"hh\:mm\:ss\.fff");
            string t = controller_log.Get_Type_File();
            foreach (var task in executedTasks)
            {
                controller_log.LogBackupAction(task.Name, task.SourceDirectory, task.TargetDirectory, formattedTime, "Execute all Task");
            }
        }

        public void DecryptFolder()
        {
            try
            {
                backupView.DisplayMessage("EnterDecryptionPassword");
                string password = backupView.ReadPassword();
                if (string.IsNullOrEmpty(password))
                {
                    backupView.DisplayMessage("PasswordEmpty");
                    PauseAndReturn();
                    return;
                }
                backupView.DisplayMessage("DecryptFolderPrompt");
                string folderPath = backupView.BrowsePath(canChooseFiles: false, canChooseDirectories: true);
                if (string.IsNullOrEmpty(folderPath))
                {
                    backupView.DisplayMessage("NoFolderSelected");
                    PauseAndReturn();
                    return;
                }
                EncryptionUtility.SetEncryptionSettings(password, true, new string[0], true);
                string[] encryptedFiles = System.IO.Directory.GetFiles(folderPath, "*.aes", System.IO.SearchOption.AllDirectories);
                bool errorOccurred = false;
                foreach (string file in encryptedFiles)
                {
                    bool success = EncryptionUtility.DecryptFileWithResult(file);
                    if (!success)
                        errorOccurred = true;
                }
                if (errorOccurred)
                    backupView.DisplayMessage("DecryptionError");
                else
                    backupView.DisplayMessage("FolderDecrypted");
                PauseAndReturn();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur lors du déchiffrement du dossier : " + ex.Message);
            }
        }

        public void ErreurChoix()
        {
            backupView.DisplayMessage("InvalidOption");
        }

        public void DisplayMainMenu()
        {
            backupView.DisplayMainMenu();
        }
    }
}
