using EasySave.Models;

namespace EasySave.Controllers
{
    /// <summary>
    /// Controller for logging actions related to backup tasks.
    /// Manages the interaction between the Log model and the view.
    /// This class is responsible for logging the states and actions during backup tasks.
    /// </summary>
    public class State_Controller
    {
        private State_models StateModels; // Instance of the State_models class to handle log operations related to task states.

        /// <summary>
        /// Constructor initializes the State_models instance.
        /// This constructor prepares the state model for logging backup task states.
        /// </summary>
        public State_Controller()
        {
            StateModels = new State_models(); // Initializes the state model to handle log actions related to task states.
        }

        /// <summary>
        /// Logs a backup action, including task details.
        /// This method updates the state of the backup task with the current status and destination.
        /// </summary>
        /// <param name="task">The backup job task that is being logged.</param>
        /// <param name="lasth">The source directory of the backup task.</param>
        /// <param name="desti">The destination directory of the backup task.</param>
        public void StateUpdate(BackupJob_Models task, string lasth, string desti)
        {
            StateModels.StateUpdate(task, lasth, desti);  // Log the current state of the backup task
        }

        /// <summary>
        /// Logs the end of a backup task.
        /// This method is called when the backup task is finished.
        /// </summary>
        /// <param name="task">The backup job task that is being logged.</param>
        /// <param name="lasth">The source directory of the completed backup task.</param>
        /// <param name="desti">The destination directory of the completed backup task.</param>
        public void StatEnd(BackupJob_Models task, string lasth, string desti)
        {
            StateModels.SatetEnd(task, lasth, desti);  // Log the end of the backup task
        }

        /// <summary>
        /// Logs an error related to a backup task.
        /// This method is called when an error occurs during the backup task.
        /// </summary>
        /// <param name="task">The backup job task that encountered an error.</param>
        /// <param name="lasth">The source directory of the backup task where the error occurred.</param>
        /// <param name="error">The error message describing what went wrong during the task.</param>
        /// <param name="desti">The destination directory where the backup task was supposed to complete.</param>
        public void StateError(BackupJob_Models task, string lasth, string error, string desti)
        {
            StateModels.StateError(task, lasth, error, desti);  // Log the error encountered during the backup task
        }
    }
}
