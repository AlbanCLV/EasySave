using System;
using System.Collections.Generic;
using EasySaveConsole.ViewModels;
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
                BackupJob_ViewModels ViewModels1 = new BackupJob_ViewModels();
                Console.Clear();  // Clear the console for a fresh display
                Console.WriteLine("Démarrage du ProcessWatcher");
                
                SelectedLanguage = ViewModels1.DisplayLangue();  // Store the selected language
                LangManager.Instance.SetLanguage(SelectedLanguage);  // Met à jour la langue
                BackupJob_ViewModels ViewModels = new BackupJob_ViewModels();
                ViewModels.StartWatch();
                MenuInvoker invoker = new MenuInvoker();
                invoker.SetCommand("1", new CreateBackupTaskCommand(ViewModels));
                invoker.SetCommand("2", new ExecuteSpecificTaskCommand(ViewModels));
                invoker.SetCommand("3", new ExecuteAllTasksCommand(ViewModels));
                invoker.SetCommand("4", new ViewTasksCommand(ViewModels));
                invoker.SetCommand("5", new DeleteTaskCommand(ViewModels));
                invoker.SetCommand("6", new ChoiceFileLogCommand(ViewModels));
                invoker.SetCommand("7", new DecryptFolderCommand(ViewModels));
                invoker.SetCommand("8", new ManageBusinessApplicationCommand(ViewModels));

                // L'option "8" permet de quitter

                ViewModels.Choice_Type_File_Log();

                while (true)
                {
                    LangManager.Instance.SetLanguage(SelectedLanguage);
                    ViewModels.DisplayMainMenu();

                    string input = Console.ReadLine()?.Trim();
                    if (input == "9")
                    {
                        ViewModels.StopWatching ();
                    }

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
        private readonly BackupJob_ViewModels _ViewModels;

        public CreateBackupTaskCommand(BackupJob_ViewModels ViewModels)
        {
            _ViewModels = ViewModels;
        }

        public void Execute() => _ViewModels.CreateBackupTask();

    }
    public class ExecuteSpecificTaskCommand : ICommand
    {
        private readonly BackupJob_ViewModels _ViewModels;

        public ExecuteSpecificTaskCommand(BackupJob_ViewModels ViewModels)
        {
            _ViewModels = ViewModels;
        }
        public void Execute() => _ViewModels.ExecuteSpecificTask();

    }
    public class ExecuteAllTasksCommand : ICommand
    {
        private readonly BackupJob_ViewModels _ViewModels;

        public ExecuteAllTasksCommand(BackupJob_ViewModels ViewModels)
        {
            _ViewModels = ViewModels;
        }
        public void Execute() => _ViewModels.ExecuteAllTasks();

    }
    public class ViewTasksCommand : ICommand
    {
        private readonly BackupJob_ViewModels _ViewModels;

        public ViewTasksCommand(BackupJob_ViewModels ViewModels)
        {
            _ViewModels = ViewModels;
        }
        public void Execute() => _ViewModels.ViewTasks();
    }
    public class ChoiceFileLogCommand : ICommand
    {
        private readonly BackupJob_ViewModels _ViewModels;

        public ChoiceFileLogCommand(BackupJob_ViewModels ViewModels)
        {
            _ViewModels = ViewModels;
        }
        public void Execute() => _ViewModels.Choice_Type_File_Log();


    }
    public class DeleteTaskCommand : ICommand
    {
        private readonly BackupJob_ViewModels _ViewModels;
        public DeleteTaskCommand(BackupJob_ViewModels ViewModels)
        {
            _ViewModels = ViewModels;
        }
        public void Execute() => _ViewModels.DeleteTask();

    }
    public class Choice_Langue : ICommand
    {
        private readonly BackupJob_ViewModels _ViewModels;
        public Choice_Langue(BackupJob_ViewModels ViewModels)  {   _ViewModels = ViewModels; }
        public void Execute()
        {
            string SelectedLanguage = _ViewModels.DisplayLangue();  // Store the selected language
            LangManager.Instance.SetLanguage(SelectedLanguage);  // Met à jour la langue
        }
    }

    public class DecryptFolderCommand : ICommand
    {
        private readonly BackupJob_ViewModels _ViewModels;
        public DecryptFolderCommand(BackupJob_ViewModels ViewModels) { _ViewModels = ViewModels; }
        public void Execute() => _ViewModels.DecryptFolder();
    }

    public class ManageBusinessApplicationCommand : ICommand
    {
        private readonly BackupJob_ViewModels _ViewModels;

        public ManageBusinessApplicationCommand(BackupJob_ViewModels ViewModels)
        {
            _ViewModels = ViewModels;
        }

        public void Execute() => _ViewModels.ManageBusinessApplication();
    }

    public class AddBusinessApplicationCommand : ICommand
    {
        private readonly BackupJob_ViewModels _ViewModels;
        public AddBusinessApplicationCommand(BackupJob_ViewModels ViewModels) { _ViewModels = ViewModels; }
        public void Execute() => _ViewModels.AddBusinessApplication();
    }

    public class RemoveBusinessApplicationCommand : ICommand
    {
        private readonly BackupJob_ViewModels _ViewModels;

        public RemoveBusinessApplicationCommand(BackupJob_ViewModels ViewModels)
        {
            _ViewModels = ViewModels;
        }

        public void Execute() => _ViewModels.RemoveBusinessApplication();
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