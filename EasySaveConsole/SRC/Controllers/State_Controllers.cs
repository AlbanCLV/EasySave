// Met � jour et enregistre les informations en temps r�el sur l'�tat des sauvegardes.
using System;
using EasySave.Models;  // Pour utiliser StateEntry ou autres mod�les si n�cessaire
using EasySave.Utilities;
using Newtonsoft.Json;

namespace EasySave.Controllers
{
    public class StateController
    {
        private const string StatePath = "States/state.json";

        public void UpdateState(BackupJobModel task, int remainingFiles, long remainingSize)
        {
            var stateEntry = new
            {
                TaskName = task.Name,
                Timestamp = DateTime.Now,
                Status = remainingFiles > 0 ? "Active" : "Completed",
                RemainingFiles = remainingFiles,
                RemainingSize = remainingSize
            };

            JsonHelper.SaveToJson(StatePath, stateEntry);
        }
    }
}
