using System;
using System.Collections.Generic;
using EasySaveConsole.Controllers;
using EasySaveConsole.Models;

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
            try
            {
                BackupJob_ViewModels controller1 = new BackupJob_ViewModels();
                Console.Clear();  // Clear the console for a fresh display

                SelectedLanguage = controller1.DisplayLangue();  // Store the selected language
                LangManager.Instance.SetLanguage(SelectedLanguage);  // Met à jour la langue
                BackupJob_ViewModels controller = new BackupJob_ViewModels();

                MenuInvoker invoker = new MenuInvoker();
                invoker.SetCommand("1", new CreateBackupTaskCommand(controller));
                invoker.SetCommand("2", new ExecuteSpecificTaskCommand(controller));
                invoker.SetCommand("3", new ExecuteAllTasksCommand(controller));
                invoker.SetCommand("4", new ViewTasksCommand(controller));
                invoker.SetCommand("5", new DeleteTaskCommand(controller));
                invoker.SetCommand("6", new ChoiceFileLogCommand(controller));
                invoker.SetCommand("7", new DecryptFolderCommand(controller));
                // L'option "8" permet de quitter

                controller.Choice_Type_File_Log();

                while (true)
                {
                    LangManager.Instance.SetLanguage(SelectedLanguage);
                    controller.DisplayMainMenu();

                    string input = Console.ReadLine()?.Trim();
                    if (input == "8")
                        break;

                    invoker.InvokeCommand(input);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Une erreur inattendue est survenue : " + ex.Message);
            }
        }
    }


    #region Commandes du Menu

    public interface ICommand
    {
        void Execute();
    }
    public class CreateBackupTaskCommand : ICommand
    {
        private readonly BackupJob_ViewModels _controller;

        public CreateBackupTaskCommand(BackupJob_ViewModels controller)
        {
            _controller = controller;
        }

        public void Execute() => _controller.CreateBackupTask();

    }
    public class ExecuteSpecificTaskCommand : ICommand
    {
        private readonly BackupJob_ViewModels _controller;

        public ExecuteSpecificTaskCommand(BackupJob_ViewModels controller)
        {
            _controller = controller;
        }
        public void Execute() => _controller.ExecuteSpecificTask();

    }
    public class ExecuteAllTasksCommand : ICommand
    {
        private readonly BackupJob_ViewModels _controller;

        public ExecuteAllTasksCommand(BackupJob_ViewModels controller)
        {
            _controller = controller;
        }
        public void Execute() => _controller.ExecuteAllTasks();

    }
    public class ViewTasksCommand : ICommand
    {
        private readonly BackupJob_ViewModels _controller;

        public ViewTasksCommand(BackupJob_ViewModels controller)
        {
            _controller = controller;
        }
        public void Execute() => _controller.ViewTasks();
    }
    public class ChoiceFileLogCommand : ICommand
    {
        private readonly BackupJob_ViewModels _controller;

        public ChoiceFileLogCommand(BackupJob_ViewModels controller)
        {
            _controller = controller;
        }
        public void Execute() => _controller.Choice_Type_File_Log();


    }
    public class DeleteTaskCommand : ICommand
    {
        private readonly BackupJob_ViewModels _controller;
        public DeleteTaskCommand(BackupJob_ViewModels controller)
        {
            _controller = controller;
        }
        public void Execute() => _controller.DeleteTask();

    }
    public class Choice_Langue : ICommand
    {
        private readonly BackupJob_ViewModels _controller;
        public Choice_Langue(BackupJob_ViewModels controller)
        {
            _controller = controller;
        }
        public void Execute()
        {
            string SelectedLanguage = _controller.DisplayLangue();  // Store the selected language
            LangManager.Instance.SetLanguage(SelectedLanguage);  // Met à jour la langue
        }
    }

    public class DecryptFolderCommand : ICommand
    {
        private readonly BackupJob_ViewModels _controller;
        public DecryptFolderCommand(BackupJob_ViewModels controller) { _controller = controller; }
        public void Execute() => _controller.DecryptFolder();
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


    #endregion











}