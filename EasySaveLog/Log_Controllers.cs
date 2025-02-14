using System;

namespace EasySave.Log
{
    /// <summary>
    /// Contrôleur de log qui enregistre les actions et erreurs de sauvegarde.
    /// </summary>
    public class Log_Controller
    {
        private readonly Log_Models logModel;

        public Log_Controller()
        {
            logModel = Log_Models.Instance ?? new Log_Models(); // On récupère l'instance unique
        }

        public void LogBackupAction(string name, string source, string target, string time, string action)
        {
            // Récupère le statut d'encryption depuis EncryptionUtility.
            logModel.LogAction(name, source, target, time, action);
        }

        public void LogBackupErreur(string name, string baseAction, string error)
        {
            logModel.LogErreurJSON(name, baseAction, error);
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
