using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace EasySaveClient
{
    /// <summary>
    /// Represents the encryption settings window.
    /// </summary>
    public partial class CryptageClient : Window
    {
        public string Password { get; private set; }
        public bool EncryptAll { get; private set; }
        public List<string> SelectedExtensions { get; private set; }
        public bool EncryptEnabled { get; private set; }

        /// <summary>
        /// Initializes the encryption settings window.
        /// </summary>
        public CryptageClient()
        {
            InitializeComponent();
            SelectedExtensions = new List<string>();
        }

        /// <summary>
        /// Applies the selected encryption settings when the user clicks "Apply".
        /// </summary>
        private void Apply_Click(object sender, RoutedEventArgs e)
        {
            string selectedOption = (OptionComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            Password = PasswordBox.Password;
            EncryptAll = false;
            EncryptEnabled = true;
            SelectedExtensions.Clear();

            if (string.IsNullOrWhiteSpace(selectedOption))
            {
                MessageBox.Show("Please select an encryption option.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (selectedOption == "Encrypt all backups" || selectedOption == "Encrypt only specific extensions")
            {
                // Ensure that a password is provided
                if (string.IsNullOrWhiteSpace(Password))
                {
                    MessageBox.Show("Please enter a password to enable encryption.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (selectedOption == "Encrypt all backups")
                {
                    EncryptAll = true;
                }
                else if (selectedOption == "Encrypt only specific extensions")
                {
                    foreach (ListBoxItem item in ExtensionListBox.SelectedItems)
                    {
                        SelectedExtensions.Add(item.Content.ToString());
                    }

                    if (SelectedExtensions.Count == 0)
                    {
                        MessageBox.Show("Please select at least one file extension.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }
            }
            else if (selectedOption == "Do not encrypt")
            {
                EncryptEnabled = false;  // Disable encryption
                Password = string.Empty; // Clear the password to avoid server errors
                EncryptAll = false;
                SelectedExtensions.Clear();
            }

            DialogResult = true;
            Close();
        }
    }
}
