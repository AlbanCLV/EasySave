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

            SelectedLanguage = controller1.DisplayLangue();  // Store the selected language
            LangManager.Instance.SetLanguage(SelectedLanguage);  // Met à jour la langue
            BackupJob_Controller controller = new BackupJob_Controller();

            MenuInvoker invoker = new MenuInvoker();
            // Ajouter les commandes à l'invoker
            invoker.SetCommand("1", new CreateBackupTaskCommand(controller));
            invoker.SetCommand("2", new ExecuteSpecificTaskCommand(controller));
            invoker.SetCommand("3", new ExecuteAllTasksCommand(controller));
            invoker.SetCommand("4", new ViewTasksCommand(controller));
            invoker.SetCommand("5", new DeleteTaskCommand(controller));
            invoker.SetCommand("6", new ChoiceFileLog(controller));


            controller.Choice_Type_File_Log();

            // Infinite loop to keep displaying the menu until the user decides to exit
            while (true)
            {
                // Display the main menu
                LangManager.Instance.SetLanguage(SelectedLanguage);  // Met à jour la langue

                controller.DisplayMainMenu();

                // Get user input and trim any extra spaces
                string input = Console.ReadLine()?.Trim();

                invoker.InvokeCommand(input);

                if (input == "7")
                {
                    Environment.Exit(0);
                }
            }
        }
    }

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
    public class Choice_Langue : ICommand
    {
        private readonly BackupJob_Controller _controller;
        public Choice_Langue(BackupJob_Controller controller)
        {
            _controller = controller;
        }
        public void Execute()
        {
            string SelectedLanguage = _controller.DisplayLangue();  // Store the selected language
            LangManager.Instance.SetLanguage(SelectedLanguage);  // Met à jour la langue
        }
    }
    public class MenuInvoker
    {
        private readonly Dictionary<string, ICommand> _commands = new Dictionary<string, ICommand>();

        // Associe une commande à un choix du menu
        public void SetCommand(string key, ICommand command)
        {
            _commands[key] = command;
        }

        // Exécute la commande associée à la clé donnée
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