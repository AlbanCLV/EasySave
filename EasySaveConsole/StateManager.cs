using System;
using System.IO;
using Newtonsoft.Json;

namespace EasySave
{
    public class StateManager
    {
        public void UpdateState(BackupTask task, int remainingFiles, long remainingSize, string currentSource, string currentTarget)
        {
            var stateEntry = new
            {
                TaskName = task.Name,
                Timestamp = DateTime.Now,
                Status = remainingFiles > 0 ? "Active" : "Completed",
                RemainingFiles = remainingFiles,
                RemainingSize = remainingSize,
                CurrentSource = currentSource,
                CurrentTarget = currentTarget
            };

            string statePath = Path.Combine("States", "state.json");
            Directory.CreateDirectory("States");
            File.WriteAllText(statePath, JsonConvert.SerializeObject(stateEntry, Formatting.Indented));
        }
    }
}
