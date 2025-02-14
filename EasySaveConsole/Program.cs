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
            try
            {
                BackupJob_Controller controller = BackupJob_Controller.Instance;
                Console.Clear();

                SelectedLanguage = controller.DisplayLangue();
                LangManager.Instance.SetLanguage(SelectedLanguage);
                Console.WriteLine($"Langue choisie: {SelectedLanguage}");

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

    #endregion
}
