using System;
using EasySave.Models;
using Terminal.Gui;

namespace EasySave.Views
{
    /// <summary>
    /// BackupJob View for user
    /// </summary>
    public class BackupJob_View
    {
        /// <summary>
        /// Load main menu in CLI
        /// </summary>
        public void DisplayMainMenu()
        {
            Console.Clear();
            Console.WriteLine("\r\n /$$$$$$$$                                /$$$$$$                               \r\n| $$_____/                               /$$__  $$                              \r\n| $$        /$$$$$$   /$$$$$$$ /$$   /$$| $$  \\__/  /$$$$$$  /$$    /$$ /$$$$$$ \r\n| $$$$$    |____  $$ /$$_____/| $$  | $$|  $$$$$$  |____  $$|  $$  /$$//$$__  $$\r\n| $$__/     /$$$$$$$|  $$$$$$ | $$  | $$ \\____  $$  /$$$$$$$ \\  $$/$$/| $$$$$$$$\r\n| $$       /$$__  $$ \\____  $$| $$  | $$ /$$  \\ $$ /$$__  $$  \\  $$$/ | $$_____/\r\n| $$$$$$$$|  $$$$$$$ /$$$$$$$/|  $$$$$$$|  $$$$$$/|  $$$$$$$   \\  $/  |  $$$$$$$\r\n|________/ \\_______/|_______/  \\____  $$ \\______/  \\_______/    \\_/    \\_______/\r\n                               /$$  | $$                                        \r\n                              |  $$$$$$/                                        \r\n                               \\______/                                         \r\n");
            Console.WriteLine("1. Create a backup task");
            Console.WriteLine("2. Execute a backup task");
            Console.WriteLine("3. Execute all tasks sequentially");
            Console.WriteLine("4. View all backup tasks");
            Console.WriteLine("5. Delete a backup task");
            Console.WriteLine("6. Exit");
            Console.Write("\nSelect an option: ");
        }

        /// <summary>
        /// Get backup details from model
        /// </summary>
        /// <returns>new BackupJob_Models(name, source, target, type)</returns>
        public BackupJob_Models GetBackupDetails()
        {
            Console.Clear(); // Clear the console before execution
            Console.WriteLine("=== Create a Backup Task ===");
            Console.Write("Enter task name: ");
            string name = Console.ReadLine();
            while (string.IsNullOrEmpty(name))
            {
                Console.WriteLine("Task name cannot be empty. Returning to menu...");
                
            }

            // Select source repository
            Console.WriteLine("\nSelect the source directory:");
            string source = BrowsePath(canChooseFiles: false, canChooseDirectories: true);
           while (string.IsNullOrEmpty(source))
            {
                Console.Write("Source directory cannot be empty. Please enter a valid source directory: ");
                source = Console.ReadLine();
            }

            Console.Write("Enter target directory: ");
            string target = BrowsePath(canChooseFiles: false, canChooseDirectories: true);
            while (string.IsNullOrEmpty(target))
            {
                Console.Write("Target directory cannot be empty. Please enter a valid target directory: ");
                target = Console.ReadLine();
            }

            Console.Write("Enter type (1 = Full, 2 = Differential): ");
            int typeInput;
            while (!int.TryParse(Console.ReadLine(), out typeInput) || (typeInput != 1 && typeInput != 2))
            {
                Console.Write("Invalid input. Please enter 1 for Full or 2 for Differential: ");
            }
            BackupType type = typeInput == 1 ? BackupType.Full : BackupType.Differential;

            return new BackupJob_Models(name, source, target, type);
        }

        /// <summary>
        /// Display message
        /// </summary>
        /// <param name="message">string message</param>
        public void DisplayMessage(string message)
        {
            Console.WriteLine(message);
        }

        /// <summary>
        /// Call interface to choose repositories in a graphical interface 
        /// </summary>
        /// <param name="canChooseFiles">bool for files that can be chosen</param>
        /// <param name="canChooseDirectories">bool for directories that can be chosen</param>
        /// <returns></returns>
        public string BrowsePath(bool canChooseFiles = false, bool canChooseDirectories = true)
        {
            Application.Init();

            var dialog = new OpenDialog("Select Path", "Choose a file or directory")
            {
                CanChooseFiles = canChooseFiles,
                CanChooseDirectories = canChooseDirectories
            };

            Application.Run(dialog);

            if (!string.IsNullOrEmpty(dialog.FilePath.ToString()))
            {
                Application.Shutdown();
                return dialog.FilePath.ToString();
            }

            Application.Shutdown();
            return null; // No path selected
        }
    }
}
