
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
using EasySaveConsole.Views;
using EasySaveLog;
using System.Threading.Tasks;
using System.Diagnostics;

namespace EasySaveWPF
{
    public partial class MainWindow : Window
    {
        private Backup_VueModelsWPF Main; // Instance du contrôleur
        public static string SelectedLanguage { get; private set; } = "en";
        private LangManager lang;
        private Log_ViewModels LogViewModels;

        public MainWindow()
        {
            InitializeComponent();
            Main = Backup_VueModelsWPF.Instance;  // Initialiser le contrôleur
            lang = LangManager.Instance;
            LogViewModels = Log_ViewModels.Instance;
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
                System.Windows.MessageBox.Show(lang.Translate("WPFtaskEmpty"), "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                LogViewModels.LogBackupErreur(nom, "create task", "Not all fields are filled in.", "-1");
                return;
            }
            if (!Directory.Exists(source))
            {
                System.Windows.MessageBox.Show(lang.Translate("SourceDirEmpty"), "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                LogViewModels.LogBackupErreur(nom, "create task", "The source folder could not be found.", "-1");

                return;
            }
            if (!Directory.Exists(destination))
            {
                System.Windows.MessageBox.Show(lang.Translate("SelectTargetDir"), "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                LogViewModels.LogBackupErreur(nom, "create task", "The destination folder could not be found.", "-1");
                return;
            }

            (string reponse, string time)  = Main.CreateBackupTaskWPF(nom, source, destination, type);
            if (reponse == "OK") 
            {
                LogViewModels.LogBackupAction(nom, source, destination, time, "Create Task", "0");
            }
            else if (reponse == "KO")
            {
                LogViewModels.LogBackupErreur(nom, "create tasks", "Error Saving Tasks", "-1");
            }
            System.Windows.MessageBox.Show(lang.Translate("TaskCreated"), " ", MessageBoxButton.OK, MessageBoxImage.None);

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
            (string réponse, string time) = Main.ExecuteSpecificTasks(selectedTask);
            if (réponse == "OK")
            {
                System.Windows.MessageBox.Show(lang.Translate("full_backup_completed"), "Succès", MessageBoxButton.OK, MessageBoxImage.None);
                LogViewModels.LogBackupAction(selectedTask.Name, selectedTask.SourceDirectory, selectedTask.TargetDirectory, time, "execute specific Task", "-1");  // Log the action
            }
            else if (réponse == "KO SOURCE")
            {
                LogViewModels.LogBackupErreur(selectedTask.Name, "execute specific Task", "The source folder could not be found.", "-1");
                System.Windows.MessageBox.Show(lang.Translate("source_directory_not_exist"), " ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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
                System.Windows.MessageBox.Show(lang.Translate("no_tasks_to_execute"), "Succès", MessageBoxButton.OK, MessageBoxImage.None);
                LogViewModels.LogBackupErreur("Error", "Execute_All_Task_attempt", "No_tasks", "-1");
                return;
            }

            (List<Backup_ModelsWPF> executedTasks, List<string> logMessages, string time) = Main.ExecuteALlTask(tasks);
            for (int i = 0; i < executedTasks.Count; i++)
            {
                if (logMessages[i] == "KO SOURCE")
                {
                    LogViewModels.LogBackupErreur(executedTasks[i].Name, "Execute_All_Task_attempt", "source_directory_not_exist", "EncryptionTime");
                    System.Windows.MessageBox.Show(lang.Translate("source_directory_not_exist"), executedTasks[i].Name, MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                else if (logMessages[i] == "OK")
                {
                    LogViewModels.LogBackupAction(executedTasks[i].Name, executedTasks[i].SourceDirectory, executedTasks[i].TargetDirectory, time, "execute ALL Task", "EncryptionTime");  // Log the action
                }
            }
            // Afficher un message avec le résultat de l'exécution
            System.Windows.MessageBox.Show(lang.Translate("full_backup_completed"), "succès", MessageBoxButton.OK, MessageBoxImage.None);
        }


        private void LangueExecute(object sender, RoutedEventArgs e)
        {
            string langue = LangueTextBox.Text.ToLower();
            if (string.IsNullOrWhiteSpace(langue))
            {
                LogViewModels.LogBackupErreur("-1", "Change Language", "Langue Empty", "-1");
                System.Windows.MessageBox.Show(lang.Translate("LangueEmpty"), "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            lang.SetLanguage(langue);
            LogViewModels.LogBackupAction("-1", "-1", "-1", "-1", "change language", "-1");
            System.Windows.MessageBox.Show(lang.Translate("NewLanguage") +" " + langue, "Succès", MessageBoxButton.OK, MessageBoxImage.None);


        }
        private void FichierLogExecute(object sender, RoutedEventArgs e)
        {
            string FichierLog = FichierLogTextBox.Text.ToLower();
            if (string.IsNullOrWhiteSpace(FichierLog))
            {
                System.Windows.MessageBox.Show(lang.Translate("FichierLogEmpty"), "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            Main.SetFichierLog(FichierLog);
            System.Windows.MessageBox.Show(lang.Translate("NewLog") + FichierLog, "Succès", MessageBoxButton.OK, MessageBoxImage.None);

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

















    }
}
