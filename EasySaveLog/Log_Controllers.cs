using System;

namespace EasySaveLog
{
    /// <summary>
    /// Controller for logging actions related to backup tasks.
    /// Manages the interaction between the Log model and the view.
    /// </summary>
    /// 
    public class Log_Controller
    {
        private readonly Log_Models logModel; // Utilisation de l'instance Singleton

        /// <summary>
        /// Constructor initializes the Log_Models instance.
        /// </summary>
        public Log_Controller()
        {
            logModel = Log_Models.Instance; // On récupère l'instance unique
        }

        // <summary>
        /// Logs a backup action, including task details.
        /// </summary>
        /// <param name="name">The name of the backup task.</param>
        /// <param name="source">The source of the files being backed up.</param>
        /// <param name="target">The target location for the backup.</param>
        /// <param name="time">The time taken for the backup (in ms).</param>
        /// <param name="action">The action performed (e.g., "Started", "Completed").</param>
        public void LogBackupAction(string name, string source, string target, string time, string action)
        {
            logModel.LogAction(name, source, target, time, action);
            Console.ReadLine();
        }
        /// <summary>
        /// Logs an error related to a backup task.
        /// </summary>
        /// <param name="task">The backup task name.</param>
        /// <param name="baseAction">The base action that triggered the error.</param>
        /// <param name="error">The error message.</param>
        public void LogBackupErreur(string nom, String Base, String Erreur)
        {
            logModel.LogErreur(nom, Base, Erreur);
            Console.ReadLine();
        }
        /// <summary>
        /// Sets the type of log file format (either "json" or "xml").
        /// </summary>
        /// <param name="type">The log file format type.</param>
        public void Type_File_Log(string type)
        {
            logModel.TypeFile(type);
        }
        /// <summary>
        /// Gets the current log file type.
        /// </summary>
        /// <returns>The type of log file format currently in use.</returns>
        public string Get_Type_File()
        {
            return logModel.Type_File;
        }

    }
}