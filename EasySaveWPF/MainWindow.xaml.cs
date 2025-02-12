using EasySave.Controllers;
using EasySave.Models;
using System;
using System.Windows;
using System.Windows.Controls;

namespace EasySaveWPF
{
    public partial class MainWindow : Window
    {
        private BackupJob_Controller backupController; // Instance du contrôleur

        public MainWindow()
        {
            InitializeComponent();
            backupController = BackupJob_Controller.Instance;  // Initialiser le contrôleur
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            // Récupérer les valeurs des champs d'entrée
            string nom = NomTextBox.Text;
            string source = SourceTextBox.Text;
            string destination = DestinationTextBox.Text;
            string type = (TypeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

            // Vérifier que tous les champs sont remplis avant d'ajouter la tâche
            if (string.IsNullOrWhiteSpace(nom) || string.IsNullOrWhiteSpace(source) || string.IsNullOrWhiteSpace(destination) || string.IsNullOrWhiteSpace(type))
            {
                MessageBox.Show("Tous les champs doivent être remplis.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Créer un objet de type BackupJob_Models à partir des informations saisies
            BackupJob_Models newTask = new BackupJob_Models(nom, source, destination, type == "Choice 1" ? BackupType.Full : BackupType.Differential, true);

            // Utiliser le contrôleur pour créer la tâche
            backupController.CreateBackupTask();  // Notez que la méthode CreateBackupTask du contrôleur va maintenant prendre l'objet newTask en paramètre
            Taches.Add(newTask); // Ajouter à la liste observable des tâches affichées dans l'interface
        }

        // Autres méthodes comme DeleteButton_Click, ExecuteButton_Click, etc.
    }
}
