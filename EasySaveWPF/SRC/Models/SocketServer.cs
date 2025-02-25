using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using EasySaveWPF.ViewModelsWPF;

namespace EasySaveWPF.ModelsWPF
{
    public class SocketServer
    {
        private static SocketServer _instance;
        private static readonly object _lock = new object();

        private TcpListener server;
        private List<TcpClient> clients = new List<TcpClient>();

        public static SocketServer Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new SocketServer();
                    }
                    return _instance;
                }
            }
        }

        public void StartServer(int port)
        {
            server = new TcpListener(IPAddress.Any, port);
            server.Start();
            Console.WriteLine($"[SERVER] En attente de connexions sur le port {port}...");

            Task.Run(async () =>
            {
                while (true)
                {
                    TcpClient client = await server.AcceptTcpClientAsync();
                    clients.Add(client);
                    Console.WriteLine($"[SERVER] Client connecté : {client.Client.RemoteEndPoint}");
                    _ = HandleClientAsync(client);
                }
            });
        }

        private async Task HandleClientAsync(TcpClient client)
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
                        Console.WriteLine($"[SERVER] Commande reçue : {command}");

                        if (command == "GET_TASKS")
                        {
                            await SendTaskListAsync(); // Envoie la liste des tâches
                        }
                    }
                }
                catch
                {
                    Console.WriteLine("[SERVER] Client déconnecté.");
                    break;
                }
            }

            clients.Remove(client);
            client.Close();
        }

        public async Task SendTaskListAsync()
        {
            var tasks = Backup_VueModelsWPF.Instance.ViewTasksWPF();

            if (tasks == null || !tasks.Any())
            {
                Console.WriteLine("[SERVER] Aucune tâche à envoyer.");
                return;
            }

            string taskList = string.Join(";", tasks.Select(t => $"{t.Name}:{t.SourceDirectory}->{t.TargetDirectory}"));
            Console.WriteLine($"[SERVER] Envoi des tâches : {taskList}"); // ✅ LOG IMPORTANT

            byte[] data = Encoding.UTF8.GetBytes($"TASKS:{taskList}");

            foreach (var client in clients)
            {
                try
                {
                    await client.GetStream().WriteAsync(data, 0, data.Length);
                    Console.WriteLine("[SERVER] Liste des tâches envoyée au client.");
                }
                catch
                {
                    client.Close();
                    clients.Remove(client);
                }
            }
        }

    }
}
