using System.Windows;
using EasySaveWPF.ViewModelsWPF;
using EasySaveConsole.Models;
namespace EasySaveWPF.Views
{
    public partial class PriorityExtensionsWindow : Window
    {
        private LangManager lang;
        private static PriorityExtensionsWindow _instance;
        public static string SelectedLanguage { get; private set; } = "en";
        private static readonly object _lock = new object();
        public static PriorityExtensionsWindow Instance
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
                            _instance = new PriorityExtensionsWindow();
                        }
                    }
                }
                return _instance;
            }
        }
        public PriorityExtensionsWindow()
        {
            InitializeComponent();
            DataContext = new PriorityExtensionsViewModel();
            lang = LangManager.Instance;
            setCollum();
        }
        public void setCollum()
        {
            Titre_Extan.Text = lang.Translate("Titre_Extan");
            bouttonadd.Content = lang.Translate("App");
            bouttonsupp.Content = lang.Translate("Supp");
        }
        public void setLanguage(string LANG)
        {
            lang.SetLanguage(LANG);
        }
    }
}
