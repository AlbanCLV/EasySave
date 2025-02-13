using System;
using System.Diagnostics;
using EasySave.Models;
using EasySave.Views;
using EasySaveConsole.Utilities;
using EasySaveLog;

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
        /// </summary>
        public void PauseAndReturn()
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
        /// Configure les paramètres de chiffrement en demandant à l'utilisateur.
        /// </summary>
        public (bool encryptEnabled, string password, bool encryptAll, string[] selectedExtensions) ConfigureEncryption()
        {
            var encryptionSettings = backupView.GetEncryptionSettings();
            EncryptionUtility.SetEncryptionSettings(encryptionSettings.password, encryptionSettings.encryptAll, encryptionSettings.selectedExtensions, encryptionSettings.encryptEnabled);
            return encryptionSettings;
        }

        /// <summary>
        /// Creates a backup task based on the user's input.
        /// </summary>
        public void CreateBackupTask()
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
                    controller_log.LogBackupAction(task.Name, task.SourceDirectory, task.TargetDirectory, formattedTime, "Create Task", "N/A");
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
                    controller_log.LogBackupErreur(task.Name, "create_task_attempt", "The user entered 'no' during the confirmation.", "N/A");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Erreur lors du log: " + ex.Message);
                }
            }
        }

        /// <summary>
        /// Deletes a backup task.
        /// </summary>
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
            controller_log.LogBackupAction(task.Name, task.SourceDirectory, task.TargetDirectory, formattedTime, "Delete Task", "N/A");
            PauseAndReturn();
        }

        /// <summary>
        /// Displays all available backup tasks.
        /// </summary>
        public void ViewTasks()
        {
            backupModel.ViewTasks();
            Console.ReadKey();
        }

        /// <summary>
        /// Executes a specific backup task selected by the user.
        /// </summary>
        public void ExecuteSpecificTask()
        {
            Console.Clear();
            backupView.DisplayMessage("ExecuteSpecificTask");
            // Configure encryption before executing the backup
            var encryption = ConfigureEncryption();
            stopwatch.Start();
            var tasks = backupModel.ExecuteSpecificTask(encryption.encryptEnabled);
            if (tasks == null || tasks.Count == 0) { return; }
            stopwatch.Stop();
            string formattedTime = stopwatch.Elapsed.ToString(@"hh\:mm\:ss\.fff");
            foreach (var task in tasks)
            {
                try
                {
                    string logType = task.Type == BackupType.Full ? "Executing Full Backup" : "Executing Differential Backup";
                    controller_log.LogBackupAction(task.Name, task.SourceDirectory, task.TargetDirectory, formattedTime, logType, encryption.encryptEnabled.ToString());
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Erreur lors du log: " + ex.Message);
                }
            }
            PauseAndReturn();
        }

        /// <summary>
        /// Executes all backup tasks sequentially.
        /// </summary>
        public void ExecuteAllTasks()
        {
            Console.Clear();
            backupView.DisplayMessage("ExecuteAllTasks");
            // Configure encryption before executing the backup
            var encryption = ConfigureEncryption();
            stopwatch.Start();
            var executedTasks = backupModel.ExecuteAllTasks(encryption.encryptEnabled);
            stopwatch.Stop();
            if (executedTasks == null) { return; }
            string formattedTime = stopwatch.Elapsed.ToString(@"hh\:mm\:ss\.fff");
            string t = controller_log.Get_Type_File();
            foreach (var task in executedTasks)
            {
                controller_log.LogBackupAction(task.Name, task.SourceDirectory, task.TargetDirectory, formattedTime, "Execute all Task", encryption.encryptEnabled.ToString());
            }
            PauseAndReturn();
        }

        /// <summary>
        /// Déchiffre l'intégralité d'un dossier choisi par l'utilisateur.
        /// </summary>
        public void DecryptFolder()
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
            // Configurer le chiffrement pour le déchiffrement (on force EncryptAll=true)
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

        /// <summary>
        /// Handles invalid menu option input from the user.
        /// </summary>
        public void ErreurChoix()
        {
            backupView.DisplayMessage("InvalidOption");
        }

        /// <summary>
        /// Displays the main menu for the user to interact with.
        /// </summary>
        public void DisplayMainMenu()
        {
            backupView.DisplayMainMenu();
        }
    }
}
