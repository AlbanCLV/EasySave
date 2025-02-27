using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using EasySaveWPF.ViewModelsWPF;
using System.Windows;
using EasySaveWPF.Views;
using Newtonsoft.Json.Linq;

namespace EasySaveWPF.ModelsWPF
{
    /// <summary>
    /// Singleton TCP server handling client connections and task execution requests.
    /// </summary>
    public class SocketServer
    {
        private static SocketServer _instance;
        private static readonly object _lock = new object();

        private TcpListener server;
        private readonly List<TcpClient> clients = new List<TcpClient>();

        /// <summary>
        /// Gets the singleton instance of the server.
        /// </summary>
        public static SocketServer Instance
        {
            get
            {
                lock (_lock)
                {
                    return _instance ??= new SocketServer();
                }
            }
        }

        /// <summary>
        /// Starts the TCP server on the specified port.
        /// </summary>
        public void StartServer(int port, CancellationToken token, MainWindow main)
        {
            server = new TcpListener(IPAddress.Any, port);
            server.Start();
            Console.WriteLine($"[SERVER] Waiting for connections on port {port}...");

            Task.Run(async () =>
            {
                while (true)
                {
                    try
                    {
                        TcpClient client = await server.AcceptTcpClientAsync();
                        clients.Add(client);
                        Console.WriteLine($"[SERVER] Client connected: {client.Client.RemoteEndPoint}");
                        _ = HandleClientAsync(client, token, main);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[SERVER] Error accepting client connection: {ex.Message}");
                    }
                }
            });
        }

        /// <summary>
        /// Handles incoming client messages asynchronously.
        /// </summary>
        private async Task HandleClientAsync(TcpClient client, CancellationToken token, MainWindow main)
        {
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[1024];

            while (client.Connected)
            {
                try
                {
                    int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                    if (bytesRead > 0)
                    {
                        string command = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();
                        Console.WriteLine($"[SERVER] Command received: {command}");

                        if (command == "GET_TASKS")
                        {
                            await SendTaskListAsync();
                        }
                        else if (command.StartsWith("EXECUTE:"))
                        {
                            await ExecuteTaskAsync(command, token, main);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[SERVER] Client disconnected: {ex.Message}");
                    break;
                }
            }

            clients.Remove(client);
            client.Close();
        }

        /// <summary>
        /// Sends the list of available backup tasks to all connected clients.
        /// </summary>
        public async Task SendTaskListAsync()
        {
            var tasks = Backup_VueModelsWPF.Instance.ViewTasksWPF();

            if (tasks == null || !tasks.Any())
            {
                Console.WriteLine("[SERVER] No tasks available to send.");
                return;
            }

            string taskList = string.Join(";", tasks.Select(t => $"{t.Name}:{t.SourceDirectory}->{t.TargetDirectory}"));
            byte[] data = Encoding.UTF8.GetBytes($"TASKS:{taskList}");

            foreach (var client in clients.ToList()) // Avoid modification during iteration
            {
                try
                {
                    await client.GetStream().WriteAsync(data, 0, data.Length);
                    Console.WriteLine("[SERVER] Task list sent to client.");
                }
                catch
                {
                    client.Close();
                    clients.Remove(client);
                }
            }
        }

        /// <summary>
        /// Executes a specified task based on the received command.
        /// </summary>
        private async Task ExecuteTaskAsync(string command, CancellationToken token, MainWindow main)
        {
            Console.WriteLine($"[SERVER] Processing execution command: {command}");

            string[] parts = command.Split(':');
            if (parts.Length < 2)
            {
                Console.WriteLine("[SERVER] Error: Invalid command format.");
                return;
            }

            string taskName = parts[1];
            string encryptionOptions = parts.Length > 2 ? string.Join(":", parts.Skip(2)) : "NO_ENCRYPT";

            Console.WriteLine($"[SERVER] Executing task: {taskName}, Encryption: {encryptionOptions}");

            if (encryptionOptions.StartsWith("ENCRYPT"))
            {
                ProcessEncryptionSettings(encryptionOptions);
            }
            else
            {
                Console.WriteLine("[SERVER] No encryption settings. Proceeding with execution.");
                Cryptage_ModelsWPF.Instance.SetEncryptionSettings("KO", false, Array.Empty<string>(), false);
            }

            var task = Backup_VueModelsWPF.Instance.ViewTasksWPF().FirstOrDefault(t => t.Name == taskName);
            if (task != null)
            {
                Console.WriteLine($"[SERVER] Starting backup for {taskName}...");
                (string status, string time, string encryptTime) = Backup_VueModelsWPF.Instance.ExecuteSpecificTasks(task,token, main);
                Console.WriteLine($"[SERVER] Task {taskName} completed. Status: {status}, Encryption Time: {encryptTime}");
            }
            else
            {
                Console.WriteLine($"[SERVER] Error: Task {taskName} not found.");
            }
        }

        /// <summary>
        /// Processes encryption settings extracted from the command.
        /// </summary>
        private void ProcessEncryptionSettings(string encryptionOptions)
        {
            string[] encParts = encryptionOptions.Split(':');

            string password = encParts.Length > 1 ? encParts[1] : "";
            bool encryptAll = encParts.Length > 2 && bool.TryParse(encParts[2], out bool result) && result;
            string[] selectedExtensions = encParts.Length > 3 ? encParts[3].Split(',') : Array.Empty<string>();

            Console.WriteLine($"[SERVER] Encryption settings: Password={password}, EncryptAll={encryptAll}, Extensions={string.Join(",", selectedExtensions)}");

            Cryptage_ModelsWPF.Instance.SetEncryptionSettings(password, encryptAll, selectedExtensions, true);

            Console.WriteLine($"[SERVER] Post-encryption settings: Password={Cryptage_ModelsWPF.UserPassword}, EncryptAll={Cryptage_ModelsWPF.EncryptAll}");
        }
    }
}
