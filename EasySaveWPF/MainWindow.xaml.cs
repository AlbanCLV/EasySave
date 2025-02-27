using System;
using System.IO;        // File and directory management
using Microsoft.Win32;  // File dialog management
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
using Newtonsoft.Json.Linq;

namespace EasySaveWPF
{
    public partial class MainWindow : Window
    {
        private CancellationTokenSource _cancellationTokenSource;
        private bool _isPaused = false;

        private Backup_VueModelsWPF Main;
        public static string SelectedLanguage { get; private set; } = "en";
        private LangManager lang;
        private Log_ViewModels Log_VM;
        private BusinessAppsWindow Business;
        private CryptageWPF Cryptage;
        private DeCryptageWPF DeCryptage;
        private SocketServer server;
        private PriorityExtensionsWindow priorityExtensionsWindow;

        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public MainWindow()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            CancellationToken token = _cancellationTokenSource.Token;
            InitializeComponent();
            server = SocketServer.Instance;
            server.StartServer(12345, token, this);
            Main = Backup_VueModelsWPF.Instance;
            lang = LangManager.Instance;
            Log_VM = Log_ViewModels.Instance;
            Business = BusinessAppsWindow.Instance;
            Cryptage = CryptageWPF.Instance;
            DeCryptage = DeCryptageWPF.Instance;
            priorityExtensionsWindow = PriorityExtensionsWindow.Instance;
            Setlanguage(SelectedLanguage);
        }

        /// <summary>
        /// Handles the Add Button click event to create a new backup task.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            /// <summary>Retrieve values from input fields</summary>
            string nom = NomTextBox.Text;
            string source = SourceTextBox.Text;
            string destination = DestinationTextBox.Text;
            string type = (TypeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

            /// <summary>Check that all fields are filled</summary>
            if (string.IsNullOrWhiteSpace(nom) || string.IsNullOrWhiteSpace(source) || string.IsNullOrWhiteSpace(destination) || string.IsNullOrWhiteSpace(type))
            {
                System.Windows.MessageBox.Show(lang.Translate("WPFtaskEmpty"), lang.Translate("Error"), MessageBoxButton.OK, MessageBoxImage.Error);
                Log_VM.LogBackupErreur(nom, "create_task_attempt", "All fields are not filled.", "-1");
                return;
            }

            /// <summary>Verify that the source directory exists</summary>
            if (!Directory.Exists(source))
            {
                System.Windows.MessageBox.Show(lang.Translate("SourceDirEmpty"), lang.Translate("Error"), MessageBoxButton.OK, MessageBoxImage.Error);
                Log_VM.LogBackupErreur(nom, "create_task_attempt", "Source folder not found.", "-1");
                return;
            }

            /// <summary>Verify that the destination directory exists</summary>
            if (!Directory.Exists(destination))
            {
                System.Windows.MessageBox.Show(lang.Translate("SelectTargetDir"), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Log_VM.LogBackupErreur(nom, "create_task_attempt", "Destination folder not found.", "-1");
                return;
            }

            var result = System.Windows.MessageBox.Show(lang.Translate("ConfirmADD"), "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                (string reponse, string time) = Main.CreateBackupTaskWPF(nom, source, destination, type);
                if (reponse == "OK")
                {
                    Log_VM.LogBackupAction(nom, source, destination, time, "Task creation", type);
                }
                else if (reponse == "KO")
                {
                    Log_VM.LogBackupErreur(nom, "create_task_attempt", "Error saving the task", "-1");
                    return;
                }
                else if (reponse == "KONAME")
                {
                    Log_VM.LogBackupErreur(nom, "create_task_attempt", "Name already in use", "-1");
                    System.Windows.MessageBox.Show(lang.Translate("TaskCreatedName"), lang.Translate("Error"), MessageBoxButton.OK, MessageBoxImage.None);
                    return;
                }
                System.Windows.MessageBox.Show(lang.Translate("TaskCreated"), lang.Translate("Success"), MessageBoxButton.OK, MessageBoxImage.None);
            }
            else
            {
                Log_VM.LogBackupErreur(nom, "create_task_attempt", "User selected NO during confirmation", "-1");
                return;
            }

            ViewButton_Click(sender, e);
        }
    }
}
