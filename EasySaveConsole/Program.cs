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
        // Property to store the selected language for the application
        public static string SelectedLanguage { get; private set; }

        /// <summary>
        /// Main method that serves as the entry point of the application.
        /// Initializes the application and handles the main menu and user interactions.
        /// </summary>
        static void Main(string[] args)
        {
            // Initialize the controller and display the language selection screen
            BackupJob_Controller controller1 = new BackupJob_Controller();
            Console.Clear();  // Clear the console for a fresh display
            SelectedLanguage = controller1.DisplayLangue();  // Store the selected language
            BackupJob_Controller controller = new BackupJob_Controller();

            // Infinite loop to keep displaying the menu until the user decides to exit
            while (true)
            {
                // Display the main menu
                controller.DisplayMainMenu();

                // Get user input and trim any extra spaces
                string input = Console.ReadLine()?.Trim();

                // Switch case to handle different user inputs for menu options
                switch (input)
                {
                    case "1":
                        // Create a new backup task
                        controller.CreateBackupTask();
                        break;
                    case "2":
                        // Execute a specific backup task
                        controller.ExecuteSpecificTask();
                        break;
                    case "3":
                        // Execute all backup tasks
                        controller.ExecuteAllTasks();
                        break;
                    case "4":
                        // View all existing tasks
                        controller.ViewTasks();
                        break;
                    case "5":
                        // Delete a backup task
                        controller.DeleteTask();
                        break;
                    case "6":
                        controller.Choice_Type_File_Log();
                        break;
                    case "7":
                        // Exit the application
                        Environment.Exit(0);
                        break;
                    default:
                        // Handle invalid menu choice
                        controller.ErreurChoix();
                        break;
                }
            }
        }
    }
}
