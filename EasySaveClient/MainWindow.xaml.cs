using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using EasySaveClient.Models;

namespace EasySaveClient
{
    public partial class MainWindow : Window
    {
        private SocketClient socketClient = new SocketClient();
        private string serverIp = "127.0.0.1"; // IP par défaut

        public ObservableCollection<BackupTask> Tasks { get; set; } = new ObservableCollection<BackupTask>();

        public MainWindow()
        {
            InitializeComponent();
            socketClient.OnMessageReceived += UpdateUI;
            TaskListView.ItemsSource = Tasks;
            if (LogsListBox == null)
            {
                Console.WriteLine("[CLIENT] Erreur : LogsListBox est NULL !");
            }
            else
            {
                Console.WriteLine("[CLIENT] LogsListBox trouvé !");
            }
        }

        private async void ConnectToServer_Click(object sender, RoutedEventArgs e)
        {
            serverIp = IpTextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(serverIp))
            {
                MessageBox.Show("Veuillez entrer une adresse IP valide.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                await socketClient.ConnectAsync(serverIp, 12345);
                MessageBox.Show($"Connecté à {serverIp}:12345", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Échec de connexion à {serverIp}: {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void GetTasks_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("[CLIENT] Demande de récupération des tâches envoyée.");
            await socketClient.SendMessageAsync("GET_TASKS");

            await Task.Delay(500); // ✅ Attendre la réponse du serveur

            Console.WriteLine("[CLIENT] Attente de la réponse du serveur...");
        }

        private async void ExecuteTask_Click(object sender, RoutedEventArgs e)
        {
            BackupTask selectedTask = TaskListView.SelectedItem as BackupTask;

            if (selectedTask == null)
            {
                Console.WriteLine("[CLIENT] Erreur : Aucune tâche sélectionnée !");
                MessageBox.Show("Veuillez sélectionner une tâche avant d'exécuter.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string taskName = selectedTask.Name;
            Console.WriteLine($"[CLIENT] Tâche sélectionnée : {taskName}");

            CryptageClient cryptageWindow = new CryptageClient();
            bool? result = cryptageWindow.ShowDialog();

            if (result == true)
            {
                string password = cryptageWindow.Password;
                bool encryptAll = cryptageWindow.EncryptAll;
                List<string> selectedExtensions = cryptageWindow.SelectedExtensions;
                bool encryptEnabled = cryptageWindow.EncryptEnabled;

                // ✅ Vérifier si l'utilisateur a choisi "Ne pas chiffrer"
                string encryptionOptions = encryptEnabled
                    ? $"ENCRYPT:{password}:{encryptAll}:{string.Join(",", selectedExtensions)}"
                    : "NO_ENCRYPT";

                string command = $"EXECUTE:{taskName}:{encryptionOptions}";

                Console.WriteLine($"[CLIENT] Commande envoyée au serveur : {command}");
                await socketClient.SendMessageAsync(command);
                LogsListBox.Items.Add($"[CLIENT] Demande d'exécution de {taskName} avec cryptage : {encryptionOptions}");
            }
            else
            {
                Console.WriteLine("[CLIENT] Fenêtre de cryptage annulée.");
                LogsListBox.Items.Add($"[CLIENT] Exécution annulée par l'utilisateur.");
            }
        }

        private void TaskListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TaskListView.SelectedItem != null)
            {
                BackupTask selectedTask = TaskListView.SelectedItem as BackupTask;
                Console.WriteLine($"[CLIENT] Nouvelle tâche sélectionnée : {selectedTask?.Name}");
            }
            else
            {
                Console.WriteLine("[CLIENT] Aucune tâche sélectionnée !");
            }
        }




        private void UpdateUI(string message)
        {
            Dispatcher.Invoke(() =>
            {
                Console.WriteLine($"[CLIENT] Message reçu : {message}");

                if (message.StartsWith("TASKS:"))
                {
                    string taskList = message.Replace("TASKS:", "").Trim();
                    string[] taskEntries = taskList.Split(';'); // ✅ Séparation des tâches

                    Tasks.Clear(); // ✅ Vider la liste avant d'ajouter de nouvelles tâches
                    foreach (var entry in taskEntries)
                    {
                        Console.WriteLine($"[CLIENT] Traitement de l'entrée : {entry}");

                        // ✅ Vérifier si l'entrée contient bien ":"
                        int firstColonIndex = entry.IndexOf(':');
                        if (firstColonIndex == -1)
                        {
                            Console.WriteLine($"[CLIENT] Erreur : format incorrect pour {entry}");
                            continue; // ❌ Ignorer l'entrée si elle est mal formatée
                        }

                        string taskName = entry.Substring(0, firstColonIndex).Trim();
                        string pathData = entry.Substring(firstColonIndex + 1).Trim();

                        // ✅ Vérifier si la partie des chemins contient bien "->"
                        string[] paths = pathData.Split(new string[] { "->" }, StringSplitOptions.None);
                        if (paths.Length != 2)
                        {
                            Console.WriteLine($"[CLIENT] Erreur : format incorrect des chemins pour {entry}");
                            continue; // ❌ Ignorer l'entrée si elle est mal formatée
                        }

                        Tasks.Add(new BackupTask
                        {
                            Name = taskName,
                            SourceDirectory = paths[0].Trim(),
                            TargetDirectory = paths[1].Trim()
                        });

                        Console.WriteLine($"[CLIENT] Ajout de la tâche : {taskName}");
                    }

                    TaskListView.Items.Refresh(); // ✅ FORCER LE RAFRAÎCHISSEMENT
                    Console.WriteLine($"[CLIENT] {Tasks.Count} tâches affichées.");
                }
            });
        }





    }

    public class BackupTask
    {
        public string Name { get; set; }
        public string SourceDirectory { get; set; }
        public string TargetDirectory { get; set; }
    }
}
