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
using ButtonWPF = System.Windows.Controls.Button;
using ButtonWinForms = System.Windows.Forms.Button;
using EasySaveWPF.Views;
using EasySaveWPF.ModelsWPF;


namespace EasySaveWPF
{
    public partial class MainWindow : Window
    {
        private Backup_VueModelsWPF Main; // Instance du contrôleur
        public static string SelectedLanguage { get; private set; } = "en";
        private LangManager lang;
        private Log_ViewModels Log_VM;
        private BusinessAppsWindow Business;
        private CryptageWPF Cryptage;


        public MainWindow()
        {
            InitializeComponent();
            Main = Backup_VueModelsWPF.Instance;  // Initialiser le contrôleur
            lang = LangManager.Instance;
            Log_VM = Log_ViewModels.Instance;
            Business = BusinessAppsWindow.Instance;
            Cryptage = CryptageWPF.Instance;
            Setlanguage(SelectedLanguage);

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
                System.Windows.MessageBox.Show(lang.Translate("WPFtaskEmpty"), lang.Translate("Error"), MessageBoxButton.OK, MessageBoxImage.Error);
                Log_VM.LogBackupErreur(nom, "create_task_attempt", "Not all fields are filled in.", "-1");
                return;
            }
            if (!Directory.Exists(source))
            {
                System.Windows.MessageBox.Show(lang.Translate("SourceDirEmpty"), lang.Translate("Error"), MessageBoxButton.OK, MessageBoxImage.Error);
                Log_VM.LogBackupErreur(nom, "create_task_attempt", "The source folder could not be found.", "-1");

                return;
            }
            if (!Directory.Exists(destination))
            {
                System.Windows.MessageBox.Show(lang.Translate("SelectTargetDir"), "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                Log_VM.LogBackupErreur(nom, "create_task_attempt", "The destination folder could not be found.", "-1");
                return;
            }
            var result = System.Windows.MessageBox.Show(lang.Translate("ConfirmADD"), "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {

                (string reponse, string time) = Main.CreateBackupTaskWPF(nom, source, destination, type);
                if (reponse == "OK")
                {
                    Log_VM.LogBackupAction(nom, source, destination, time, "Create Task", type);
                }
                else if (reponse == "KO")
                {
                    Log_VM.LogBackupErreur(nom, "create_task_attempt", "Error Saving Tasks", "-1");
                }
                System.Windows.MessageBox.Show(lang.Translate("TaskCreated"), lang.Translate("Success"), MessageBoxButton.OK, MessageBoxImage.None);

            }
            else
            {
                Log_VM.LogBackupErreur(nom, "create_task_attempt", "Select NO during confirmation", "-1");
            }

            ViewButton_Click(sender, e);


        }
        private void ViewButton_Click(object sender, RoutedEventArgs e)
        {
            TasksDataGrid.ItemsSource = null;
            TasksDataGrid.ItemsSource = Main.ViewTasksWPF();
            Log_VM.LogBackupAction("-1", "-1", "-1", "-1", "View Task", "-1");  // Log the action
        }
        private void ExecuteButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedTask = TasksDataGrid.SelectedItem as Backup_ModelsWPF;

            var result = System.Windows.MessageBox.Show(lang.Translate("ConfirmExecute"), "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                if (Cryptage == null || !Cryptage.IsVisible)
                {
                    Cryptage = new CryptageWPF();
                }
                Cryptage.ShowDialog();  // Affiche la fenêtre de cryptage pour que l'utilisateur définisse ses paramètres
                // Récupérer les paramètres de cryptage définis précédemment depuis EncryptionSettings
                string password = Cryptage_ModelsWPF.UserPassword;
                bool encryptAll = Cryptage_ModelsWPF.EncryptAll;
                string[] selectedExtensions = Cryptage_ModelsWPF.SelectedExtensions;
                bool encry = Cryptage_ModelsWPF.EncryptAll;

                // Exécuter la tâche en fonction des paramètres récupérés
                (string réponse, string time, string timeencrypt) = Main.ExecuteSpecificTasks(selectedTask);

                if (réponse == "OK")
                {
                    System.Windows.MessageBox.Show(lang.Translate("full_backup_completed"), lang.Translate("Success"), MessageBoxButton.OK, MessageBoxImage.None);
                    Log_VM.LogBackupAction(selectedTask.Name, selectedTask.SourceDirectory, selectedTask.TargetDirectory, time, "execute specific Task", timeencrypt);  // Log the action
                }
                else if (réponse == "KO SOURCE")
                {
                    Log_VM.LogBackupErreur(selectedTask.Name, "Execute_Task_attempt", "The source folder could not be found.", timeencrypt);
                    System.Windows.MessageBox.Show(lang.Translate("source_directory_not_exist"), " ", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                Log_VM.LogBackupErreur(selectedTask.Name, "Execute_Task_attempt", "select NO during confirmation", "-1");
            }

            // Réinitialiser la source des tâches
            TasksDataGrid.ItemsSource = null;
            TasksDataGrid.ItemsSource = Main.ViewTasksWPF();
        }

        private void ExecuteAllButton_Click(object sender, RoutedEventArgs e)
        {
            var tasks = TasksDataGrid.ItemsSource as List<Backup_ModelsWPF>;
            if (tasks == null || !tasks.Any())
            {
                System.Windows.MessageBox.Show(lang.Translate("no_tasks_to_execute"), "Succès", MessageBoxButton.OK, MessageBoxImage.None);
                Log_VM.LogBackupErreur("Error", "Execute_All_Task_attempt", "No_tasks", "-1");
                return;
            }
            var result = System.Windows.MessageBox.Show(lang.Translate("ConfirmAllExecute"), "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {

                if (Cryptage == null || !Cryptage.IsVisible)
                {
                    Cryptage = new CryptageWPF();
                }
                Cryptage.ShowDialog();  // Affiche la fenêtre de cryptage pour que l'utilisateur définisse ses paramètres
                // Récupérer les paramètres de cryptage définis précédemment depuis EncryptionSettings
                string password = Cryptage_ModelsWPF.UserPassword;
                bool encryptAll = Cryptage_ModelsWPF.EncryptAll;
                string[] selectedExtensions = Cryptage_ModelsWPF.SelectedExtensions;
                bool encry = Cryptage_ModelsWPF.EncryptAll;

                (List<Backup_ModelsWPF> executedTasks, List<string> logMessages, string time, List<string> TimeEncry) = Main.ExecuteALlTask(tasks);
                for (int i = 0; i < executedTasks.Count; i++)
                {
                    if (logMessages[i] == "KO SOURCE")
                    {
                        Log_VM.LogBackupErreur(executedTasks[i].Name, "Execute_Task_attempt", "source_directory_not_exist", TimeEncry[i]);
                        System.Windows.MessageBox.Show(lang.Translate("source_directory_not_exist"), executedTasks[i].Name, MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    else if (logMessages[i] == "OK")
                    {
                        Log_VM.LogBackupAction(executedTasks[i].Name, executedTasks[i].SourceDirectory, executedTasks[i].TargetDirectory, time, "execute ALL Task", TimeEncry[i]);  // Log the action
                    }
                }
                // Afficher un message avec le résultat de l'exécution
                System.Windows.MessageBox.Show(lang.Translate("full_backup_completed"), lang.Translate("Success"), MessageBoxButton.OK, MessageBoxImage.None);

            }
            else
            {
                Log_VM.LogBackupErreur("All Tasks", "Execute_Task_attempt", "select NO during confirmation.", "-1");

            }
        }
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var result = System.Windows.MessageBox.Show(lang.Translate("ConfirmDelete"), "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
            var selectedTask = TasksDataGrid.SelectedItem as Backup_ModelsWPF;

            if (result == MessageBoxResult.Yes)
            {
                string t = Main.DeleteBackupTaskWPF(selectedTask);
                System.Windows.MessageBox.Show(t, lang.Translate("Success"), MessageBoxButton.OK, MessageBoxImage.None);
                TasksDataGrid.ItemsSource = null;
                TasksDataGrid.ItemsSource = Main.ViewTasksWPF();
            }
            else
            {
                Log_VM.LogBackupErreur(selectedTask.Name, "delete_task_attempt", "Select NO during confirmation", "-1");
            }
        }
        private void LangueExecute(object sender, RoutedEventArgs e)
        {
            string langue = LangueTextBox.Text.ToLower();
            if (string.IsNullOrWhiteSpace(langue))
            {
                Log_VM.LogBackupErreur("-1", "Change Language", "Langue Empty", "-1");
                System.Windows.MessageBox.Show(lang.Translate("LangueEmpty"), lang.Translate("Error"), MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            Setlanguage(langue);

            Log_VM.LogBackupAction("-1", langue, "-1", "-1", "change language", "-1");
            System.Windows.MessageBox.Show(lang.Translate("NewLanguage") +" " + langue, lang.Translate("Success"), MessageBoxButton.OK, MessageBoxImage.None);


        }
        private void FichierLogExecute(object sender, RoutedEventArgs e)
        {
            string FichierLog = FichierLogTextBox.Text.ToLower();
            if (string.IsNullOrWhiteSpace(FichierLog))
            {
                System.Windows.MessageBox.Show(lang.Translate("FichierLogEmpty"), lang.Translate("Error"), MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            Main.SetFichierLog(FichierLog);
            System.Windows.MessageBox.Show(lang.Translate("NewLog") + FichierLog, lang.Translate("Success"), MessageBoxButton.OK, MessageBoxImage.None);
            Log_VM.LogBackupAction("-1", "Unknow", FichierLog, "-1", "ChooseFileLog", "-1");
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

        private void SetColumnHeaders()
        {
            // Exemple pour le DataGrid
            TasksDataGrid.Columns[0].Header = lang.Translate("task_name"); // Traduction de "Nom"
            TasksDataGrid.Columns[1].Header = lang.Translate("source_directory"); // Traduction de "Source"
            TasksDataGrid.Columns[2].Header = lang.Translate("target_directory"); // Traduction de "Destination"
            TasksDataGrid.Columns[3].Header = lang.Translate("backup_type"); // Traduction de "Type"
            TaskNameTextBlock.Text = lang.Translate("task_name"); // Traduction de "Nom"
            SourceTextBlock.Text = lang.Translate("source_directory"); // Traduction de "Nom"
            DestinationTextBlock.Text = lang.Translate("target_directory"); // Traduction de "Nom"
            TypeTextBLock.Text = lang.Translate("backup_type"); // Traduction de "Nom"
            Boutton_ADD.Content = lang.Translate("Create");
            Boutton_View.Content = lang.Translate("View");
            Boutton_execute_All.Content = lang.Translate("ExeAll");
            Boutton_Log_app.Content = lang.Translate("App");
            Boutton_Langue_app.Content = lang.Translate("App");
            LangueTextBlock.Text = lang.Translate("Langue");
            FichierLogTextBlock.Text = lang.Translate("FileLog");
            Boutton_Metier.Content = lang.Translate("businesssoftware");
        }

        private void OpenBusinessAppsWindow(object sender, RoutedEventArgs e)
        {
            if (Business == null || !Business.IsVisible)
            {
                Business = new BusinessAppsWindow();
            }
            Business.ShowDialog();
        }

        private void Setlanguage(string langue)
        {
            lang.SetLanguage(langue);
            Business.SetLanguage(langue);
            Cryptage.SetLanguage(langue);

            SetColumnHeaders();
            Business.SetColumnHeaders();
            Cryptage.SetColumnHeaders();
        }











    }
}