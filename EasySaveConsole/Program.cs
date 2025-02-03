using System;
using EasySave.Controllers;

namespace EasySave
{
    class Program
    {
        static void Main(string[] args)
        {
            // Le contrôleur s'occupe de créer ses propres instances de la vue et du modèle
            BackupJob_Controller controller = new BackupJob_Controller();
            controller.Run();  // Démarre la boucle principale de l'application
        }
    }
}
