using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

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
                    if(_instance == null) {
                        _instance = new SocketServer();
                    }
                    return _instance;
                }
            }
        }
        public void StartServer(int port)
        {
            server = new TcpListener(IPAddress.Loopback, port);
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

                        if (command == "PAUSE") PauseBackup();
                        else if (command == "RESUME") ResumeBackup();
                        else if (command == "STOP") StopBackup();
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

        public async Task SendBackupStatusAsync(string status)
        {
            byte[] data = Encoding.UTF8.GetBytes(status);

            foreach (var client in clients)
            {
                try
                {
                    await client.GetStream().WriteAsync(data, 0, data.Length);
                }
                catch
                {
                    client.Close();
                    clients.Remove(client);
                }
            }
        }

        private void PauseBackup() => Console.WriteLine("[SERVER] Pause demandée.");
        private void ResumeBackup() => Console.WriteLine("[SERVER] Reprise demandée.");
        private void StopBackup() => Console.WriteLine("[SERVER] Arrêt demandé.");
    }
}
