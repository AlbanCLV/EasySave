using System;
using System.Collections.Generic;
using EasySave.Controllers;
using EasySave.Utilities;

namespace EasySave
{
    /// <summary>
    /// Entry point for the EasySave application.
    /// </summary>
    class Program
    {
        // Property to store the selected language for the application
        public static string SelectedLanguage { get; private set; }

        /// <summary>
        /// Main method that serves as the entry point of the application.
        /// Initializes the application and handles the main menu and user interactions.
        /// </summary>
        static void Main(string[] args)
        {
            // Initialize the controller and display the language selection screen
            BackupJob_Controller controller1 = new BackupJob_Controller();
            Console.Clear();  // Clear the console for a fresh display
            Console.WriteLine("Démarrage du ProcessWatcher");
            ProcessWatcher.StartWatching(); //Lancement du programme de surveillance

            SelectedLanguage = controller1.DisplayLangue();  // Store the selected language
            LangManager.Instance.SetLanguage(SelectedLanguage);  // Update the language
            BackupJob_Controller controller = new BackupJob_Controller();

            MenuInvoker invoker = new MenuInvoker();
            // Add commands to the invoker (linking menu options to actions)
            invoker.SetCommand("1", new CreateBackupTaskCommand(controller));
            invoker.SetCommand("2", new ExecuteSpecificTaskCommand(controller));
            invoker.SetCommand("3", new ExecuteAllTasksCommand(controller));
            invoker.SetCommand("4", new ViewTasksCommand(controller));
            invoker.SetCommand("5", new DeleteTaskCommand(controller));
            invoker.SetCommand("6", new ChoiceFileLogCommand(controller));
            invoker.SetCommand("7", new AddBusinessApplicationCommand(controller));


            // Initialize file log choice
            controller.Choice_Type_File_Log();

            // Infinite loop to keep displaying the menu until the user decides to exit
            while (true)
            {
                // Display the main menu
                LangManager.Instance.SetLanguage(SelectedLanguage);  // Update the language

                controller.DisplayMainMenu();

                // Get user input and trim any extra spaces
                string input = Console.ReadLine()?.Trim();
                // Invoke the command based on user input
                invoker.InvokeCommand(input);

                // Exit the application if the user selects "8"
                if (input == "8")
                {
                    Environment.Exit(0);
                }
            }
        }
    }

    /// <summary>
    /// ICommand interface defines a common method for all command classes to execute actions.
    /// </summary>
    public interface ICommand
    {
        void Execute();
    }
    /// <summary>
    /// Command class to create a new backup task.
    /// </summary>
    public class CreateBackupTaskCommand : ICommand
    {
        private readonly BackupJob_Controller _controller;

        public CreateBackupTaskCommand(BackupJob_Controller controller)
        {
            // Constructor to initialize the controller for this command
            _controller = controller;
        }

        public void Execute()
        {
            // Executes the creation of a backup task
            _controller.CreateBackupTask();
        }
    }
    /// <summary>
    /// Command class to execute a specific backup task.
    /// </summary>
    public class ExecuteSpecificTaskCommand : ICommand
    {
        private readonly BackupJob_Controller _controller;

        public ExecuteSpecificTaskCommand(BackupJob_Controller controller)
        {
            _controller = controller;
        }

        public void Execute()
        {
            _controller.ExecuteSpecificTask();
        }
    }
    /// <summary>
    /// Command class to execute all backup task.
    /// </summary>
    public class ExecuteAllTasksCommand : ICommand
    {
        private readonly BackupJob_Controller _controller;

        public ExecuteAllTasksCommand(BackupJob_Controller controller)
        {
            _controller = controller;
        }

        public void Execute()
        {
            _controller.ExecuteAllTasks();
        }
    }
    /// <summary>
    /// Command class to view all backup tasks.
    /// </summary>
    public class ViewTasksCommand : ICommand
    {
        private readonly BackupJob_Controller _controller;

        public ViewTasksCommand(BackupJob_Controller controller)
        {
            _controller = controller;
        }

        public void Execute()
        {
            _controller.ViewTasks();
        }
    }
    /// <summary>
    /// Command class to choose the file log type.
    /// </summary>
    public class ChoiceFileLog : ICommand
    {
        private readonly BackupJob_Controller _controller;

        public ChoiceFileLog(BackupJob_Controller controller)
        {
            _controller = controller;
        }

        public void Execute()
        {
            _controller.Choice_Type_File_Log();
        }
    }
    /// <summary>
    /// Command class to delete a backup task.
    /// </summary>
    public class DeleteTaskCommand : ICommand
    {
        private readonly BackupJob_Controller _controller;
        public DeleteTaskCommand(BackupJob_Controller controller)
        {
            _controller = controller;
        }
        public void Execute()
        {
            _controller.DeleteTask();
        }
    }
    /// <summary>
    /// Command class to change the application language.
    /// </summary>
    public class ChoiceFileLogCommand : ICommand
    {
        private readonly BackupJob_Controller _controller;
        public ChoiceFileLogCommand(BackupJob_Controller controller)
        {
            _controller = controller;
        }
        public void Execute()
        {
            _controller.Choice_Type_File_Log();
        }
    }
    /// <summary>
    /// MenuInvoker class is responsible for associating menu options with commands.
    /// </summary>
    public class MenuInvoker
    {
        private readonly Dictionary<string, ICommand> _commands = new Dictionary<string, ICommand>();

        /// <summary>
        /// Associates a command with a menu option.
        /// </summary>
        /// <param name="key">The menu option key.</param>
        /// <param name="command">The command to be executed.</param>
        public void SetCommand(string key, ICommand command)
        {
            _commands[key] = command;
        }

        /// <summary>
        /// Executes the command associated with the given menu option key.
        /// </summary>
        /// <param name="key">The menu option key.</param>
        public void InvokeCommand(string key)
        {
            if (_commands.ContainsKey(key))
            {
                _commands[key].Execute();
            }
            else
            {
                Console.WriteLine("Commande invalide.");
            }
        }
    }

    /// <summary>
    /// Command class to add a business application.
    /// </summary>
    public class AddBusinessApplicationCommand : ICommand
    {
        private readonly BackupJob_Controller _controller;

        public AddBusinessApplicationCommand(BackupJob_Controller controller)
        {
            _controller = controller;
        }

        public void Execute()
        {
            _controller.AddBusinessApplication();
        }
    }












}