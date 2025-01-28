using System;

namespace EasySave
{
    class Program
    {
        static void Main(string[] args)
        {
            BackupManager backupManager = new BackupManager();

            Console.WriteLine("Welcome to EasySave!");
            Console.WriteLine("1. Create a backup task");
            Console.WriteLine("2. Execute a backup task");
            Console.WriteLine("3. Execute all tasks sequentially");
            Console.WriteLine("4. Exit");

            while (true)
            {
                Console.Write("\nSelect an option: ");
                string input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        backupManager.CreateBackupTask();
                        break;
                    case "2":
                        backupManager.ExecuteSpecificTask();
                        break;
                    case "3":
                        backupManager.ExecuteAllTasks();
                        break;
                    case "4":
                        Console.WriteLine("Exiting EasySave...");
                        Console.ReadLine();
                        Environment.Exit(0);
                        Console.ReadLine();
                        break;
                    default:
                        Console.WriteLine("Invalid option. Try again.");
                        break;
                }
            }
        }
    }
}
