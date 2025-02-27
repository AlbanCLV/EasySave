using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasySaveConsole.Models;
using Terminal.Gui;

namespace EasySaveConsole.Views
{
    /// <summary>
    /// Classe représentant l'affichage et l'interaction avec l'utilisateur pour la gestion des sauvegardes.
    /// </summary>
    internal class Backup_View
    {
        /// <summary>
        /// Instance du gestionnaire de langues pour la traduction des messages.
        /// </summary>
        private readonly LangManager lang;

        /// <summary>
        /// Instance unique de la classe Backup_View (Singleton).
        /// </summary>
        private static Backup_View _instance;

        /// <summary>
        /// Objet utilisé pour verrouiller l'instance (thread-safe singleton).
        /// </summary>
        private static readonly object _lock = new object();

        /// <summary>
        /// Constructeur privé de la classe Backup_View.
        /// Initialise le gestionnaire de langues.
        /// </summary>
        private Backup_View()
        {
            lang = LangManager.Instance;
        }

        /// <summary>
        /// Obtient l'instance unique de Backup_View (Singleton).
        /// </summary>
        public static Backup_View Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                            _instance = new Backup_View();
                    }
                }
                return _instance;
            }
        }

        /// <summary>
        /// Affiche le menu principal de l'application.
        /// </summary>
        public void DisplayMainMenu()
        {
            Console.Clear();
            // ASCII art header
            Console.WriteLine("\r\n /$$$$$$$$                                /$$$$$$                               ");
            Console.WriteLine("| $$_____/                               /$$__  $$                              ");
            Console.WriteLine("| $$        /$$$$$$   /$$$$$$$ /$$   /$$| $$  \\__/  /$$$$$$  /$$    /$$ /$$$$$$ ");
            Console.WriteLine("| $$$$$    |____  $$ /$$_____/| $$  | $$|  $$$$$$  |____  $$|  $$  /$$//$$__  $$");
            Console.WriteLine("| $$__/     /$$$$$$$|  $$$$$$ | $$  | $$ \\____  $$  /$$$$$$$ \\  $$/$$/| $$$$$$$$");
            Console.WriteLine("| $$       /$$__  $$ \\____  $$| $$  | $$ /$$  \\ $$ /$$__  $$  \\  $$$/ | $$_____/");
            Console.WriteLine("| $$$$$$$$|  $$$$$$$ /$$$$$$$/|  $$$$$$$|  $$$$$$/|  $$$$$$$   \\  $/  |  $$$$$$$");
            Console.WriteLine("|________/ \\_______/|_______/  \\____  $$ \\______/  \\_______/    \\_/    \\_______/");
            Console.WriteLine($"1. {lang.Translate("CreateBackupTask")}");
            Console.WriteLine($"2. {lang.Translate("ExecuteBackupTask")}");
            Console.WriteLine($"3. {lang.Translate("ExecuteAllTasks")}");
            Console.WriteLine($"4. {lang.Translate("ViewAllTasks")}");
            Console.WriteLine($"5. {lang.Translate("DeleteTask")}");
            Console.WriteLine($"6. {lang.Translate("Choice_log_display")}");
            Console.WriteLine($"7. {lang.Translate("DecryptFile")}");
            Console.WriteLine($"8. {lang.Translate("add_buisness_app")}");
            Console.WriteLine($"9. {lang.Translate("Exit")}");
            Console.Write($"\n{lang.Translate("SelectOption")}");
        }

        /// <summary>
        /// Demande à l'utilisateur les détails d'une nouvelle tâche de sauvegarde.
        /// </summary>
        /// <returns>Un tuple contenant un objet Backup_Models et la confirmation de l'utilisateur.</returns>
        public (Backup_Models, string) GetBackupDetails()
        {
            Console.Clear();
            Console.WriteLine($"=== {lang.Translate("CreateBackupTask")} ===");

            Console.Write(lang.Translate("EnterTaskName"));
            string name = Console.ReadLine();

            while (string.IsNullOrEmpty(name))
            {
                Console.WriteLine(lang.Translate("TaskNameEmpty"));
                Console.Write(lang.Translate("EnterTaskName"));
                name = Console.ReadLine();
            }

            Console.WriteLine($"\n{lang.Translate("SelectSourceDir")}");
            Console.ReadKey();
            string source = BrowsePath(canChooseFiles: false, canChooseDirectories: true);
            while (string.IsNullOrEmpty(source))
            {
                Console.WriteLine(lang.Translate("SourceDirEmpty"));
                source = BrowsePath(canChooseFiles: false, canChooseDirectories: true);
            }
            Console.WriteLine(source);

            Console.WriteLine($"\n{lang.Translate("SelectTargetDir")}");
            Console.ReadKey();
            string target = BrowsePath(canChooseFiles: false, canChooseDirectories: true);
            while (string.IsNullOrEmpty(target))
            {
                Console.WriteLine(lang.Translate("TargetDirEmpty"));
                target = BrowsePath(canChooseFiles: false, canChooseDirectories: true);
            }
            Console.WriteLine(target);

            Console.Write(lang.Translate("EnterBackupType"));
            int typeInput;
            while (!int.TryParse(Console.ReadLine(), out typeInput) || (typeInput != 1 && typeInput != 2))
            {
                Console.WriteLine(lang.Translate("InvalidBackupType"));
            }
            string type = typeInput == 1 ? "Full" : "Differential";

            // Résumé et confirmation
            Console.Clear();
            Console.WriteLine(lang.Translate("task_summary"));
            Console.WriteLine($"{lang.Translate("task_name")}: {name}");
            Console.WriteLine($"{lang.Translate("source_directory")}: {source}");
            Console.WriteLine($"{lang.Translate("target_directory")}: {target}");
            Console.WriteLine($"{lang.Translate("backup_type")}: {(type == "Full" ? lang.Translate("full_backup") : lang.Translate("differential_backup"))}");
            Console.WriteLine($"\n{lang.Translate("confirm_task_creation")}");

            string confirmation = Console.ReadLine()?.ToUpper();

            return (new Backup_Models(name, source, target, type), confirmation);
        }

        /// <summary>
        /// Ouvre une boîte de dialogue pour sélectionner un chemin (dossier ou fichier).
        /// </summary>
        /// <param name="canChooseFiles">Indique si l'utilisateur peut sélectionner des fichiers.</param>
        /// <param name="canChooseDirectories">Indique si l'utilisateur peut sélectionner des dossiers.</param>
        /// <returns>Le chemin sélectionné sous forme de chaîne.</returns>
        public string BrowsePath(bool canChooseFiles = false, bool canChooseDirectories = true)
        {
            Application.Init();
            var dialog = new OpenDialog(lang.Translate("SelectPathTitle"), lang.Translate("SelectPathDescription"))
            {
                CanChooseFiles = canChooseFiles,
                CanChooseDirectories = canChooseDirectories
            };
            Application.Run(dialog);
            string result = dialog.FilePath.ToString();
            Application.Shutdown();
            return string.IsNullOrEmpty(result) ? null : result;
        }

        /// <summary>
        /// Demande à l'utilisateur de saisir le numéro de la tâche de sauvegarde à supprimer.
        /// </summary>
        /// <returns>Le numéro de la tâche ou -1 en cas d'erreur.</returns>
        public int GetDeleteTask()
        {
            while (true)
            {
                Console.Write(lang.Translate("enter_task_number_to_delete"));
                if (int.TryParse(Console.ReadLine(), out int taskNumber) && taskNumber > 0)
                {
                    return taskNumber;
                }
                else
                {
                    return -1;
                }
            }
        }

        /// <summary>
        /// Demande à l'utilisateur de saisir le numéro de la tâche de sauvegarde à exécuter.
        /// </summary>
        /// <returns>Le numéro de la tâche ou -1 en cas d'erreur.</returns>
        public int GetExecuteTasks()
        {
            Console.Write(lang.Translate("enter_task_number_to_execute"));
            if (int.TryParse(Console.ReadLine(), out int taskNumber) && taskNumber > 0)
            {
                return taskNumber;
            }
            else
            {
                return -1;
            }
        }
    }
}
