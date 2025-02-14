using System;
using System.Diagnostics;
using EasySave.Models;
using EasySave.Views;
using EasySave.Log;
using EasySave.Utilities;  // For LangManager and EncryptionUtility
using EasySave.Controllers; // For State_Controller

namespace EasySave.Controllers
{
    /// <summary>
    /// Main controller that handles backup tasks (creation, execution, deletion)
    /// and integrates logging and state updates.
    /// </summary>
    public class BackupJob_Controller
    {
        private static BackupJob_Controller _instance;
        private static readonly object _lock = new object();
        private BackupJob_View backupView;
        private Log_Controller controller_log;
        private State_Controller controller_state;
        private Stopwatch stopwatch = new Stopwatch();

        // Private constructor for the singleton pattern.
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
                Console.WriteLine("Error choosing log type: " + ex.Message);
            }
        }

        /// <summary>
        /// Configures encryption settings based on user input.
        /// </summary>
        public void ConfigureEncryption()
        {
            var encryptionSettings = backupView.GetEncryptionSettings();
            EncryptionUtility.SetEncryptionSettings(
                encryptionSettings.password,
                encryptionSettings.encryptAll,
                encryptionSettings.selectedExtensions,
                encryptionSettings.encryptEnabled);
        }

        public void CreateBackupTask()
        {
            try
            {
                BackupJob_Models task = backupView.GetBackupDetails();
                stopwatch.Restart();
                string cond = BackupJob_Models.CreateBackupTask(task);
                stopwatch.Stop();
                string formattedTime = stopwatch.Elapsed.ToString(@"hh\:mm\:ss\.fff");

                if (cond == "ok")
                {
                    try
                    {
                        controller_log.LogBackupAction(task.Name, task.SourceDirectory, task.TargetDirectory, formattedTime, "Create Task");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error during log: " + ex.Message);
                    }
                }
                else if (cond == "ko")
                {
                    try
                    {
                        controller_log.LogBackupErreur(task.Name, "create_task_attempt", "User cancelled task creation.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error during log: " + ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error creating backup task: " + ex.Message);
            }
        }

        public void DeleteTask()
        {
            try
            {
                Console.Clear();
                ViewTasks();
                stopwatch.Restart();
                BackupJob_Models task = BackupJob_Models.DeleteTask();
                if (task == null)
                    return;
                stopwatch.Stop();
                string formattedTime = stopwatch.Elapsed.ToString(@"hh\:mm\:ss\.fff");
                controller_log.LogBackupAction(task.Name, task.SourceDirectory, task.TargetDirectory, formattedTime, "Delete Task");
                PauseAndReturn();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error deleting task: " + ex.Message);
            }
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
                Console.WriteLine("Error viewing tasks: " + ex.Message);
            }
        }

        public void ExecuteSpecificTask()
        {
            try
            {
                Console.Clear();
                backupView.DisplayMessage("ExecuteSpecificTask");
                // Configure encryption before executing the backup.
                ConfigureEncryption();
                stopwatch.Restart();
                // ExecuteSpecificTask is assumed to be a static method that executes the specific task.
                BackupJob_Models.ExecuteSpecificTask();
                stopwatch.Stop();
                PauseAndReturn();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error executing specific task: " + ex.Message);
            }
        }

        public void ExecuteAllTasks()
        {
            try
            {
                Console.Clear();
                backupView.DisplayMessage("ExecuteAllTasks");
                // Configure encryption before executing the backup.
                ConfigureEncryption();
                stopwatch.Restart();
                BackupJob_Models.ExecuteAllTasks();
                stopwatch.Stop();
                PauseAndReturn();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error executing all tasks: " + ex.Message);
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
                Console.WriteLine("Error decrypting folder: " + ex.Message);
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
