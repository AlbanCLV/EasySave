using System;
using EasySave.Controllers;
using EasySave.Utilities;

namespace EasySave
{
    /// <summary>
    /// Point d'entrée de l'application EasySave.
    /// </summary>
    class Program
    {
        public static string SelectedLanguage { get; private set; }

        static void Main(string[] args)
        {
            // Initialize the controller and display the language selection screen
            BackupJob_Controller controller1 = new BackupJob_Controller();
            Console.Clear();  // Clear the console for a fresh display
            Console.WriteLine("Démarrage du ProcessWatcher");
            ProcessWatcher.StartWatching(); //Lancement du programme de surveillance

                SelectedLanguage = controller.DisplayLangue();
                LangManager.Instance.SetLanguage(SelectedLanguage);
                Console.WriteLine($"Langue choisie: {SelectedLanguage}");

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

                while (true)
                {
                    LangManager.Instance.SetLanguage(SelectedLanguage);
                    controller.DisplayMainMenu();

                    string input = Console.ReadLine()?.Trim();
                    if (input == "8")
                        break;

                // Exit the application if the user selects "8"
                if (input == "8")
                {
                    Environment.Exit(0);
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
        private readonly BackupJob_Controller _controller;
        public CreateBackupTaskCommand(BackupJob_Controller controller) { _controller = controller; }
        public void Execute() => _controller.CreateBackupTask();
    }

    public class ExecuteSpecificTaskCommand : ICommand
    {
        private readonly BackupJob_Controller _controller;
        public ExecuteSpecificTaskCommand(BackupJob_Controller controller) { _controller = controller; }
        public void Execute() => _controller.ExecuteSpecificTask();
    }

    public class ExecuteAllTasksCommand : ICommand
    {
        private readonly BackupJob_Controller _controller;
        public ExecuteAllTasksCommand(BackupJob_Controller controller) { _controller = controller; }
        public void Execute() => _controller.ExecuteAllTasks();
    }

    public class ViewTasksCommand : ICommand
    {
        private readonly BackupJob_Controller _controller;
        public ViewTasksCommand(BackupJob_Controller controller) { _controller = controller; }
        public void Execute() => _controller.ViewTasks();
    }

    public class DeleteTaskCommand : ICommand
    {
        private readonly BackupJob_Controller _controller;
        public DeleteTaskCommand(BackupJob_Controller controller) { _controller = controller; }
        public void Execute() => _controller.DeleteTask();
    }

    public class ChoiceFileLogCommand : ICommand
    {
        private readonly BackupJob_Controller _controller;
        public ChoiceFileLogCommand(BackupJob_Controller controller) { _controller = controller; }
        public void Execute() => _controller.Choice_Type_File_Log();
    }

    public class DecryptFolderCommand : ICommand
    {
        private readonly BackupJob_Controller _controller;
        public DecryptFolderCommand(BackupJob_Controller controller) { _controller = controller; }
        public void Execute() => _controller.DecryptFolder();
    }

    public class MenuInvoker
    {
        private readonly System.Collections.Generic.Dictionary<string, ICommand> _commands =
            new System.Collections.Generic.Dictionary<string, ICommand>();

        public void SetCommand(string key, ICommand command) => _commands[key] = command;

        public void InvokeCommand(string key)
        {
            if (_commands.ContainsKey(key))
                _commands[key].Execute();
            else
                Console.WriteLine("Commande invalide.");
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