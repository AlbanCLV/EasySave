using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace EasySaveClient.Models
{
    public class SocketClient
    {
        private TcpClient client;
        private NetworkStream stream;
        public event Action<string> OnMessageReceived;

        public async Task ConnectAsync(string serverIp, int port)
        {
            client = new TcpClient();
            await client.ConnectAsync(serverIp, port);
            stream = client.GetStream();
            _ = ListenForMessagesAsync();
        }

        private async Task ListenForMessagesAsync()
        {
            byte[] buffer = new byte[1024];

            while (client.Connected)
            {
                try
                {
                    int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                    if (bytesRead > 0)
                    {
                        string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        OnMessageReceived?.Invoke(message);
                    }
                }
                catch
                {
                    break;
                }
            }
        }

        public async Task SendMessageAsync(string message)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            await stream.WriteAsync(data, 0, data.Length);
        }

        public void Disconnect()
        {
            stream?.Close();
            client?.Close();
        }
    }
}
