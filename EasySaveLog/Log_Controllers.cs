using System;
using System.Diagnostics;  // Pour utiliser Stopwatch

namespace EasySaveLog
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
        public void LogBackupAction(string name, string source, string target, string time, string action)
        {
            logModel.LogAction(name, source, target, time, action); // Logs the action in the Log_Models.
            Console.ReadLine(); // Pauses the program to allow the user to read the debug output.
        }
        public void LogBackupErreur(string nom, String Base, String Erreur)
        {
            logModel.LogErreur(nom, Base, Erreur); // Logs the action in the Log_Models.
            Console.ReadLine(); // Pauses the program to allow the user to read the debug output.
        }
        public void Type_File_Log(string type)
        {
            logModel.TypeFile(type);
        }
        public string Get_Type_File()
        {
            return logModel.Type_File;
        }

    }
}