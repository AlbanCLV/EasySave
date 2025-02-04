using System;
using EasySave.Controllers;

namespace EasySave
{
    /// <summary>
    /// Main programm to call controller for interactions between Views and Models
    /// </summary>
    class Program
    {
        /// <summary>
        /// Call Controller
        /// </summary>
        /// <param name="args">CLI arguments</param>
        static void Main(string[] args)
        {
            // Controller will initiate Viexs and Models
            BackupJob_Controller controller = new BackupJob_Controller();

            /// <summary>
            /// Main method that manages the menu and navigation.
            /// </summary>
            while (true)
            {
                controller.DisplayMainMenu();
                string input = Console.ReadLine();

                switch (input)
                {
                    case "1": // Create a backup task
                        controller.CreateBackupTask();
                        break;
                    case "2": // Execute a specific task
                        
                        controller.ExecuteSpecificTask();
                        break;
                    case "3": // Execute all tasks
                        controller.ExecuteAllTasks();
                        break;
                    case "4": // View all tasks
                        controller.ViewTasks();
                        break;
                    case "5": // Delete a task
                        controller.DeleteTask();
                        break;
                    case "6": // Exit the application
                        return;
                    default:
                        controller.ErreurChoix();
                        break;
                }
            }
        }
    }
}
