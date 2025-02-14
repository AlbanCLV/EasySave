using System;
using System.Linq;
using System.Text;
using EasySave.Models;
using EasySave.Utilities;
using Terminal.Gui;

namespace EasySave.Views
{
    /// <summary>
    /// Vue qui gère les interactions utilisateur, l'affichage du menu et la saisie de données.
    /// </summary>
    public class BackupJob_View
    {
        private readonly LangManager lang;

        public BackupJob_View()
        {
            lang = LangManager.Instance;
        }

        public void DisplayMainMenu()
        {
            Console.Clear();
            Console.WriteLine("\r\n /$$$$$$$$                                /$$$$$$                               ");
            Console.WriteLine("| $$_____/                               /$$__  $$                              ");
            Console.WriteLine("| $$        /$$$$$$   /$$$$$$$ /$$   /$$| $$  \\__/  /$$$$$$  /$$    /$$ /$$$$$$ ");
            Console.WriteLine("| $$$$$    |____  $$ /$$_____/| $$  | $$|  $$$$$$  |____  $$|  $$  /$$//$$__  $$");
            Console.WriteLine("| $$__/     /$$$$$$$|  $$$$$$ | $$  | $$ \\____  $$  /$$$$$$$ \\  $$/$$/| $$$$$$$$");
            Console.WriteLine("| $$       /$$__  $$ \\____  $$| $$  | $$ /$$  \\ $$ /$$__  $$  \\  $$$/ | $$_____/");
            Console.WriteLine("| $$$$$$$$|  $$$$$$$ /$$$$$$$/|  $$$$$$$|  $$$$$$/|  $$$$$$$   \\  $/  |  $$$$$$$");
            Console.WriteLine("|________/ \\_______/|_______/  \\____  $$ \\______/  \\_______/    \\_/    \\_______/");
            Console.WriteLine("                               /$$  | $$                                        ");
            Console.WriteLine("                              |  $$$$$$/                                        ");
            Console.WriteLine("                               \\______/                                         ");
            Console.WriteLine($"1. {lang.Translate("CreateBackupTask")}");
            Console.WriteLine($"2. {lang.Translate("ExecuteBackupTask")}");
            Console.WriteLine($"3. {lang.Translate("ExecuteAllTasks")}");
            Console.WriteLine($"4. {lang.Translate("ViewAllTasks")}");
            Console.WriteLine($"5. {lang.Translate("DeleteTask")}");
            Console.WriteLine($"6. {lang.Translate("Choice_log_display")}");
            Console.WriteLine($"7. {lang.Translate("DecryptFile")}");
            Console.WriteLine($"8. {lang.Translate("Exit")}");
            Console.Write($"\n{lang.Translate("SelectOption")}");
        }

        public void Get_Type_Log(string a)
        {
            Console.Clear();
            Console.WriteLine(lang.Translate("Type_Now") + $"{a}");
        }

        public string Type_File_Log()
        {
            Console.Write(lang.Translate("Choix_log"));
            string format = Console.ReadLine();

            while (format != "xml" && format != "json" && format != "exit")
            {
                Console.WriteLine(lang.Translate("No_Change_Type_Log"));
                Console.Write(lang.Translate("Choix_log"));
                format = Console.ReadLine();
            }
            if (format == "json" || format == "xml")
            {
                Console.WriteLine(lang.Translate("GUI_Log"));
                Console.WriteLine(format);
                return format;
            }
            else if (format == "exit")
            {
                Console.WriteLine(lang.Translate("No_Change_Log"));
                return format;
            }
            return format;
        }

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
                Console.Write(lang.Translate("InvalidBackupType"));
            }
            BackupType type = typeInput == 1 ? BackupType.Full : BackupType.Differential;

            return new BackupJob_Models(name, source, target, type);
        }

        public void DisplayMessage(string key)
        {
            Console.WriteLine(lang.Translate(key));
        }

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

        public string DisplayLangue()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("Choose a language / Choisissez une langue:");
            Console.WriteLine("1. English");
            Console.WriteLine("2. Français");
            Console.Write("Enter your choice: ");
            return Console.ReadLine()?.Trim() == "2" ? "fr" : "en";
        }

        public (bool encryptEnabled, string password, bool encryptAll, string[] selectedExtensions) GetEncryptionSettings()
        {
            Console.Clear();
            string response = "";
            while (true)
            {
                Console.WriteLine("Voulez-vous crypter les sauvegardes ? (O/N)");
                response = Console.ReadLine()?.Trim().ToUpper();
                if (string.IsNullOrEmpty(response))
                {
                    Console.WriteLine("Entrée vide. Veuillez répondre par O ou N.");
                    continue;
                }
                if (response == "O" || response == "N")
                    break;
                else
                    Console.WriteLine("Réponse invalide. Veuillez répondre par O ou N.");
            }
            if (response == "N")
            {
                return (false, "", false, new string[0]);
            }
            string option = "";
            while (true)
            {
                Console.WriteLine("Choisissez l'option de chiffrement :");
                Console.WriteLine("1. Chiffrer toutes les sauvegardes");
                Console.WriteLine("3. Chiffrer seulement certaines extensions");
                option = Console.ReadLine()?.Trim();
                if (string.IsNullOrEmpty(option))
                {
                    Console.WriteLine("Entrée vide. Veuillez entrer 1, 2 ou 3.");
                    continue;
                }
                if (option == "1" || option == "2" || option == "3")
                    break;
                else
                    Console.WriteLine("Entrée invalide. Veuillez entrer 1, 2 ou 3.");
            }
            if (option == "2")
            {
                return (false, "", false, new string[0]);
            }
            bool encryptAll = option == "1";
            string[] selectedExtensions = new string[0];
            if (option == "3")
            {
                Console.WriteLine("Entrez les extensions à crypter, séparées par des virgules (ex : .txt, .docx) :");
                string extensionsInput = Console.ReadLine();
                while (string.IsNullOrEmpty(extensionsInput))
                {
                    Console.WriteLine("Aucune extension fournie. Veuillez entrer au moins une extension.");
                    extensionsInput = Console.ReadLine();
                }
                selectedExtensions = extensionsInput.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                                    .Select(e => e.Trim()).ToArray();
            }
            Console.WriteLine("Entrez le mot de passe pour le chiffrement (il ne sera pas affiché) :");
            string password = ReadPassword();
            while (string.IsNullOrEmpty(password))
            {
                Console.WriteLine("Le mot de passe ne peut pas être vide. Veuillez réessayer :");
                password = ReadPassword();
            }
            return (true, password, encryptAll, selectedExtensions);
        }

        public string ReadPassword()
        {
            StringBuilder sb = new StringBuilder();
            ConsoleKeyInfo key;
            while ((key = Console.ReadKey(true)).Key != ConsoleKey.Enter)
            {
                if (key.Key == ConsoleKey.Backspace && sb.Length > 0)
                    sb.Remove(sb.Length - 1, 1);
                else if (!char.IsControl(key.KeyChar))
                    sb.Append(key.KeyChar);
            }
            Console.WriteLine();
            return sb.ToString();
        }
    }
}
