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
            BackupJob_Controller controller_Task = new BackupJob_Controller();
            Log_Controller controller_Log = new Log_Controller();

            /// <summary>
            /// Main method that manages the menu and navigation.
            /// </summary>
            while (true)
            {
                controller_Task.DisplayMainMenu();
                string input = Console.ReadLine();

                switch (input)
                {
                    case "1": // Create a backup task
                        controller_Task.CreateBackupTask();

                        break;
                    case "2": // Execute a specific task


                        controller_Task.ExecuteSpecificTask();
                        break;
                    case "3": // Execute all tasks
                        controller_Task.ExecuteAllTasks();
                        break;
                    case "4": // View all tasks
                        controller_Task.ViewTasks();
                        break;
                    case "5": // Delete a task
                        controller_Task.DeleteTask();
                        break;
                    case "6": // Exit the application
                        Environment.Exit(0); // 0 signifie une fermeture réussie
                        return;
                    default:
                        controller_Task.ErreurChoix();
                        break;
                }
            }
        }
    }
}
