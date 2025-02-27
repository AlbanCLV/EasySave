using System.Windows;
using EasySaveConsole.Models;
using EasySaveWPF.ViewModelsWPF;

namespace EasySaveWPF.Views
{
    /// <summary>
    /// Interaction logic for BusinessAppsWindow.
    /// This window allows users to manage business application settings.
    /// </summary>
    public partial class BusinessAppsWindow : Window
    {
        private LangManager lang;
        private static BusinessAppsWindow _instance;
        private static readonly object _lock = new object();

        /// <summary>
        /// Gets the selected language for the application.
        /// </summary>
        public static string SelectedLanguage { get; private set; } = "en";

        /// <summary>
        /// Gets the singleton instance of <see cref="BusinessAppsWindow"/>.
        /// Ensures that only one instance of the window is created (thread-safe).
        /// </summary>
        public static BusinessAppsWindow Instance
        {
            get
            {
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

        /// <summary>
        /// Initializes a new instance of the <see cref="BusinessAppsWindow"/> class.
        /// Sets up the DataContext and initializes the UI elements.
        /// </summary>
        public BusinessAppsWindow()
        {
            lang = LangManager.Instance;
            lang.SetLanguage(SelectedLanguage);
            InitializeComponent();
            DataContext = new BusinessApps_ViewModel(SelectedLanguage, true);
            SetColumnHeaders();
        }

        /// <summary>
        /// Closes the window.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event data.</param>
        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

        /// <summary>
        /// Sets the application language.
        /// </summary>
        /// <param name="langue">The language code to set.</param>
        public void SetLangueLog(string langue)
        {
            SelectedLanguage = langue;
            lang.SetLanguage(langue);
        }

        /// <summary>
        /// Updates UI labels with the selected language translations.
        /// </summary>
        public void SetColumnHeaders()
        {
            TitreBusinessTexBLox.Text = lang.Translate("TitreBusinessTexBLox");
            Boutton_ADD.Content = lang.Translate("Create");
            Boutton_Supprimer.Content = lang.Translate("Delete");
        }
    }
}
