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
            try
            {
                var encryptionSettings = backupView.GetEncryptionSettings();
                EncryptionUtility.SetEncryptionSettings(
                    encryptionSettings.password,
                    encryptionSettings.encryptAll,
                    encryptionSettings.selectedExtensions,
                    encryptionSettings.encryptEnabled);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur lors de la configuration du chiffrement : " + ex.Message);
            }
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
                    controller_log.LogBackupAction(task.Name, task.SourceDirectory, task.TargetDirectory, formattedTime, "Create Task");
                else
                    controller_log.LogBackupErreur(task.Name, "create_task_attempt", "User canceled task creation.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur lors de la création de la tâche : " + ex.Message);
            }
        }

        public void DeleteTask()
        {
            try
            {
                Console.Clear();
                BackupJob_Models.ViewTasks();
                stopwatch.Restart();
                BackupJob_Models deletedTask = BackupJob_Models.DeleteTask();
                if (deletedTask == null)
                    return;
                stopwatch.Stop();
                string formattedTime = stopwatch.Elapsed.ToString(@"hh\:mm\:ss\.fff");
                controller_log.LogBackupAction(deletedTask.Name, deletedTask.SourceDirectory, deletedTask.TargetDirectory, formattedTime, "Delete Task");
                PauseAndReturn();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur lors de la suppression de la tâche : " + ex.Message);
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
                Console.WriteLine("Erreur lors de l'affichage des tâches : " + ex.Message);
            }
        }

        public void ExecuteSpecificTask()
        {
            try
            {
                Console.Clear();
                backupView.DisplayMessage("ExecuteSpecificTask");
                ConfigureEncryption();
                stopwatch.Restart();
                BackupJob_Models.ExecuteSpecificTask();
                stopwatch.Stop();
                PauseAndReturn();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur lors de l'exécution de la tâche spécifique : " + ex.Message);
            }
        }

        public void ExecuteAllTasks()
        {
            try
            {
                Console.Clear();
                backupView.DisplayMessage("ExecuteAllTasks");
                ConfigureEncryption();
                stopwatch.Restart();
                BackupJob_Models.ExecuteAllTasks();
                stopwatch.Stop();
                PauseAndReturn();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur lors de l'exécution de toutes les tâches : " + ex.Message);
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
