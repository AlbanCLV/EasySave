using System;
using EasySave.Models;
using System.Diagnostics;  // Pour utiliser Stopwatch
using EasySave.Views;

namespace EasySave.Controllers
{
    /// <summary>
    /// Controller for logging actions related to backup tasks.
    /// Manages the interaction between the Log model and the view.
    /// </summary>
    public class Log_Controller
    {
        private Log_Models logModel; // Instance of the Log_Models class to handle log operations.

        /// <summary>
        /// Constructor initializes the Log_Models instance.
        /// </summary>
        public Log_Controller()
        {
            logModel = new Log_Models(); // Initializes the log model to handle log actions.
        }

        /// <summary>
        /// Logs a backup action, including task details.
        /// </summary>
        /// <param name="task">The backup job task that is being logged.</param>
        /// <param name="action">The action (event) that is being performed (e.g., start, complete).</param>
        public void LogBackupAction(BackupJob_Models task,string time, string action)
        {
            logModel.LogAction(task,time, action); // Logs the action in the Log_Models.
            Console.ReadLine(); // Pauses the program to allow the user to read the debug output.
        }
        public void LogBackupErreur(string nom, String Base, String Erreur) 
        {
            logModel.LogErreur(nom, Base, Erreur); // Logs the action in the Log_Models.
            Console.ReadLine(); // Pauses the program to allow the user to read the debug output.
        }
    }
}
