using System.Windows;
using EasySaveWPF.ViewModelsWPF;
using EasySaveConsole.Models;

namespace EasySaveWPF.Views
{
    /// <summary>
    /// Interaction logic for PriorityExtensionsWindow.
    /// This window allows users to manage priority file extensions.
    /// </summary>
    public partial class PriorityExtensionsWindow : Window
    {
        private LangManager lang;
        private static PriorityExtensionsWindow _instance;
        private static readonly object _lock = new object();

        /// <summary>
        /// Gets the selected language for the application.
        /// </summary>
        public static string SelectedLanguage { get; private set; } = "en";

        /// <summary>
        /// Gets the singleton instance of <see cref="PriorityExtensionsWindow"/>.
        /// Ensures that only one instance of the window is created (thread-safe).
        /// </summary>
        public static PriorityExtensionsWindow Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new PriorityExtensionsWindow();
                        }
                    }
                }
                return _instance;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PriorityExtensionsWindow"/> class.
        /// Sets up the DataContext and initializes the UI elements.
        /// </summary>
        public PriorityExtensionsWindow()
        {
            InitializeComponent();
            DataContext = new PriorityExtensionsViewModel();
            lang = LangManager.Instance;
            setCollum();
        }

        /// <summary>
        /// Updates UI labels with the selected language translations.
        /// </summary>
        public void setCollum()
        {
            Titre_Extan.Text = lang.Translate("Titre_Extan");
            bouttonadd.Content = lang.Translate("App");
            bouttonsupp.Content = lang.Translate("Supp");
        }

        /// <summary>
        /// Sets the application language.
        /// </summary>
        /// <param name="LANG">The language code to set.</param>
        public void setLanguage(string LANG)
        {
            lang.SetLanguage(LANG);
        }
    }
}
