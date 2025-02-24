using System;
using System.IO;        // Pour manipuler les fichiers et répertoires
using Microsoft.Win32;  // Pour OpenFileDialog
using System.Windows;
using System.Windows.Controls;
using EasySaveWPF.ModelsWPF;
using EasySaveWPF.ViewModelsWPF;
using System.Windows.Forms;
using System.Collections.ObjectModel;
using EasySaveConsole.Models;
using EasySaveWPF.Views;
namespace EasySaveWPF
{
    public partial class MainWindow : Window
    {
        private Backup_VueModelsWPF Main; // Instance du contrôleur
        public static string SelectedLanguage { get; private set; } = "fr";
        private LangManager lang;

        public MainWindow()
        {
            InitializeComponent();
            Main = Backup_VueModelsWPF.Instance;  // Initialiser le contrôleur
            lang = LangManager.Instance;
            lang.SetLanguage(SelectedLanguage);
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            // Récupérer les valeurs des champs d'entrée
            string nom = NomTextBox.Text;
            string source = SourceTextBox.Text;
            string destination = DestinationTextBox.Text;
            string type = (TypeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

            // Vérifier que tous les champs sont remplis
            if (string.IsNullOrWhiteSpace(nom) || string.IsNullOrWhiteSpace(source) || string.IsNullOrWhiteSpace(destination) || string.IsNullOrWhiteSpace(type))
            {
                System.Windows.MessageBox.Show("Tous les champs doivent être remplis.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (!Directory.Exists(source))
            {
                System.Windows.MessageBox.Show("Le chemin source n'existe pas ou n'est pas un dossier valide.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (!Directory.Exists(destination))
            {
                System.Windows.MessageBox.Show("Le chemin de destination n'existe pas ou n'est pas un dossier valide.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string t = Main.CreateBackupTaskWPF(nom, source, destination, type);
            System.Windows.MessageBox.Show(t, "Succès", MessageBoxButton.OK, MessageBoxImage.None);
            System.Windows.MessageBox.Show(lang.Translate("Choice_log_display"), "Succès", MessageBoxButton.OK, MessageBoxImage.None);

            ViewButton_Click(sender, e);


        }

        private void ViewButton_Click(object sender, RoutedEventArgs e)
        {
            TasksDataGrid.ItemsSource = null;
            TasksDataGrid.ItemsSource = Main.ViewTasksWPF();
        }

        private void ExecuteButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedTask = TasksDataGrid.SelectedItem as Backup_ModelsWPF;
            string t = Main.ExecuteSpecificTasks(selectedTask);
            System.Windows.MessageBox.Show(t, "Succès", MessageBoxButton.OK, MessageBoxImage.None);
            TasksDataGrid.ItemsSource = null;
            TasksDataGrid.ItemsSource = Main.ViewTasksWPF();

        }
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedTask = TasksDataGrid.SelectedItem as Backup_ModelsWPF;
            string t = Main.DeleteBackupTaskWPF(selectedTask);
            System.Windows.MessageBox.Show(t, "Succès", MessageBoxButton.OK, MessageBoxImage.None);
            TasksDataGrid.ItemsSource = null;
            TasksDataGrid.ItemsSource = Main.ViewTasksWPF();
        }
        private void ExecuteAllButton_Click(object sender, RoutedEventArgs e)
        {
            var tasks = TasksDataGrid.ItemsSource as List<Backup_ModelsWPF>;

            if (tasks == null || !tasks.Any())
            {
                System.Windows.MessageBox.Show("La liste des tâches est vide ou invalide.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Appeler la méthode ExecuteAllTask avec la liste des tâches
            string t = Main.ExecuteALlTask(tasks);

            // Afficher un message avec le résultat de l'exécution
            System.Windows.MessageBox.Show(t, "Succès", MessageBoxButton.OK, MessageBoxImage.None);
        }


        private void LangueExecute(object sender, RoutedEventArgs e)
        {
            string langue = LangueTextBox.Text;
            System.Windows.MessageBox.Show(lang.Translate("Choice_log_display"), "Succès", MessageBoxButton.OK, MessageBoxImage.None);
            System.Windows.MessageBox.Show(lang.Translate("EnterTaskName"), "Succès", MessageBoxButton.OK, MessageBoxImage.None);


        }
        private void FichierLogExecute(object sender, RoutedEventArgs e)
        {
            string FichierLog = FichierLogTextBox.Text;

        }



        private void BrowseSourceButton_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    SourceTextBox.Text = dialog.SelectedPath;
                }
            }
        }

        private void BrowseDestinationButton_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    DestinationTextBox.Text = dialog.SelectedPath;
                }
            }
        }
        private void OpenBusinessAppsWindow(object sender, RoutedEventArgs e)
        {
            BusinessAppsWindow window = new BusinessAppsWindow();
            window.ShowDialog(); // Ouvre la fenêtre en mode modal
        }



    }
}