using System;
using System.Diagnostics;  // Pour utiliser Stopwatch

namespace EasySaveLog
{
    /// <summary>
    /// Controller for logging actions related to backup tasks.
    /// Manages the interaction between the Log model and the view.
    /// </summary>
    /// 
    public class Log_ViewModels
    {
        private readonly Log_Models logModel; // Utilisation de l'instance Singleton
        private static Log_ViewModels _instance;
        private static readonly object _lock = new object();

        /// <summary>
        /// Constructor initializes the Log_Models instance.
        /// </summary>
        public Log_ViewModels()
        {
            logModel = Log_Models.Instance; // On récupère l'instance unique
        }
        public static Log_ViewModels Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                            _instance = new Log_ViewModels();
                    }
                }
                return _instance;
            }
        }
        /// <summary>
        /// Logs a backup action, including task details.
        /// </summary>
        /// <param name="task">The backup job task that is being logged.</param>
        /// <param name="action">The action (event) that is being performed (e.g., start, complete).</param>
        public void LogBackupAction(string name, string source, string target, string time, string action)
        {
            logModel.LogAction(name, source, target, time, action);
        }

        public void LogBackupErreur(string nom, String Base, String Erreur)
        {
            logModel.LogErreur(nom, Base, Erreur);
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