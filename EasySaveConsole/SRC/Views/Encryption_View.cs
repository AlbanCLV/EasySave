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
    /// Represents the encryption view, handling encryption and decryption settings.
    /// </summary>
    internal class Encryption_View
    {
        private readonly LangManager lang;
        private Lang_View LangView;
        private static Encryption_View _instance;
        private static readonly object _lock = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="Encryption_View"/> class.
        /// </summary>
        public Encryption_View()
        {
            lang = LangManager.Instance;
            LangView = Lang_View.Instance;
        }

        /// <summary>
        /// Gets the singleton instance of <see cref="Encryption_View"/>.
        /// </summary>
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

        /// <summary>
        /// Prompts the user to configure encryption settings.
        /// </summary>
        /// <returns>A tuple containing encryption settings: whether encryption is enabled, the password, 
        /// whether all files should be encrypted, and the selected file extensions.</returns>
        public (bool encryptEnabled, string password, bool encryptAll, string[] selectedExtensions) GetEncryptionSettings()
        {
            Console.Clear();
            string response = "";
            while (true)
            {
                Console.WriteLine(lang.Translate("EncryptPrompt"));
                response = Console.ReadLine()?.Trim().ToUpper();
                if (string.IsNullOrEmpty(response))
                {
                    Console.WriteLine(lang.Translate("EmptyInput"));
                    continue;
                }
                if (response == "Y" || response == "O" || response == "N")
                    break;
                else
                    Console.WriteLine(lang.Translate("InvalidInput"));
            }
            if (response == "N")
            {
                return (false, "", false, new string[0]);
            }

            string option = "";
            while (true)
            {
                Console.WriteLine(lang.Translate("EncryptionOptionPrompt"));
                Console.WriteLine("1. " + lang.Translate("EncryptAllBackups"));
                Console.WriteLine("2. " + lang.Translate("EncryptSelectedExtensions"));
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
                Console.WriteLine(lang.Translate("EnterExtensions"));
                string extensionsInput = Console.ReadLine();
                while (string.IsNullOrEmpty(extensionsInput))
                {
                    Console.WriteLine(lang.Translate("EmptyInput"));
                    extensionsInput = Console.ReadLine();
                }
                selectedExtensions = extensionsInput.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                                    .Select(e => e.Trim()).ToArray();
            }

            Console.WriteLine(lang.Translate("EnterEncryptionPassword"));
            string password = ReadPassword();
            while (string.IsNullOrEmpty(password))
            {
                Console.WriteLine(lang.Translate("EmptyInput"));
                password = ReadPassword();
            }
            return (true, password, encryptAll, selectedExtensions);
        }

        /// <summary>
        /// Reads a password from the console input without displaying characters.
        /// </summary>
        /// <returns>The entered password as a string.</returns>
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

        /// <summary>
        /// Prompts the user for a password and a folder path to decrypt files.
        /// </summary>
        /// <returns>A tuple containing the decryption password and the folder path.</returns>
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
                    continue;
                }

                if (password.ToLower() == "exit")
                {
                    LangView.DisplayMessage("Exiting...");
                    return ("KO", "KO");
                }

                break;
            }

            while (true)
            {
                LangView.DisplayMessage("DecryptFolderPrompt");
                folderPath = BrowsePath(canChooseFiles: false, canChooseDirectories: true);

                if (string.IsNullOrEmpty(folderPath))
                {
                    LangView.DisplayMessage("NoFolderSelected");
                    Console.ReadKey();
                    continue;
                }

                if (folderPath.ToLower() == "exit")
                {
                    LangView.DisplayMessage("Exiting...");
                    return ("KO", "KO");
                }

                break;
            }

            return (password, folderPath);
        }

        /// <summary>
        /// Opens a file or directory browser to select a path.
        /// </summary>
        /// <param name="canChooseFiles">Indicates if files can be selected.</param>
        /// <param name="canChooseDirectories">Indicates if directories can be selected.</param>
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
            string result = dialog.FilePath.ToString();
            Application.Shutdown();
            return string.IsNullOrEmpty(result) ? null : result;
        }
    }
}
