using System;
using System.IO;
using EasySave.Models;

namespace EasySave.Controllers
{
    /// <summary>
    /// Contrôleur d'état qui enregistre l'état d'exécution des sauvegardes.
    /// </summary>
    public class State_Controller
    {
        private const string StateFilePath = "state.log";
        private static State_Controller _instance;
        private static readonly object _lock = new object();

        private State_Controller() { }

        public static State_Controller Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                            _instance = new State_Controller();
                    }
                }
                return _instance;
            }
        }

        public void StateUpdate(BackupJob_Models task, string timeStamp, string targetDirectory)
        {
            string entry = $"{timeStamp} - Task: {task.Name} is running. Target: {targetDirectory}";
            AppendState(entry);
        }

        public void StatEnd(BackupJob_Models task, string timeStamp, string targetDirectory)
        {
            string entry = $"{timeStamp} - Task: {task.Name} completed. Target: {targetDirectory}";
            AppendState(entry);
        }

        public void StateError(BackupJob_Models task, string timeStamp, string error, string targetDirectory)
        {
            string entry = $"{timeStamp} - Task: {task.Name} encountered an error. Error: {error}";
            AppendState(entry);
        }

        private void AppendState(string entry)
        {
            try
            {
                File.AppendAllText(StateFilePath, entry + Environment.NewLine);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de l'écriture de l'état : {ex.Message}");
            }
        }
    }
}
