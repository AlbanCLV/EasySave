using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasySaveConsole.Models;
using Terminal.Gui;

namespace EasySaveConsole.Views
{
    internal class Encryption_View
    {
        private readonly LangManager lang;
        private Lang_View LangView;
        private static Encryption_View _instance;
        private static readonly object _lock = new object();
        public Encryption_View()
        {
            lang = LangManager.Instance;
            LangView = Lang_View.Instance;

        }
        public static Encryption_View Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                            _instance = new Encryption_View();
                    }
                }
                return _instance;
            }
        }
        public (bool encryptEnabled, string password, bool encryptAll, string[] selectedExtensions) GetEncryptionSettings()
        {
            Console.Clear();
            string response = "";
            while (true)
            {
                // "EncryptPrompt" should be defined in your translation file, e.g., "Do you want to encrypt backups? (Y/N)"
                Console.WriteLine(lang.Translate("EncryptPrompt"));
                response = Console.ReadLine()?.Trim().ToUpper();
                if (string.IsNullOrEmpty(response))
                {
                    Console.WriteLine(lang.Translate("EmptyInput"));
                    continue;
                }
                // Accept "Y" or "O" (for yes) and "N" for no.
                if (response == "Y" || response == "O" || response == "N")
                    break;
                else
                    Console.WriteLine(lang.Translate("InvalidInput"));
            }
            if (response == "N")
            {
                return (false, "", false, new string[0]);
            }
            // Since encryption is desired, present only two options: encrypt all or encrypt selected extensions.
            string option = "";
            while (true)
            {
                Console.WriteLine(lang.Translate("EncryptionOptionPrompt")); // e.g., "Choose encryption option:"
                Console.WriteLine("1. " + lang.Translate("EncryptAllBackups"));   // e.g., "Encrypt all backups"
                Console.WriteLine("2. " + lang.Translate("EncryptSelectedExtensions")); // e.g., "Encrypt only selected extensions"
                option = Console.ReadLine()?.Trim();
                if (string.IsNullOrEmpty(option))
                {
                    Console.WriteLine(lang.Translate("EmptyInput"));
                    continue;
                }
                if (option == "1" || option == "2")
                    break;
                else
                    Console.WriteLine(lang.Translate("InvalidInput"));
            }
            bool encryptAll = option == "1";
            string[] selectedExtensions = new string[0];
            if (option == "2")
            {
                Console.WriteLine(lang.Translate("EnterExtensions")); // e.g., "Enter extensions to encrypt, separated by commas:"
                string extensionsInput = Console.ReadLine();
                while (string.IsNullOrEmpty(extensionsInput))
                {
                    Console.WriteLine(lang.Translate("EmptyInput"));
                    extensionsInput = Console.ReadLine();
                }
                selectedExtensions = extensionsInput.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                                    .Select(e => e.Trim()).ToArray();
            }
            Console.WriteLine(lang.Translate("EnterEncryptionPassword")); // e.g., "Enter the encryption password (it will not be displayed):"
            string password = ReadPassword();
            while (string.IsNullOrEmpty(password))
            {
                Console.WriteLine(lang.Translate("EmptyInput"));
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
        public (string, string) GetDecryptFolder()
        {
            string password;
            string folderPath;

            while (true)
            {
                LangView.DisplayMessage("EnterDecryptionPassword");
                password = ReadPassword();

                if (string.IsNullOrEmpty(password))
                {
                    LangView.DisplayMessage("PasswordEmpty");
                    Console.ReadKey();
                    continue;  // Redemande le mot de passe
                }

                if (password.ToLower() == "exit")
                {
                    LangView.DisplayMessage("Exiting...");
                    return ("KO", "KO");
                }

                break;  // Mot de passe correct
            }

            while (true)
            {
                LangView.DisplayMessage("DecryptFolderPrompt");
                folderPath = BrowsePath(canChooseFiles: false, canChooseDirectories: true);

                if (string.IsNullOrEmpty(folderPath))
                {
                    LangView.DisplayMessage("NoFolderSelected");
                    Console.ReadKey();
                    continue;  // Redemande le dossier
                }

                if (folderPath.ToLower() == "exit")
                {
                    LangView.DisplayMessage("Exiting...");
                    return ("KO", "KO");
                }

                break;  // Dossier correct
            }

            return (password, folderPath);
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

    }
}
