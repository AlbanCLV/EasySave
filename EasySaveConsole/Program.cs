using System;
using EasySave.Controllers;

namespace EasySave
{
    /// <summary>
    /// Main programm to call controller for interactions between Views and Models
    /// </summary>
    class Program
    {
        /// <summary>
        /// Call Controller
        /// </summary>
        /// <param name="args">CLI arguments</param>
        static void Main(string[] args)
        {
            // Controller will initiate Viexs and Models
            BackupJob_Controller controller = new BackupJob_Controller();
            controller.Run();  // Run main loop of application
        }
    }
}
