using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using EasySaveConsole.Models;
using EasySaveWPF.ModelsWPF;
using EasySaveWPF.ViewModelsWPF;

namespace EasySaveWPF.Views
{
    public partial class CryptageWPF : Window
    {
        private static CryptageWPF _instance;
        public static string SelectedLanguage { get; private set; } = "en";
        private static readonly object _lock = new object();
        private LangManager lang;
        private Cryptage_ModelsWPF EncryptionModelsWPF;

        public CryptageWPF()
        {
            InitializeComponent();
            lang = LangManager.Instance;
            EncryptionModelsWPF = Cryptage_ModelsWPF.Instance;
            lang.SetLanguage(SelectedLanguage);
            DataContext = new BusinessApps_ViewModel(); // Lier la fenêtre au ViewModel
            SetColumnHeaders();

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
        private void Boutton_Apply_Click(object sender, RoutedEventArgs e)
        {
            // Récupérer l'option sélectionnée dans le ComboBox
            string optionText = (OptionComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            string pass = PassTextBox.Password.ToString();
            bool EncryptionALL = false;
            var selectedExtensions = new List<string>();

            // Vérifier si l'option ou le mot de passe est vide
            if (string.IsNullOrWhiteSpace(optionText) || string.IsNullOrWhiteSpace(pass)) { return; }
            if (optionText == "Chiffrer toutes les sauvegardes" || optionText == "Encrypt all backups")
            {
                EncryptionALL = true;
            }
            else if (optionText == "Chiffrer uniquement les extensions sélectionnées" || optionText == "Encrypt only selected extensions")
            {
                if (ExtensionListBox.SelectedItems.Count == 0)
                {
                    System.Windows.MessageBox.Show(lang.Translate("ExtensionError"), lang.Translate("Error"), MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                foreach (ListBoxItem item in ExtensionListBox.SelectedItems)
                {
                    selectedExtensions.Add(item.Content.ToString());
                }
            }
            else if (optionText == "Ne pas chiffrer"|| optionText == "Do not encrypt")
            {
                EncryptionModelsWPF.SetEncryptionSettings("KO", false, selectedExtensions.ToArray(), false);
                CloseWindow(sender, e);
            }

            EncryptionModelsWPF.SetEncryptionSettings(pass, EncryptionALL, selectedExtensions.ToArray(), true);
            CloseWindow(sender, e);
        }


        public void SetLanguage(string langue)
        {
            SelectedLanguage = langue;
            lang.SetLanguage(langue);
        }
        public void SetColumnHeaders()
        {
            OptionTextBlock.Text = lang.Translate("OptionTextBlock");
            ExtansionTextBlock.Text = lang.Translate("ExtansionTextBlock");
            Option1.Content = lang.Translate("EncryptAllBackups");
            Option2.Content = lang.Translate("EncryptSelectedExtensions");
            Option3.Content = lang.Translate("NepasChiffrer");
            Boutton_Apply.Content = lang.Translate("Launch");
            PassTextBloc.Text = lang.Translate("MDP");
        }
        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }
    }
}