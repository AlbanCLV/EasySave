using System;
using EasySave.Models;
using EasySave.Utilities;
using Terminal.Gui;
using EasySaveLog;

namespace EasySave.Views
{
    /// <summary>
    /// View class for managing user interactions with translations.
    /// </summary>
    public class BackupJob_View
    {
        private readonly LangManager lang;

        /// <summary>
        /// Initializes the view with a specified language.
        /// </summary>
        /// <param name="language">Language code (e.g., "en" or "fr").</param>
        public BackupJob_View()

        {
            lang = new LangManager(Program.SelectedLanguage);
        }

        /// <summary>
        /// Load and display the main menu in CLI.
        /// </summary>
        public void DisplayMainMenu()
        {
            Console.Clear();
            Console.WriteLine("\r\n /$$$$$$$$                                /$$$$$$                               \r\n| $$_____/                               /$$__  $$                              \r\n| $$        /$$$$$$   /$$$$$$$ /$$   /$$| $$  \\__/  /$$$$$$  /$$    /$$ /$$$$$$ \r\n| $$$$$    |____  $$ /$$_____/| $$  | $$|  $$$$$$  |____  $$|  $$  /$$//$$__  $$\r\n| $$__/     /$$$$$$$|  $$$$$$ | $$  | $$ \\____  $$  /$$$$$$$ \\  $$/$$/| $$$$$$$$\r\n| $$       /$$__  $$ \\____  $$| $$  | $$ /$$  \\ $$ /$$__  $$  \\  $$$/ | $$_____/\r\n| $$$$$$$$|  $$$$$$$ /$$$$$$$/|  $$$$$$$|  $$$$$$/|  $$$$$$$   \\  $/  |  $$$$$$$\r\n|________/ \\_______/|_______/  \\____  $$ \\______/  \\_______/    \\_/    \\_______/\r\n                               /$$  | $$                                        \r\n                              |  $$$$$$/                                        \r\n                               \\______/                                         \r\n");
            Console.WriteLine($"1. {lang.Translate("CreateBackupTask")}");
            Console.WriteLine($"2. {lang.Translate("ExecuteBackupTask")}");
            Console.WriteLine($"3. {lang.Translate("ExecuteAllTasks")}");
            Console.WriteLine($"4. {lang.Translate("ViewAllTasks")}");
            Console.WriteLine($"5. {lang.Translate("DeleteTask")}");
            Console.WriteLine($"6. {lang.Translate("Exit")}");
            Console.Write($"\n{lang.Translate("SelectOption")}");
        }

        /// <summary>
        /// Get backup details from the user.
        /// </summary>
        /// <returns>A new instance of <see cref="BackupJob_Models"/>.</returns>
        public BackupJob_Models GetBackupDetails()
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
            Console.Write("");

            Console.WriteLine($"\n{lang.Translate("SelectTargetDir")}");
            Console.ReadKey();
            string target = BrowsePath(canChooseFiles: false, canChooseDirectories: true);
            while (string.IsNullOrEmpty(target))
            {
                Console.WriteLine(lang.Translate("TargetDirEmpty"));
                target = BrowsePath(canChooseFiles: false, canChooseDirectories: true);
            }
            Console.WriteLine(target);
            Console.Write("");

            Console.Write(lang.Translate("EnterBackupType"));
            int typeInput;
            while (!int.TryParse(Console.ReadLine(), out typeInput) || (typeInput != 1 && typeInput != 2))
            {
                Console.Write(lang.Translate("InvalidBackupType"));
            }
            BackupType type = typeInput == 1 ? BackupType.Full : BackupType.Differential;

            return new BackupJob_Models(name, source, target, type);
        }

        /// <summary>
        /// Display a translated message.
        /// </summary>
        /// <param name="key">Key for the message.</param>
        public void DisplayMessage(string key)
        {
            Console.WriteLine(lang.Translate(key));
        }

        /// <summary>
        /// Browse and select a directory or file using a graphical interface.
        /// </summary>
        /// <param name="canChooseFiles">Whether files can be chosen.</param>
        /// <param name="canChooseDirectories">Whether directories can be chosen.</param>
        /// <returns>The selected path as a string.</returns>
        public string BrowsePath(bool canChooseFiles = false, bool canChooseDirectories = true)
        {
            Application.Init();

            var dialog = new OpenDialog(lang.Translate("SelectPathTitle"), lang.Translate("SelectPathDescription"))
            {
                CanChooseFiles = canChooseFiles,
                CanChooseDirectories = canChooseDirectories
            };

            Application.Run(dialog);

            if (!string.IsNullOrEmpty(dialog.FilePath.ToString()))
            {
                Application.Shutdown();
                return dialog.FilePath.ToString();
            }

            Application.Shutdown();
            return null; // No path selected
        }

        /// <summary>
        /// Displays a language selection menu and returns the selected language code.
        /// </summary>
       
        public string DisplayLangue()
        {
            // Set console output encoding to UTF-8 to properly handle special characters
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            // Display the language selection prompt to the user
            Console.WriteLine("Choose a language / Choisissez une langue:");
            Console.WriteLine("1. English");
            Console.WriteLine("2. Français");

            // Ask the user to enter their choice
            Console.Write("Enter your choice: ");

            // Return the selected language code ("fr" for French, "en" for English)
            return Console.ReadLine()?.Trim() == "2" ? "fr" : "en";
        }


    }

}