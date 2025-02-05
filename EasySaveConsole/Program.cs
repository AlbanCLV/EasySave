using System;
using EasySave.Controllers;
using EasySave.Utilities;

namespace EasySave
{
    /// <summary>
    /// Entry point for the EasySave application.
    /// </summary>
    class Program
    {
        public static string SelectedLanguage { get; private set; }
        static void Main(string[] args)
        {
           
            // Initialize the controller with the selected language
            BackupJob_Controller controller1 = new BackupJob_Controller();
            SelectedLanguage = controller1.DisplayLangue();
            BackupJob_Controller controller = new BackupJob_Controller();

            while (true)
            {
                controller.DisplayMainMenu();
                string input = Console.ReadLine()?.Trim(); // Trim to avoid leading/trailing spaces

                switch (input)
                {
                    case "1":
                        controller.CreateBackupTask();
                        break;
                    case "2":
                        controller.ExecuteSpecificTask();
                        break;
                    case "3":
                        controller.ExecuteAllTasks();
                        break;
                    case "4":
                        controller.ViewTasks();
                        break;
                    case "5":
                        controller.DeleteTask();
                        break;
                    case "6":
                        Environment.Exit(0);
                        break;
                    default:
                        controller.ErreurChoix();
                        break;
                }
            }
        }
    }
}

