using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using EasySaveClient.Models;

namespace EasySaveClient
{
    /// <summary>
    /// Main window class for the EasySave client.
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly SocketClient socketClient = new SocketClient();
        private string serverIp = "127.0.0.1"; // Default IP address

        /// <summary>
        /// Collection of backup tasks displayed in the UI.
        /// </summary>
        public ObservableCollection<BackupTask> Tasks { get; set; } = new ObservableCollection<BackupTask>();

        /// <summary>
        /// Constructor initializing the UI and socket communication.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            socketClient.OnMessageReceived += UpdateUI;
            TaskListView.ItemsSource = Tasks;

            if (LogsListBox == null)
            {
                Console.WriteLine("[CLIENT] Error: LogsListBox is NULL.");
            }
        }

        /// <summary>
        /// Attempts to connect to the backup server.
        /// </summary>
        private async void ConnectToServer_Click(object sender, RoutedEventArgs e)
        {
            serverIp = IpTextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(serverIp))
            {
                MessageBox.Show("Please enter a valid IP address.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                await socketClient.ConnectAsync(serverIp, 12345);
                MessageBox.Show($"Connected to {serverIp}:12345", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to connect to {serverIp}: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Sends a request to the server to retrieve the list of backup tasks.
        /// </summary>
        private async void GetTasks_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("[CLIENT] Requesting task list from the server.");
            await socketClient.SendMessageAsync("GET_TASKS");

            await Task.Delay(500); // Wait for server response
            Console.WriteLine("[CLIENT] Waiting for server response...");
        }

        /// <summary>
        /// Executes the selected backup task with optional encryption settings.
        /// </summary>
        private async void ExecuteTask_Click(object sender, RoutedEventArgs e)
        {
            if (TaskListView.SelectedItem is not BackupTask selectedTask)
            {
                MessageBox.Show("Please select a task before executing.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Console.WriteLine($"[CLIENT] Selected task: {selectedTask.Name}");

            var cryptageWindow = new CryptageClient();
            bool? result = cryptageWindow.ShowDialog();

            Console.WriteLine($"[CLIENT] Encryption window opened, result: {result}");

            string encryptionOptions = "NO_ENCRYPT";

            if (result == true && cryptageWindow.EncryptEnabled)
            {
                encryptionOptions = $"ENCRYPT:{cryptageWindow.Password}:{cryptageWindow.EncryptAll}:{string.Join(",", cryptageWindow.SelectedExtensions)}";
            }

            // Remove the password from the log but keep it in the command sent to the server
            string displayEncryptionOptions = encryptionOptions.StartsWith("ENCRYPT")
                ? $"ENCRYPT:<hidden_password>:{cryptageWindow.EncryptAll}:{string.Join(",", cryptageWindow.SelectedExtensions)}"
                : encryptionOptions;

            string command = $"EXECUTE:{selectedTask.Name}:{encryptionOptions}";

            Console.WriteLine($"[CLIENT] Sending command to server: {command}");
            await socketClient.SendMessageAsync(command);
            LogsListBox.Items.Add($"[CLIENT] Execution request sent for {selectedTask.Name} with encryption: {displayEncryptionOptions}");
        }


        /// <summary>
        /// Handles selection changes in the task list.
        /// </summary>
        private void TaskListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TaskListView.SelectedItem is BackupTask selectedTask)
            {
                Console.WriteLine($"[CLIENT] Selected task: {selectedTask.Name}");
            }
            else
            {
                Console.WriteLine("[CLIENT] No task selected.");
            }
        }

        /// <summary>
        /// Updates the UI with received messages from the server.
        /// </summary>
        private void UpdateUI(string message)
        {
            Dispatcher.Invoke(() =>
            {
                Console.WriteLine($"[CLIENT] Message received: {message}");

                if (!message.StartsWith("TASKS:")) return;

                string taskList = message.Replace("TASKS:", "").Trim();
                string[] taskEntries = taskList.Split(';');

                Tasks.Clear();

                foreach (var entry in taskEntries)
                {
                    Console.WriteLine($"[CLIENT] Processing entry: {entry}");

                    // Ensure the entry has a valid format
                    int firstColonIndex = entry.IndexOf(':');
                    if (firstColonIndex == -1)
                    {
                        Console.WriteLine($"[CLIENT] Error: Invalid format for {entry}");
                        continue;
                    }

                    string taskName = entry.Substring(0, firstColonIndex).Trim();
                    string pathData = entry.Substring(firstColonIndex + 1).Trim();

                    // Validate source and target directories
                    string[] paths = pathData.Split(new string[] { "->" }, StringSplitOptions.None);
                    if (paths.Length != 2)
                    {
                        Console.WriteLine($"[CLIENT] Error: Invalid path format for {entry}");
                        continue;
                    }

                    Tasks.Add(new BackupTask
                    {
                        Name = taskName,
                        SourceDirectory = paths[0].Trim(),
                        TargetDirectory = paths[1].Trim()
                    });

                    Console.WriteLine($"[CLIENT] Task added: {taskName}");
                }

                TaskListView.Items.Refresh();
                Console.WriteLine($"[CLIENT] {Tasks.Count} tasks displayed.");
            });
        }
    }

    /// <summary>
    /// Represents a backup task with its properties.
    /// </summary>
    public class BackupTask
    {
        public string Name { get; set; }
        public string SourceDirectory { get; set; }
        public string TargetDirectory { get; set; }
    }
}
