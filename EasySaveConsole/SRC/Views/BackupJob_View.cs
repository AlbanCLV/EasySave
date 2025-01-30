// Gère l'affichage des menus pour interagir avec l'utilisateur via la console.

// Ce dossier contient les éléments d'affichage ou d'interface utilisateur. Dans la version 1.0, il s'agit d'une application console.

using System;
using System.Threading.Tasks;
using EasySave.Controllers;  // Si tu fais des appels aux contrôleurs depuis la vue
using EasySave.Models;  // Si tu fais des appels aux contrôleurs depuis la vue

namespace EasySave.Views
{
    public class BackupJob_View
    {
        public void DisplayMainMenu()
        {
            Console.Clear();
            Console.WriteLine("\r\n /$$$$$$$$                                /$$$$$$                               \r\n| $$_____/                               /$$__  $$                              \r\n| $$        /$$$$$$   /$$$$$$$ /$$   /$$| $$  \\__/  /$$$$$$  /$$    /$$ /$$$$$$ \r\n| $$$$$    |____  $$ /$$_____/| $$  | $$|  $$$$$$  |____  $$|  $$  /$$//$$__  $$\r\n| $$__/     /$$$$$$$|  $$$$$$ | $$  | $$ \\____  $$  /$$$$$$$ \\  $$/$$/| $$$$$$$$\r\n| $$       /$$__  $$ \\____  $$| $$  | $$ /$$  \\ $$ /$$__  $$  \\  $$$/ | $$_____/\r\n| $$$$$$$$|  $$$$$$$ /$$$$$$$/|  $$$$$$$|  $$$$$$/|  $$$$$$$   \\  $/  |  $$$$$$$\r\n|________/ \\_______/|_______/  \\____  $$ \\______/  \\_______/    \\_/    \\_______/\r\n                               /$$  | $$                                        \r\n                              |  $$$$$$/                                        \r\n                               \\______/                                         \r\n");
            Console.WriteLine("Welcome to EasySave!");
            Console.WriteLine("1. Create a backup task");
            Console.WriteLine("2. Execute a backup task");
            Console.WriteLine("3. Execute all tasks sequentially");
            Console.WriteLine("4. View all backup tasks");
            Console.WriteLine("5. Delete a backup task");
            Console.WriteLine("6. Exit");
            Console.Write("\nSelect an option: ");
        }

        public BackupJobModel GetBackupDetails()
        {
            Console.Clear(); // Clear the console before execution
            Console.Write("Enter task name: ");
            string name = Console.ReadLine();

            Console.Write("Enter source directory: ");
            string source = Console.ReadLine();

            Console.Write("Enter target directory: ");
            string target = Console.ReadLine();

            Console.Write("Enter type (1 = Full, 2 = Differential): ");
            int typeInput = int.Parse(Console.ReadLine() ?? "1");
            BackupType type = typeInput == 1 ? BackupType.Full : BackupType.Differential;

            return new BackupJobModel(name, source, target, type);
        }

        public string GetTaskName()
        {
            Console.Write("Enter the name of the task: ");
            return Console.ReadLine();
        }

        public void DisplayMessage(string message)
        {
            Console.WriteLine(message);
        }
    }
}