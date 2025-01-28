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

            while (true)
            {
                // Clear the console before displaying the menu
                Console.Clear();

                // Display the main menu
                Console.WriteLine("Welcome to EasySave!");
                Console.WriteLine("1. Create a backup task"); // Option to create a backup task
                Console.WriteLine("2. Execute a backup task"); // Option to execute a specific backup task
                Console.WriteLine("3. Execute all tasks sequentially"); // Option to execute all tasks sequentially
                Console.WriteLine("4. View all backup tasks"); // Option to display all backup tasks
                Console.WriteLine("5. Delete a backup task"); // Option to delete a backup task
                Console.WriteLine("6. Exit"); // Option to exit the program

                // Prompt the user to select an option
                Console.Write("\nSelect an option: ");
                string input = Console.ReadLine();

                switch (input)
                {
                    case "1": // Option to create a backup task
                        Console.Clear(); // Clear the console before execution
                        Console.WriteLine("Create a Backup Task\n");
                        backupManager.CreateBackupTask(); // Call the method to create a new backup task
                        PauseAndReturn(); // Pause before returning to the menu
                        break;
                    case "2": // Option to execute a specific backup task
                        Console.Clear(); // Clear the console before execution
                        Console.WriteLine("Execute a Backup Task\n");
                        backupManager.ExecuteSpecificTask(); // Call the method to execute a specific task
                        PauseAndReturn();
                        break;
                    case "3": // Option to execute all tasks sequentially
                        Console.Clear(); // Clear the console before execution
                        Console.WriteLine("Execute All Backup Tasks\n");
                        backupManager.ExecuteAllTasks(); // Call the method to execute all tasks
                        PauseAndReturn();
                        break;
                    case "4": // Option to display all backup tasks
                        Console.Clear(); // Clear the console before execution
                        Console.WriteLine("View All Backup Tasks\n");
                        backupManager.ViewTasks(); // Call the method to view all tasks
                        PauseAndReturn();
                        break;
                    case "5": // Option to delete a backup task
                        Console.Clear(); // Clear the console before execution
                        Console.WriteLine("Delete a Backup Task\n");
                        backupManager.DeleteTask(); // Call the method to delete a specific task
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
