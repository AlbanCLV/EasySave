using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using EasySaveConsole.Models;
using EasySaveConsole.Views;
using EasySaveLog;
using System.IO;


namespace EasySaveConsole.ViewModels
{
    /// <summary>
    /// Controller class for managing interactions between the BackupJob model and view.
    /// This class handles the creation, execution, and management of backup tasks.
    /// </summary>
    public class BackupJob_ViewModels
    {
        // Singleton instance
        private static BackupJob_ViewModels _instance;
        private static readonly object _lock = new object();

        private Backup_Models backupModel; // Instance du contrôleur
        private MainView MainView;
        private Log_ViewModels Log_VM;
        private State_models StateModels;
        private Stopwatch stopwatch = new Stopwatch();
        private Encryption_Models EncryptionModels;
        private ProcessWatcher processWatcher;
        private LangManager lang;

        /// <summary>
        /// Private constructor to prevent direct instantiation.
        /// </summary>
        public BackupJob_ViewModels( )
        {
            MainView = MainView.Instance;
            backupModel = Backup_Models.Instance;  // Initialiser le contrôleur
            Log_VM = Log_ViewModels.Instance;
            StateModels = State_models.Instance;
            EncryptionModels = Encryption_Models.Instance;
            processWatcher = ProcessWatcher.Instance;
            lang = LangManager.Instance;
            backupModel.LoadTasks();
        }
        /// <summary>
        /// Gets the singleton instance of the BackupJob_ViewModels class.
        /// </summary>
        public static BackupJob_ViewModels Instance
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
                            _instance = new BackupJob_ViewModels();
                        }
                    }
                }
                return _instance;
            }
        }
        /// <summary>
        /// Displays the language selection screen and returns the chosen language.
        /// </summary>
        public string DisplayLangue() => MainView.DisplayLangue();
        public void StartWatch() =>   processWatcher.StartWatching();
        public void StopWatching() =>  processWatcher.StopWatching();
        /// <summary>
        /// Pauses execution and waits for the user to press a key before returning to the menu.
        /// This is used for user interaction after an operation is complete.
        /// </summary>
        public void PauseAndReturn()
        {
            MainView.DisplayMessage("PressKeyToReturn");
            Console.ReadKey();
        }

        public void Choice_Type_File_Log()
        {
            try
            {
                string Type_Now = Log_VM.Get_Type_File();
                MainView.Get_Type_Log(Type_Now);
                string reponse = MainView.GET_Type_File_Log();
                Log_VM.Type_File_Log(reponse);
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
        public bool ConfigureEncryption()
        {
            try
            {
                var encryptionSettings = MainView.GetEncryptionSettings();
                EncryptionModels.SetEncryptionSettings(encryptionSettings.password, encryptionSettings.encryptAll, encryptionSettings.selectedExtensions, encryptionSettings.encryptEnabled);

                return encryptionSettings.encryptEnabled;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error ConfigureEncryption: " + ex.Message);
                return false;
            }
        }
        /// <summary>
        /// Creates a backup task based on the user's input.
        /// Records the time taken to create the task and logs the action.
        /// </summary>
        public void CreateBackupTask()
        {
            try
            {
                (Backup_Models task, string con) = MainView.GetBackupDetails();  // Get task details from user
                stopwatch.Start();
                string cond = backupModel.CreateBackupTask(task, con);  // Create the backup task
                stopwatch.Stop();
                string formattedTime = stopwatch.Elapsed.ToString(@"hh\:mm\:ss\.fff");  // Format elapsed time
           
                if (cond == "OK")
                {
                    try
                    {
                        Log_VM.LogBackupAction(task.Name, task.SourceDirectory, task.TargetDirectory, formattedTime, "Create Task", "-1");
                        MainView.DisplayMessage("TaskCreated");  // Notify user of task creation
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error during log: " + ex.Message);
                    }
                }
                else if (cond == "KO")
                {
                    try
                    {
                        Log_VM.LogBackupErreur(task.Name, "create_task_attempt", "User cancelled task creation.", "-1");
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
            PauseAndReturn();  // Wait for user input before returning to the menu
        }

        /// <summary>
        /// Deletes a backup task.
        /// The user is prompted to view tasks before deleting one, and the action is logged.
        /// </summary>
        public void DeleteTask()
        {
            Console.Clear();
            ViewTasks();  // Display current tasks for the user to select from
            int taskdelete = MainView.GetDeleteTask();
            (Backup_Models task , string cond, string e) = backupModel.DeleteTask(taskdelete);  // Delete the selected task
            string formattedTime = e;  // Format elapsed time
            if (cond == "OTask")
                {
                try
                {
                    MainView.DisplayMessage("no_tasks_to_delete");
                    Log_VM.LogBackupErreur("Error", "delete_task_attempt", "no_tasks_to_delete", "-1");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error during log: " + ex.Message);
                }
            }
            else if(cond == "Invalid")
                {
                try
                {
                    MainView.DisplayMessage("invalid_task_number");
                    Log_VM.LogBackupErreur("Error", "delete_task_attempt","invalid_task_number", "-1");  // Log the action
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error during log: " + ex.Message);
                }
            }
            else if (cond == "OK")
            {
                try
                {
                    MainView.DisplayMessage("deleting_task");
                    MainView.DisplayMessage("task_deleted_successfully");
                    Log_VM.LogBackupAction(task.Name, task.SourceDirectory, task.TargetDirectory, formattedTime, "Delete Task", "-1");  // Log the action
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error during log: " + ex.Message);
                }
            }
            PauseAndReturn();  // Wait for user input before returning to the menu
        }

        /// <summary>
        /// Displays all available backup tasks.
        /// Allows the user to review existing tasks.
        /// </summary>
        public void ViewTasks()
        {
            try
            {
                string cond = backupModel.ViewTasks();  // Display all tasks
                if (cond == "OTask")
                {
                    try
                    {
                        Log_VM.LogBackupErreur("Error", "View_task_attempt", "No_tasks", "-1");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error during log: " + ex.Message);
                    }
                }
                else if (cond == "OK")
                {
                    try
                    {
                        Log_VM.LogBackupAction("-1", "-1", "-1","-1" ,"View Task","-1");  // Log the action
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error during log: " + ex.Message);
                    }
                }
                Console.ReadKey();  // Wait for user input before proceeding

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error viewing tasks: " + ex.Message);
            }
        }

        /// <summary>
        /// Executes a specific backup task selected by the user.
        /// The task execution time is logged, and a message is displayed based on the type of backup.
        /// </summary>
        public void ExecuteSpecificTask()
        {
            try
            {
                if (processWatcher.IsBusinessApplicationRunning()){return;}
                stopwatch.Start();
                bool cond2 = ConfigureEncryption();
                stopwatch.Stop();
                string EncryptionTime = stopwatch.ElapsedMilliseconds.ToString(); // Temps écoulé en millisecondes
                if (cond2 == false) 
                { 
                    EncryptionTime = "0"; 
                }
                int c = MainView.GetExecuteTasks();
                stopwatch.Start();
                (Backup_Models task, string cond) = backupModel.ExecuteSpecificTask(c);  // Execute the selected backup task
                stopwatch.Stop();

                string formattedTime = stopwatch.Elapsed.ToString(@"hh\:mm\:ss\.fff");  // Format elapsed time
                if (cond == "0Task")
                {
                    try
                    {
                        MainView.DisplayMessage("no_tasks_to_execute");
                        Log_VM.LogBackupErreur("Error", "Execute_Specific_Task_attempt", "No_tasks", EncryptionTime);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error during log: " + ex.Message);
                    }
                }
                else if (cond == "Invalid")
                {
                    try
                    {
                        MainView.DisplayMessage("invalid_task_number");
                        Log_VM.LogBackupErreur("Error", "Execute_Specific_Task_attempt", "Invalid_Task_Number", EncryptionTime);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error during log: " + ex.Message);
                    }
                }
                else if (cond == "Source")
                {
                    try
                    {
                        Log_VM.LogBackupErreur(task.Name, "execute_task_attempt", "source_directory_not_exist", EncryptionTime);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error during log: " + ex.Message);
                    }
                }
                else if (cond == "OK")
                {
                    try
                    {
                        Log_VM.LogBackupAction(task.Name, task.SourceDirectory, task.TargetDirectory, formattedTime, "execute specific Task", EncryptionTime);  // Log the action
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error during log: " + ex.Message);
                    }

                }
                else if (cond == "KO")
                {
                    try
                    {
                        Log_VM.LogBackupErreur(task.Name, "execute_task_attempt", "Canceled", EncryptionTime);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error during log: " + ex.Message);
                    }

                }
                PauseAndReturn();  // Wait for user input before returning to the menu

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error executing specific task: " + ex.Message);
            }
        }

        /// <summary>
        /// Executes all backup tasks sequentially.
        /// Logs each task's execution and the total time taken.
        /// </summary>
        public void ExecuteAllTasks()
        {
            try
            {
                Console.Clear();
                stopwatch.Start();
                bool cond2 = ConfigureEncryption();
                stopwatch.Stop();
                string EncryptionTime = stopwatch.Elapsed.ToString(@"hh\:mm\:ss\.fff");  // Format elapsed time
                if (cond2 == false)
                {
                    EncryptionTime = "0";
                }
                MainView.DisplayMessage("ExecuteAllTasks");  // Notify the user that all tasks will be executed
                stopwatch.Start();
                (List<Backup_Models> executedTasks, List<string> logMessages) = backupModel.ExecuteAllTasks();  // Execute all tasks
                stopwatch.Stop();
                string formattedTime = stopwatch.Elapsed.ToString(@"hh\:mm\:ss\.fff");  // Format elapsed time
                Console.WriteLine(logMessages);
                for (int i = 0; i < executedTasks.Count; i++)
                {
                    if (processWatcher.IsBusinessApplicationRunning())
                    {
                        Console.WriteLine($"Logiciel métier détecté ! La sauvegarde '{executedTasks[i].Name}' est suspendue.");
                        break; // Arrêter la sauvegarde après la tâche en cours
                    }
                    if (logMessages[i] == "0Task")
                    {
                        try
                        {
                            Log_VM.LogBackupErreur("Error", "Execute_All_Task_attempt", "No_tasks", EncryptionTime);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error during log: " + ex.Message);
                        }
                    }
                    else if (logMessages[i] == "Invalid")
                    {
                        try
                        {
                            Log_VM.LogBackupErreur("Error", "Execute_All_Task_attempt", "Invalid_Task_Number", EncryptionTime);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error during log: " + ex.Message);
                        }
                    }
                    else if (logMessages[i] == "Source")
                    {
                        try
                        {
                            Log_VM.LogBackupErreur(executedTasks[i].Name, "Execute_All_Task_attempt", "source_directory_not_exist", EncryptionTime);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error during log: " + ex.Message);
                        }
                    }
                    else if (logMessages[i] == "OK")
                    {
                        try
                        {
                            Log_VM.LogBackupAction(executedTasks[i].Name, executedTasks[i].SourceDirectory, executedTasks[i].TargetDirectory, formattedTime, "execute ALL Task", EncryptionTime);  // Log the action
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error during log: " + ex.Message);
                        }

                    }
                    else if (logMessages[i] == "KO")
                    {
                        try
                        {
                            Log_VM.LogBackupErreur(executedTasks[i].Name, "Execute_All_Task_attempt", "Canceled", EncryptionTime);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error during log: " + ex.Message);
                        }

                    }
                }

                PauseAndReturn();  // Wait for user input before returning to the menu
            }
            catch(Exception ex)
            {
                Console.WriteLine("Error executing all tasks: " + ex.Message);
            }
        }

        /// <summary>
        /// Handles invalid menu option input from the user.
        /// Displays an error message to guide the user back to a valid choice.
        /// </summary>
        public void ErreurChoix()
        {
            MainView.DisplayMessage("InvalidOption");  // Display error message for invalid input
        }

        /// <summary>
        /// Displays the main menu for the user to interact with.
        /// Allows the user to choose different operations.
        /// </summary>
        public void DisplayMainMenu()
        {
            MainView.DisplayMainMenu();  // Show the main menu to the user
        }

        public void DecryptFolder()
        {
            try
            {
                (string password, string folderPath) = MainView.GetDecryptFolder();
                if (password == "KO" || folderPath == "KO") 
                { 
                    Log_VM.LogBackupErreur("Decrypt Folder", "Decrypt Folder", "Candeled", "-1"); 
                }
                stopwatch.Start();
                EncryptionModels.SetEncryptionSettings(password, true, new string[0], true);
                string[] encryptedFiles = System.IO.Directory.GetFiles(folderPath, "*.aes", System.IO.SearchOption.AllDirectories);
                bool errorOccurred = false;
                foreach (string file in encryptedFiles)
                {
                    bool success = EncryptionModels.DecryptFileWithResult(file);
                    if (!success)
                        errorOccurred = true;
                }
                if (errorOccurred) {
                    MainView.DisplayMessage("DecryptionError");
                    Log_VM.LogBackupErreur("Decrypt Folder", "Decrypt Folder", "DecryptionError", "-1");
                }
                else 
                {
                    MainView.DisplayMessage("FolderDecrypted");
                    stopwatch.Stop();
                    string formattedTime = stopwatch.Elapsed.ToString(@"hh\:mm\:ss\.fff");  // Format elapsed time
                    Log_VM.LogBackupAction("....", "....", "....", formattedTime, "FolderDecrypted", "-1");
                }
                PauseAndReturn();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error decrypting folder: " + ex.Message);
            }
        }


        public void ManageBusinessApplication()
        {
            Console.Clear();
            processWatcher.DisplayExistingApplications();
            string condition = processWatcher.GetChoiceMetier();
            switch (condition)
            {
                case "1":
                    AddBusinessApplication();
                    break;
                case "2":
                    RemoveBusinessApplication();
                    break;
                case "3":
                    return;  // Sortir du sous-menu et revenir au menu principal
                default:
                    Console.WriteLine("\n❌ Option invalide. Appuyez sur une touche pour continuer...");
                    Console.ReadKey();
                    break;
            }
            
        }
        public void AddBusinessApplication()
        {
            Console.Clear();
            processWatcher.DisplayExistingApplications();
            Console.Write("Entrez le nom de l'application métier à surveiller : ");
            string appName = Console.ReadLine()?.Trim();

            processWatcher.AddBusinessApplication(appName);
        }

        public void RemoveBusinessApplication()
        {
            Console.Clear();
            processWatcher.RemoveBusinessApplication();
        }
    }
}
