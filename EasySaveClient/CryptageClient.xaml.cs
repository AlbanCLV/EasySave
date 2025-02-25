using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace EasySaveClient
{
    public partial class CryptageClient : Window
    {
        public string Password { get; private set; }
        public bool EncryptAll { get; private set; }
        public List<string> SelectedExtensions { get; private set; }
        public bool EncryptEnabled { get; private set; }

        public CryptageClient()
        {
            InitializeComponent();
            SelectedExtensions = new List<string>();
        }

        private void Apply_Click(object sender, RoutedEventArgs e)
        {
            string selectedOption = (OptionComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            Password = PasswordBox.Password;
            EncryptAll = false;
            EncryptEnabled = true;
            SelectedExtensions.Clear();

            if (string.IsNullOrWhiteSpace(selectedOption))
            {
                MessageBox.Show("Veuillez choisir une option de cryptage.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (selectedOption == "Chiffrer toutes les sauvegardes")
            {
                EncryptAll = true;
            }
            else if (selectedOption == "Chiffrer uniquement certaines extensions")
            {
                foreach (ListBoxItem item in ExtensionListBox.SelectedItems)
                {
                    SelectedExtensions.Add(item.Content.ToString());
                }

                if (SelectedExtensions.Count == 0)
                {
                    MessageBox.Show("Veuillez sélectionner au moins une extension.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }
            else if (selectedOption == "Ne pas chiffrer")
            {
                EncryptEnabled = false;  // ✅ Désactive le cryptage
                Password = string.Empty; // ✅ Efface le mot de passe pour éviter une erreur serveur
                EncryptAll = false;
                SelectedExtensions.Clear();
            }

            DialogResult = true;
            Close();
        }

    }
}
