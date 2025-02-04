using System;
using EasySave.Controllers;

namespace EasySave
{
    class Program
    {
        static void Main(string[] args)
        {
            BackupJob_Controller controller_Tasks = new BackupJob_Controller();

            while (true)
            {

                string input = Console.ReadLine();

                switch (input)
                {
                    case "1": // Créer une tâche de sauvegarde
                        controller_Tasks.CreateBackupTask();

                        break;
                    case "2": // Exécuter une tâche spécifique
                        Console.Clear();
                        Console.WriteLine("Execute a Backup Task\n");
                        controller_Tasks.ExecuteSpecificTask();
                        break;
                    case "3": // Exécuter toutes les tâches
                        controller_Tasks.ExecuteAllTasks();
                        break;
                    case "4": // Voir toutes les tâches
                        controller_Tasks.ViewTasks();
                        break;
                    case "5": // Supprimer une tâche
                        Console.Write("Delete a backup task \n");
                        controller_Tasks.DeleteTask();
                        break;
                    case "6": // Quitter l'application
                        return;
                    default:
                        controller_Tasks.ErreurChoixMenu();
                        break;
                }
            }

        }
    }
}
