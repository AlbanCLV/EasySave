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
        /// </summary>
        static void Main(string[] args)
        {
            // Initialize the controller and display the language selection screen
            BackupJob_Controller controller1 = new BackupJob_Controller();
            Console.Clear();

            SelectedLanguage = controller1.DisplayLangue();
            LangManager.Instance.SetLanguage(SelectedLanguage);
            BackupJob_Controller controller = new BackupJob_Controller();

            MenuInvoker invoker = new MenuInvoker();
            // Mapping the menu options to commands
            invoker.SetCommand("1", new CreateBackupTaskCommand(controller));
            invoker.SetCommand("2", new ExecuteSpecificTaskCommand(controller));
            invoker.SetCommand("3", new ExecuteAllTasksCommand(controller));
            invoker.SetCommand("4", new ViewTasksCommand(controller));
            invoker.SetCommand("5", new DeleteTaskCommand(controller));
            invoker.SetCommand("6", new ChoiceFileLogCommand(controller));
            invoker.SetCommand("7", new DecryptFolderCommand(controller));  // Option to decrypt a folder

            // Initialize file log choice
            controller.Choice_Type_File_Log();

            // Main menu loop
            while (true)
            {
                LangManager.Instance.SetLanguage(SelectedLanguage);
                controller.DisplayMainMenu();

                string input = Console.ReadLine()?.Trim();
                invoker.InvokeCommand(input);

                // Exit the application if the user selects option 8
                if (input == "8")
                {
                    Environment.Exit(0);
                }
            }
        }
    }

    /// <summary>
    /// ICommand interface defines a common method for all command classes.
    /// </summary>
    public interface ICommand
    {
        void Execute();
    }

    public class CreateBackupTaskCommand : ICommand
    {
        private readonly BackupJob_Controller _controller;
        public CreateBackupTaskCommand(BackupJob_Controller controller)
        {
            _controller = controller;
        }
        public void Execute()
        {
            _controller.CreateBackupTask();
        }
    }

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
    /// Command class to decrypt an entire folder.
    /// </summary>
    public class DecryptFolderCommand : ICommand
    {
        private readonly BackupJob_Controller _controller;
        public DecryptFolderCommand(BackupJob_Controller controller)
        {
            _controller = controller;
        }
        public void Execute()
        {
            _controller.DecryptFolder();
        }
    }

    /// <summary>
    /// MenuInvoker class is responsible for associating menu options with commands.
    /// </summary>
    public class MenuInvoker
    {
        private readonly Dictionary<string, ICommand> _commands = new Dictionary<string, ICommand>();
        public void SetCommand(string key, ICommand command)
        {
            _commands[key] = command;
        }
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
}
