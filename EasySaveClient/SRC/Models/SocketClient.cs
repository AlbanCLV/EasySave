using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace EasySaveClient.Models
{
    /// <summary>
    /// Represents a TCP socket client for communication with the EasySave server.
    /// </summary>
    public class SocketClient
    {
        private TcpClient client;
        private NetworkStream stream;

        /// <summary>
        /// Event triggered when a message is received from the server.
        /// </summary>
        public event Action<string> OnMessageReceived;

        /// <summary>
        /// Asynchronously connects to the server.
        /// </summary>
        /// <param name="serverIp">IP address of the server.</param>
        /// <param name="port">Port number to connect to.</param>
        public async Task ConnectAsync(string serverIp, int port)
        {
            try
            {
                client = new TcpClient();
                await client.ConnectAsync(serverIp, port);
                stream = client.GetStream();
                _ = ListenForMessagesAsync();
                Console.WriteLine($"[CLIENT] Connected to server at {serverIp}:{port}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CLIENT] Connection failed: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Listens for incoming messages asynchronously.
        /// </summary>
        private async Task ListenForMessagesAsync()
        {
            byte[] buffer = new byte[4096]; // Optimized buffer size
            StringBuilder completeMessage = new StringBuilder();

            while (client?.Connected == true)
            {
                try
                {
                    int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                    if (bytesRead > 0)
                    {
                        string messagePart = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        completeMessage.Append(messagePart);

                        // Check if message is complete
                        if (messagePart.EndsWith("\n") || bytesRead < buffer.Length)
                        {
                            string fullMessage = completeMessage.ToString().Trim();
                            completeMessage.Clear();
                            Console.WriteLine($"[CLIENT] Message received: {fullMessage}");
                            OnMessageReceived?.Invoke(fullMessage);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[CLIENT] Error receiving data: {ex.Message}");
                    Disconnect();
                    break;
                }
            }
        }

        /// <summary>
        /// Sends a message to the server asynchronously.
        /// </summary>
        /// <param name="message">The message to send.</param>
        public async Task SendMessageAsync(string message)
        {
            if (client?.Connected == true)
            {
                try
                {
                    byte[] data = Encoding.UTF8.GetBytes(message + "\n"); // Ensures message termination
                    await stream.WriteAsync(data, 0, data.Length);
                    Console.WriteLine($"[CLIENT] Message sent: {message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[CLIENT] Error sending message: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("[CLIENT] Cannot send message, not connected to server.");
            }
        }

        /// <summary>
        /// Disconnects from the server and releases network resources.
        /// </summary>
        public void Disconnect()
        {
            if (client != null)
            {
                Console.WriteLine("[CLIENT] Disconnecting...");
                stream?.Close();
                client?.Close();
                client = null;
            }
        }
    }
}
