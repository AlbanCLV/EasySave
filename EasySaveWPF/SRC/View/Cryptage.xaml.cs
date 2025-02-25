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
            DataContext = new BusinessApps_ViewModel(SelectedLanguage); // Lier la fenêtre au ViewModel
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
            if (string.IsNullOrWhiteSpace(optionText))
            {
                System.Windows.MessageBox.Show("Veuillez remplir tous les champs.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Définir les paramètres selon l'option sélectionnée
            if (optionText == "Chiffrer toutes les sauvegardes" || optionText == "Encrypt all backups")
            {
                if (string.IsNullOrWhiteSpace(pass))
                {
                    System.Windows.MessageBox.Show("Veuillez remplir tous les champs.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                EncryptionALL = true; // Chiffrer toutes les sauvegardes
            }
            else if (optionText == "Chiffrer uniquement les extensions sélectionnées" || optionText == "Encrypt only selected extensions")
            {
                // Vérifier si des extensions ont été sélectionnées
                if (ExtensionListBox.SelectedItems.Count == 0)
                {
                    System.Windows.MessageBox.Show("Veuillez sélectionner des extensions.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Ajouter les extensions sélectionnées à la liste
                foreach (ListBoxItem item in ExtensionListBox.SelectedItems)
                {
                    selectedExtensions.Add(item.Content.ToString());
                }
                EncryptionALL = false; // Ne pas tout chiffrer, seulement les extensions sélectionnées
            }
            else if (optionText == "Ne pas chiffrer" || optionText == "Do not encrypt")
            {
                // Si "Ne pas chiffrer", on met les options d'encryption sur false
                EncryptionModelsWPF.SetEncryptionSettings("KO", false, selectedExtensions.ToArray(), false);
                CloseWindow(sender, e);
                return;
            }
            else
            {
                // Si aucune option valide n'est sélectionnée
                System.Windows.MessageBox.Show("Option non valide sélectionnée.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Affichage pour débogage ou test (peut être retiré après tests)
            System.Windows.MessageBox.Show($"EncryptionAll: {EncryptionALL}, Extensions sélectionnées: {string.Join(", ", selectedExtensions)}", "Paramètres d'encryption", MessageBoxButton.OK, MessageBoxImage.Information);

            // Appliquer les paramètres d'encryption
            EncryptionModelsWPF.SetEncryptionSettings(pass, EncryptionALL, selectedExtensions.ToArray(), true);

            // Fermer la fenêtre après application
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