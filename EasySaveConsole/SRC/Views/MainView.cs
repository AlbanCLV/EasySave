using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using EasySaveConsole.ViewModels;
using EasySaveConsole.Models;
using Terminal.Gui;
using EasySaveConsole.Views;

namespace EasySaveConsole.Views
{
    /// <summary>
    /// View that handles user interactions, displays the menu, and gathers input.
    /// </summary>
    public class MainView
    {
        private readonly LangManager lang;
        private static MainView _instance;
        private static readonly object _lock = new object();
        private Log_View LogView;
        private Lang_View LangView;
        private Backup_View BackupView;
        private Encryption_View EncryptionView;
        public MainView()
        {
            lang = LangManager.Instance;
            LogView = Log_View.Instance;
            BackupView = Backup_View.Instance;
            LangView = Lang_View.Instance;
            EncryptionView = Encryption_View.Instance;
        }
        public static MainView Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                            _instance = new MainView();
                    }
                }
                return _instance;
            }
        }


        /// <summary>
        /// LOG
        /// </summary>
        /// <returns></returns>
        public void Get_Type_Log(string a) =>  LogView.Get_Type_Log(a);
        public string GET_Type_File_Log() { return LogView.GET_Type_File_Log(); }



        /// <summary>
        /// Backup
        /// </summary>
        /// <returns></returns>
        public void DisplayMainMenu() => BackupView.DisplayMainMenu();
        public Backup_Models GetBackupDetails() { return BackupView.GetBackupDetails();  }

        

       
        /// <summary>
        /// Langue
        /// </summary>
        /// <returns></returns>
        public string DisplayLangue() { return LangView.DisplayLangue(); }
        public void DisplayMessage(string key) => LangView.DisplayMessage(key);


        /// <summary>
        /// Encryption
        /// </summary>
        /// <returns></returns>
        public (bool encryptEnabled, string password, bool encryptAll, string[] selectedExtensions) GetEncryptionSettings()
        {
            return EncryptionView.GetEncryptionSettings();
        }

      
        public (string, string) GetDecryptFolder()
        {
            return EncryptionView.GetDecryptFolder();
        }

    }
}
