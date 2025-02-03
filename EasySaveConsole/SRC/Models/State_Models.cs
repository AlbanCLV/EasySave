using System;
using System.IO;
using EasySave.Models;
using Newtonsoft.Json;

namespace EasySave
{
    /// <summary>
    /// Class for State models
    /// </summary>
    public class State_Models
    {
        /// <summary>
        /// Update the state of a backup task job
        /// </summary>
        /// <param name="task">BackupJob_Models task object</param>
        /// <param name="remainingFiles">integer indicator of remainingFiles to copy</param>
        /// <param name="remainingSize">long indicator of remaining size of files to copy</param>
        /// <param name="currentSource">string current source of folder to copy</param>
        /// <param name="currentTarget">string current target folder to copy</param>
        public void UpdateState(BackupJob_Models task, int remainingFiles, long remainingSize, string currentSource, string currentTarget)
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
