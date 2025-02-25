using System;
using System.Windows;
using System.Threading.Tasks;
using EasySaveClient.Models;
using System.Windows.Controls;

namespace EasySaveClient
{
    public partial class MainWindow : Window
    {
        private SocketClient socketClient = new SocketClient();
        private string serverIp = "127.0.0.1"; // Valeur par défaut

        public MainWindow()
        {
            InitializeComponent();
            socketClient.OnMessageReceived += UpdateUI;
        }

        private async void ConnectToServer_Click(object sender, RoutedEventArgs e)
        {
            serverIp = IpTextBox.Text.Trim(); // Récupère l'IP saisie

            if (string.IsNullOrWhiteSpace(serverIp))
            {
                MessageBox.Show("Veuillez entrer une adresse IP valide.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                await socketClient.ConnectAsync(serverIp, 12345);
                LogsListBox.Items.Add($"[CLIENT] Connecté à {serverIp}:12345");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Échec de connexion à {serverIp}: {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateUI(string message)
        {
            Dispatcher.Invoke(() => LogsListBox.Items.Add(message));
        }

        private async void PauseBackup_Click(object sender, RoutedEventArgs e)
        {
            await socketClient.SendMessageAsync("PAUSE");
        }

        private async void ResumeBackup_Click(object sender, RoutedEventArgs e)
        {
            await socketClient.SendMessageAsync("RESUME");
        }

        private async void StopBackup_Click(object sender, RoutedEventArgs e)
        {
            await socketClient.SendMessageAsync("STOP");
        }
    }
}
