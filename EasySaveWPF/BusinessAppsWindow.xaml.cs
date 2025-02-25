using System.Windows;
using EasySaveConsole.Models;
using EasySaveWPF.ViewModelsWPF;

namespace EasySaveWPF.Views
{
    public partial class BusinessAppsWindow : Window
    {
        private LangManager lang;
        public static string SelectedLanguage { get; private set; } = "en";
        private static BusinessAppsWindow _instance;
        private static readonly object _lock = new object();
        public BusinessAppsWindow()
        {
            lang = LangManager.Instance;
            lang.SetLanguage(SelectedLanguage);
            InitializeComponent();
            DataContext = new BusinessApps_ViewModel(SelectedLanguage); // Lier la fenêtre au ViewModel
            SetColumnHeaders();
        }
        public static BusinessAppsWindow Instance
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
                            _instance = new BusinessAppsWindow();
                        }
                    }
                }
                return _instance;
            }
        }
        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }
        public void SetLangueLog(string langue)
        {
            SelectedLanguage = langue;
            lang.SetLanguage(langue);
        }
        public void SetColumnHeaders()
        {
            TitreBusinessTexBLox.Text = lang.Translate("TitreBusinessTexBLox");
            Boutton_ADD.Content = lang.Translate("Create");
            Boutton_Supprimer.Content = lang.Translate("Delete");
        }

    }
}