using System;
using EasySave.Models;
using EasySave.Views;
using EasySave.Controllers;

namespace EasySave
{
    class Program
    {
        static void Main(string[] args)
        {
            // Créer les objets
            BackupJob_View view = new BackupJob_View();
            BackupJob_Models model = new BackupJob_Models("", "", "", BackupType.Full);  // On peut initialiser un modèle vide ici
            model.LoadTasks();  // Charger les tâches existantes à partir du fichier
            BackupJob_Controller controller = new BackupJob_Controller(view, model);

            // Boucle principale
            while (true)
            {
                view.DisplayMainMenu();
                string input = Console.ReadLine();

                switch (input)
                {
                    case "1": // Créer une tâche de sauvegarde
                        controller.CreateBackupTask();
                        PauseAndReturn();
                        break;
                    case "2": // Option to execute a specific backup task
                        Console.Clear(); // Clear the console before execution
                        Console.WriteLine("Execute a Backup Task\n");
                        controller.ExecuteSpecificTask(); // Call the method to execute a specific task
                        PauseAndReturn();
                        break;
                    case "3": // Exécuter toutes les tâches
                        controller.ExecuteAllTasks();
                        PauseAndReturn();
                        break;
                    case "4": // Voir toutes les tâches
                        controller.ViewTasks();
                        PauseAndReturn();
                        break;
                    case "5": // Supprimer une tâche
                        Console.Write("Delete a backup task \n");
                        controller.DeleteTask();
                        break;
                    case "6": // Quitter
                        return;
                    default:
                        view.DisplayMessage("Invalid option. Please try again.");
                        break;
                }
            }
        }
        static void PauseAndReturn()
        {
            Console.WriteLine("\nPress any key to return to the menu...");
            Console.ReadKey(); // Wait for the user to press a key
        }
    }
}
