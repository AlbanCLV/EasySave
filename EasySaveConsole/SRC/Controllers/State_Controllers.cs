using System;
using EasySave.Models;
using System.Diagnostics;  // Pour utiliser Stopwatch
using EasySave.Views;
using Newtonsoft.Json;
using System.IO;

namespace EasySave.Controllers
{
    /// <summary>
    /// Controller for logging actions related to backup tasks.
    /// Manages the interaction between the Log model and the view.
    /// </summary>
    public class State_Controller
    {
        private State_models StateModels; // Instance of the Log_Models class to handle log operations.

        /// <summary>
        /// Constructor initializes the Log_Models instance.
        /// </summary>
        public State_Controller()
        {
            StateModels = new State_models(); // Initializes the log model to handle log actions.
        }

        /// <summary>
        /// Logs a backup action, including task details.
        /// </summary>
        /// <param name="task">The backup job task that is being logged.</param>
        /// <param name="action">The action (event) that is being performed (e.g., start, complete).</param>
        public void StateUpdate(BackupJob_Models task, string lasth, string desti)
        {
            StateModels.StateUpdate(task, lasth, desti);
        }
        public void StatEnd(BackupJob_Models task, string lasth, string desti)
        {
            StateModels.SatetEnd(task, lasth, desti);
        }
        public void StateError(BackupJob_Models task, string lasth, string error, string desti)
        {
            StateModels.StateError(task, lasth, error, desti);
        }
    }
}
