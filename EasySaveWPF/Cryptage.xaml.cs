using System.Windows;
using EasySaveConsole.Models;
using EasySaveWPF.ViewModelsWPF;

namespace EasySaveWPF.Views
{
    public partial class CryptageWPF : Window
    {
        private static CryptageWPF _instance;
        public static string SelectedLanguage { get; private set; } = "en";
        private static readonly object _lock = new object();
        private LangManager lang;

        public CryptageWPF()
        {
            InitializeComponent();
            lang = LangManager.Instance;
            lang.SetLanguage(SelectedLanguage);
            DataContext = new BusinessApps_ViewModel(); // Lier la fenêtre au ViewModel
        }
        public static CryptageWPF Instance
        {
            get
            {
                // Use double-check locking to ensure thread-safety
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new CryptageWPF();
                        }
                    }
                }
                return _instance;
            }
        }
        private void FichierLogExecute(object sender, RoutedEventArgs e)
        {

        }

    }
}