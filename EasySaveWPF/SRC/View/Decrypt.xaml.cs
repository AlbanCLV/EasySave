using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using EasySaveConsole.Models;
using EasySaveWPF.ModelsWPF;
using EasySaveWPF.ViewModelsWPF;
using System.Windows.Forms;
using System.IO;  // pour FolderBrowserDialog

namespace EasySaveWPF.Views
{
    /// <summary>
    /// Interaction logic for DeCryptageWPF.
    /// This window allows users to decrypt files using a provided password.
    /// </summary>
    public partial class DeCryptageWPF : Window
    {
        private static DeCryptageWPF _instance;
        private static readonly object _lock = new object();
        private LangManager lang;

        /// <summary>
        /// Gets the selected language for the application.
        /// </summary>
        public static string SelectedLanguage { get; private set; } = "en";

        /// <summary>
        /// Gets the singleton instance of <see cref="DeCryptageWPF"/>.
        /// Ensures that only one instance of the window is created (thread-safe).
        /// </summary>
        public static DeCryptageWPF Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new DeCryptageWPF();
                        }
                    }
                }
                return _instance;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeCryptageWPF"/> class.
        /// Sets up the DataContext and initializes the UI elements.
        /// </summary>
        public DeCryptageWPF()
        {
            InitializeComponent();
            lang = LangManager.Instance;
            lang.SetLanguage(SelectedLanguage);
            DataContext = new BusinessApps_ViewModel(SelectedLanguage, false);
            SetColumnHeaders();
        }

        /// <summary>
        /// Handles the click event for the apply button.
        /// Starts the decryption process for .aes files in the selected directory.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event data.</param>
        public void Boutton_Apply_Click(object sender, RoutedEventArgs e)
        {
            string pass = PassTextBox.Password.ToString();

            if (string.IsNullOrWhiteSpace(pass))
            {
                System.Windows.MessageBox.Show("Veuillez remplir tous les champs.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Cryptage_ModelsWPF.Instance.SetEncryptionSettings(pass, false, null, false);

            string selectedPath = "";
            using (var dialog = new FolderBrowserDialog())
            {
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    selectedPath = dialog.SelectedPath;
                }
                else
                {
                    System.Windows.MessageBox.Show("Aucun dossier sélectionné.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            string[] files = Directory.GetFiles(selectedPath, "*.aes");

            if (files.Length == 0)
            {
                System.Windows.MessageBox.Show("Aucun fichier à décrypter trouvé dans ce dossier.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            foreach (var file in files)
            {
                var decryptResult = Cryptage_ModelsWPF.Instance.DecryptFileWithResult(file);
                if (decryptResult.Item3)
                {
                    System.Windows.MessageBox.Show($"Décryptage réussi pour le fichier : {decryptResult.Item2}", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    System.Windows.MessageBox.Show($"Échec du décryptage pour le fichier : {decryptResult.Item2}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
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
