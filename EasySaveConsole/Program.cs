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
            BackupJob_Controller controller_Tasks = new BackupJob_Controller();

            while (true)
            {

                string input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        controller_Tasks.CreateBackupTask();

                        break;
                    case "2":
                        Console.Clear();
                        Console.WriteLine("Execute a Backup Task\n");
                        controller_Tasks.ExecuteSpecificTask();
                        break;
                    case "3":
                        controller_Tasks.ExecuteAllTasks();
                        break;
                    case "4":
                        controller_Tasks.ViewTasks();
                        break;
                    case "5":
                        Console.Write("Delete a backup task \n");
                        controller_Tasks.DeleteTask();
                        break;
                    case "6":
                        return;
                    default:
                        controller_Tasks.ErreurChoixMenu();
                        break;
                }
            }

        }
    }
}
