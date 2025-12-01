using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Messenger
{
    public class ChatServer
    {
        private TcpListener? _listener;
        private readonly List<TcpClient> _clients = new();
        private readonly Dictionary<TcpClient, string> _usernames = new();
        private readonly CancellationTokenSource _cancellationTokenSource = new();
        private int _port;

        // пришло новое сообщение
        public event Action<Message>? MessageReceived;

        // список пользователей изменился
        public event Action<List<string>>? UsersChanged;

        // статус сервера
        public bool IsRunning { get; private set; }

        // запуск сервера
        public async Task StartAsync(int port)
        {
            if (IsRunning) return;

            _port = port;
            _listener = new TcpListener(IPAddress.Any, port);
            _listener.Start();
            IsRunning = true;

            _ = Task.Run(AcceptClientsAsync, _cancellationTokenSource.Token);
        }

        // остановка сервера
        public void Stop()
        {
            if (!IsRunning) return;

            _cancellationTokenSource.Cancel();
            _listener?.Stop();

            lock (_clients)
            {
                foreach (var client in _clients.ToList())
                {
                    client?.Close();
                }
                _clients.Clear();
                _usernames.Clear();
            }

            IsRunning = false;
            OnUsersChanged();
        }

        private async Task AcceptClientsAsync()
        {
            try
            {
                while (!_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    var client = await _listener.AcceptTcpClientAsync();
                    lock (_clients)
                    {
                        _clients.Add(client);
                    }
                    _ = Task.Run(() => HandleClientAsync(client), _cancellationTokenSource.Token);
                }
            }
            catch (ObjectDisposedException)
            {
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка сервера: {ex.Message}");
            }
        }

        // обработка одного клиента
        private async Task HandleClientAsync(TcpClient client)
        {
            var stream = client.GetStream();
            var buffer = new byte[1024];
            string username = "unknown";

            try
            {
                using var reader = new StreamReader(stream, Encoding.UTF8);
                using var writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };

                string firstLine = await reader.ReadLineAsync();
                if (string.IsNullOrEmpty(firstLine)) return;

                var message = JsonSerializer.Deserialize<Message>(firstLine);
                if (message == null || string.IsNullOrWhiteSpace(message.Author))
                {
                    var errorResponse = new Message("Система", "Ошибка: имя пользователя не может быть пустым.");
                    var json = JsonSerializer.Serialize(errorResponse);
                    var data = Encoding.UTF8.GetBytes(json + Environment.NewLine);
                    stream.Write(data, 0, data.Length);
                    client.Close();
                    return;
                }

                username = message.Author;

                lock (_clients)
                {
                    _usernames[client] = username;
                }

                var currentUsers = _usernames.Values.ToList();
                var userListMessage = new Message("Система", $"[USERS]{string.Join("|", currentUsers)}");
                var userListJson = JsonSerializer.Serialize(userListMessage) + Environment.NewLine;
                var userListData = Encoding.UTF8.GetBytes(userListJson);

                try
                {
                    client.GetStream().Write(userListData, 0, userListData.Length);
                }
                catch
                {
                }

                var joinMessage = new Message("Система", $"{username} присоединился к чату");
                BroadcastMessage(joinMessage, excludeClient: client);
                OnMessageReceived(joinMessage);

                OnUsersChanged();

                while (!_cancellationTokenSource.Token.IsCancellationRequested && client.Connected)
                {
                    string? line = await reader.ReadLineAsync();
                    if (string.IsNullOrEmpty(line)) break;

                    var msg = JsonSerializer.Deserialize<Message>(line);
                    if (msg?.Author == username)
                    {
                        if (string.IsNullOrEmpty(msg.Recipient))
                        {
                            // публичное сообщение
                            OnMessageReceived(msg);
                            BroadcastMessage(msg, excludeClient: null);
                        }
                        else
                        {
                            // личное сообщение
                            bool delivered = false;
                            lock (_clients)
                            {
                                foreach (var kvp in _usernames)
                                {
                                    if (kvp.Value == msg.Recipient)
                                    {
                                        var json = JsonSerializer.Serialize(msg) + Environment.NewLine;
                                        var data = Encoding.UTF8.GetBytes(json);
                                        try
                                        {
                                            kvp.Key.GetStream().Write(data, 0, data.Length);
                                            delivered = true;
                                        }
                                        catch { }
                                        break;
                                    }
                                }
                            }

                            OnMessageReceived(msg);

                            if (!delivered)
                            {
                                var errorMsg = new Message("Система", $"Пользователь '{msg.Recipient}' не в сети.");
                                try
                                {
                                    var json = JsonSerializer.Serialize(errorMsg) + Environment.NewLine;
                                    var data = Encoding.UTF8.GetBytes(json);
                                    stream.Write(data, 0, data.Length);
                                }
                                catch { }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка обработки клиента: {ex.Message}");
            }
            finally
            {
                lock (_clients)
                {
                    _clients.Remove(client);
                    _usernames.Remove(client);
                }

                var leaveMessage = new Message("Система", $"{username} покинул чат");
                BroadcastMessage(leaveMessage, excludeClient: null);
                OnUsersChanged();

                client?.Close();
            }
        }

        // рассылка сообщения всем клиентам
        private void BroadcastMessage(Message message, TcpClient? excludeClient = null)
        {
            var json = JsonSerializer.Serialize(message);
            var data = Encoding.UTF8.GetBytes(json + Environment.NewLine);

            lock (_clients)
            {
                foreach (var client in _clients.ToList())
                {
                    if (client == excludeClient || !client.Connected) continue;

                    try
                    {
                        client.GetStream().Write(data, 0, data.Length);
                    }
                    catch
                    {
                    }
                }
            }
            if (excludeClient == null) 
            {
                OnMessageReceived(message);
            }
        }

        private void OnMessageReceived(Message message) => MessageReceived?.Invoke(message);
        private void OnUsersChanged()
        {
            var users = new List<string>();
            lock (_clients)
            {
                users.AddRange(_usernames.Values);
            }
            UsersChanged?.Invoke(users);
        }
    }
}