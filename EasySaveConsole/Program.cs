using System;
using EasySave.Services;
using EasySave.Models;
using EasySave.Utilities;
using EasySave.Controllers;
using EasySave.Views;


namespace EasySave
{
    class Program
    {
        static void Main(string[] args)
        {
            // Initialize the backup manager
            BackupJob_Services backupManager = new BackupJob_Services();
            BackupJob_View BackupView = new BackupJob_View();

            while (true)
            {
                BackupView.DisplayMainMenu();
                string input = Console.ReadLine();

                switch (input)
                {
                    case "1": // Option to create a backup task
                        var task = BackupView.GetBackupDetails();
                        backupManager.CreateBackupTask(task);
                        BackupView.DisplayMessage("Backup task created successfully.");
                        PauseAndReturn(); // Pause before returning to the menu
                        break;
                    case "2": // Option to execute a specific backup task
                        Console.Clear(); // Clear the console before execution
                        backupManager.ViewTasks();
                        backupManager.ExecuteSpecificTask();
                        PauseAndReturn();
                        break;
                    case "3": // Option to execute all tasks sequentially
                        Console.Clear(); // Clear the console before execution
                        backupManager.ExecuteAllTasks();
                        PauseAndReturn();
                        break;
                    case "4": // Option to display all backup tasks
                        Console.Clear(); // Clear the console before execution
                        backupManager.ViewTasks();
                        PauseAndReturn();
                        break;
                    case "5": // Option to delete a backup task
                        Console.Clear(); // Clear the console before execution
                        backupManager.DeleteTask();
                        PauseAndReturn();
                        break;
                    case "6": // Option to exit the application
                        Console.Clear(); // Clear the console before exiting
                        Console.WriteLine("Exiting EasySave...");
                        return; // Exit the program
                    default: // Invalid option entered by the user
                        Console.Clear(); // Clear the console before displaying the error
                        Console.WriteLine("Invalid option. Please try again."); // Inform the user about the invalid input
                        PauseAndReturn(); // Pause before returning to the menu
                        break;
                }
            }
        }

        // Method to display a message and wait for the user to press a key before returning to the menu
        static void PauseAndReturn()
        {
            Console.WriteLine("\nPress any key to return to the menu...");
            Console.ReadKey(); // Wait for the user to press a key
        }
    }
}
