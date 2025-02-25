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
    public partial class DeCryptageWPF : Window
    {
        private static DeCryptageWPF _instance;
        public static string SelectedLanguage { get; private set; } = "en";
        private static readonly object _lock = new object();
        private LangManager lang;

        public DeCryptageWPF()
        {
            InitializeComponent();
            lang = LangManager.Instance;
            lang.SetLanguage(SelectedLanguage);
            DataContext = new BusinessApps_ViewModel(SelectedLanguage, false); // Lier la fenêtre au ViewModel
            SetColumnHeaders();

        }
        public static DeCryptageWPF Instance
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
                            _instance = new DeCryptageWPF();
                        }
                    }
                }
                return _instance;
            }
        }

        public void Boutton_Apply_Click(object sender, RoutedEventArgs e)
        {
            // Récupérer le mot de passe
            string pass = PassTextBox.Password.ToString();

            // Vérifier si le mot de passe est vide
            if (string.IsNullOrWhiteSpace(pass))
            {
                System.Windows.MessageBox.Show("Veuillez remplir tous les champs.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Lire le mot de passe (l'ancienne fonction ReadPassword semble obsolète ici)
            Cryptage_ModelsWPF.Instance.SetEncryptionSettings(pass, false, null, false);

            // Ouvrir le dialog de sélection du dossier
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

            // Liste des fichiers .aes dans le dossier sélectionné
            string[] files = Directory.GetFiles(selectedPath, "*.aes");

            // Vérifier s'il y a des fichiers .aes dans le dossier
            if (files.Length == 0)
            {
                System.Windows.MessageBox.Show("Aucun fichier à décrypter trouvé dans ce dossier.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Démarrer le processus de décryptage
            foreach (var file in files)
            {
                var decryptResult = Cryptage_ModelsWPF.Instance.DecryptFileWithResult(file);

                // Vérifier le résultat de la décryption
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


        public void SetLanguage(string langue)
        {
            SelectedLanguage = langue;
            lang.SetLanguage(langue);
        }
        public void SetColumnHeaders()
        {
            Boutton_Apply.Content = lang.Translate("Launch");
            PassTextBloc.Text = lang.Translate("MDP");
        }
        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }
    }
}