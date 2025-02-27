using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using EasySaveConsole.Models;
using EasySaveWPF.ModelsWPF;
using EasySaveWPF.ViewModelsWPF;
using System.Windows.Forms;
using System.IO;

namespace EasySaveWPF.Views
{
    /// <summary>
    /// Interaction logic for CryptageWPF.
    /// This window allows users to configure encryption settings.
    /// </summary>
    public partial class CryptageWPF : Window
    {
        private static CryptageWPF _instance;
        private static readonly object _lock = new object();
        private LangManager lang;
        private Cryptage_ModelsWPF EncryptionModelsWPF;

        /// <summary>
        /// Gets the selected language for the application.
        /// </summary>
        public static string SelectedLanguage { get; private set; } = "en";

        /// <summary>
        /// Gets the singleton instance of <see cref="CryptageWPF"/>.
        /// Ensures that only one instance of the window is created (thread-safe).
        /// </summary>
        public static CryptageWPF Instance
        {
            get
            {
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

        /// <summary>
        /// Initializes a new instance of the <see cref="CryptageWPF"/> class.
        /// Sets up the DataContext and initializes the UI elements.
        /// </summary>
        public CryptageWPF()
        {
            InitializeComponent();
            lang = LangManager.Instance;
            EncryptionModelsWPF = Cryptage_ModelsWPF.Instance;
            lang.SetLanguage(SelectedLanguage);
            DataContext = new BusinessApps_ViewModel(SelectedLanguage, false);
            SetColumnHeaders();
        }

        /// <summary>
        /// Handles the click event for the apply button.
        /// Configures encryption settings based on user selection.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event data.</param>
        private void Boutton_Apply_Click(object sender, RoutedEventArgs e)
        {
            string optionText = (OptionComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            string pass = PassTextBox.Password.ToString();
            bool EncryptionALL = false;
            var selectedExtensions = new List<string>();

            if (string.IsNullOrWhiteSpace(optionText))
            {
                System.Windows.MessageBox.Show("Veuillez remplir tous les champs.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (optionText == "Chiffrer toutes les sauvegardes" || optionText == "Encrypt all backups")
            {
                if (string.IsNullOrWhiteSpace(pass))
                {
                    System.Windows.MessageBox.Show("Veuillez remplir tous les champs.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                EncryptionALL = true;
            }
            else if (optionText == "Chiffrer uniquement les extensions sélectionnées" || optionText == "Encrypt only selected extensions")
            {
                if (ExtensionListBox.SelectedItems.Count == 0)
                {
                    System.Windows.MessageBox.Show("Veuillez sélectionner des extensions.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                foreach (ListBoxItem item in ExtensionListBox.SelectedItems)
                {
                    selectedExtensions.Add(item.Content.ToString());
                }
                EncryptionALL = false;
            }
            else if (optionText == "Ne pas chiffrer" || optionText == "Do not encrypt")
            {
                EncryptionModelsWPF.SetEncryptionSettings("KO", false, selectedExtensions.ToArray(), false);
                CloseWindow(sender, e);
                return;
            }
            else
            {
                System.Windows.MessageBox.Show("Option non valide sélectionnée.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            System.Windows.MessageBox.Show($"EncryptionAll: {EncryptionALL}, Extensions sélectionnées: {string.Join(", ", selectedExtensions)}", "Paramètres d'encryption", MessageBoxButton.OK, MessageBoxImage.Information);

            EncryptionModelsWPF.SetEncryptionSettings(pass, EncryptionALL, selectedExtensions.ToArray(), true);
            CloseWindow(sender, e);
        }

        /// <summary>
        /// Sets the application language.
        /// </summary>
        /// <param name="langue">The language code to set.</param>
        public void SetLanguage(string langue)
        {
            SelectedLanguage = langue;
            lang.SetLanguage(langue);
        }

        /// <summary>
        /// Updates UI labels with the selected language translations.
        /// </summary>
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

        /// <summary>
        /// Closes the window.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event data.</param>
        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }
    }
}
