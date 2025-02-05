using System;
using EasySave.Controllers;

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
            // Set console output encoding to UTF-8
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            // Display language selection
            Console.WriteLine("Choose a language / Choisissez une langue:");
            Console.WriteLine("1. English");
            Console.WriteLine("2. Français");
            Console.Write("Enter your choice: ");
            SelectedLanguage = Console.ReadLine()?.Trim() == "2" ? "fr" : "en";

            // Initialize the controller with the selected language
            BackupJob_Controller controller = new BackupJob_Controller(SelectedLanguage);

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

