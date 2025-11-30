using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Messenger
{
    public class ChatClient
    {
        private TcpClient? _client;
        private string _username = string.Empty;
        private readonly CancellationTokenSource _cancellationTokenSource = new();
        private bool _isConnected;

        public event Action<Message>? MessageReceived;
        public event Action? Disconnected; // вызывается при потере соединения

        public bool IsConnected => _isConnected;

        // подключение к серверу
        public async Task<bool> ConnectAsync(string serverIp, int port, string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Имя пользователя не может быть пустым.", nameof(username));

            try
            {
                _client = new TcpClient();
                await _client.ConnectAsync(serverIp, port);
                _username = username;
                _isConnected = true;

                _ = Task.Run(ReadMessagesAsync, _cancellationTokenSource.Token);

                var joinMessage = new Message(username, "[JOIN]");
                await SendMessageAsync(joinMessage);

                return true;
            }
            catch
            {
                _client?.Close();
                _client = null;
                _isConnected = false;
                return false;
            }
        }

        // отправка сообщения
        public async Task SendMessageAsync(Message message)
        {
            if (!_isConnected || _client == null || message == null)
                return;

            if (message.Author != _username)
                message.Author = _username;

            try
            {
                var json = JsonSerializer.Serialize(message);
                var data = Encoding.UTF8.GetBytes(json + Environment.NewLine);

                var stream = _client.GetStream();
                await stream.WriteAsync(data, 0, data.Length);
            }
            catch
            {
                Disconnect();
            }
        }

        // отключение
        public void Disconnect()
        {
            if (!_isConnected) return;

            _cancellationTokenSource.Cancel();
            _client?.Close();
            _client = null;
            _isConnected = false;

            Disconnected?.Invoke();
        }

        // чтение входящих сообщений
        private async Task ReadMessagesAsync()
        {
            try
            {
                if (_client == null) return;

                using var stream = _client.GetStream();
                using var reader = new StreamReader(stream, Encoding.UTF8);

                while (!_cancellationTokenSource.Token.IsCancellationRequested && _client.Connected)
                {
                    string? line = await reader.ReadLineAsync();
                    if (string.IsNullOrEmpty(line)) break;

                    var message = JsonSerializer.Deserialize<Message>(line);
                    if (message != null)
                    {
                        MessageReceived?.Invoke(message);
                    }
                }
            }
            catch
            {
            }
            finally
            {
                if (_isConnected)
                {
                    Disconnect();
                }
            }
        }
    }
}