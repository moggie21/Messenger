namespace Messenger
{
    public partial class Form1 : Form
    {
        private ChatServer? _server;
        private ChatClient? _client;
        private string? _selectedRecipient = null;
        public Form1()
        {
            InitializeComponent();
            txtMessageInput.KeyDown += txtMessageInput_KeyDown;
            lstUsers.DoubleClick += lstUsers_DoubleClick;
        }

        private async void btnStartServer_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtPort.Text, out int port) || port < 1024 || port > 65535)
            {
                MessageBox.Show("Пожалуйста, введите корректный порт (1024–65535).", "Ошибка",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                _server = new ChatServer();

                _server.MessageReceived += OnMessageReceived;
                _server.UsersChanged += (users) =>
                {
                    if (lstUsers.InvokeRequired)
                        lstUsers.Invoke(new Action(() => UpdateUserList(users)));
                    else
                        UpdateUserList(users);
                };

                await _server.StartAsync(port);

                SetUiMode(isServer: true);
                AppendMessage(new Message("Система", $"Сервер запущен на порту {port}"));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось запустить сервер:\n{ex.Message}", "Ошибка",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnConnect_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                MessageBox.Show("Имя пользователя не может быть пустым.", "Ошибка",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(txtPort.Text, out int port) || port < 1024 || port > 65535)
            {
                MessageBox.Show("Пожалуйста, введите корректный порт (1024–65535).", "Ошибка",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string ip = txtServerIp.Text.Trim();
            if (string.IsNullOrWhiteSpace(ip))
            {
                MessageBox.Show("Введите IP-адрес сервера.", "Ошибка",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _client = new ChatClient();
            _client.MessageReceived += OnMessageReceived;
            _client.Disconnected += OnClientDisconnected;

            bool connected = await _client.ConnectAsync(ip, port, txtUsername.Text);

            if (connected)
            {
                SetUiMode(isServer: false);
                AppendMessage(new Message("Система", $"Подключено к {ip}:{port}"));
            }
            else
            {
                MessageBox.Show("Не удалось подключиться к серверу.", "Ошибка",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                _client = null;
            }
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            if (_server != null)
            {
                _server.Stop();
                _server = null;
            }

            if (_client != null)
            {
                _client.Disconnect();
                _client = null;
            }

            SetUiMode(isServer: null);
            AppendMessage(new Message("Система", "Отключено."));
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            SendCurrentMessage();
        }

        private void txtMessageInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SendCurrentMessage();
                e.SuppressKeyPress = true;
            }
        }

        private void SendCurrentMessage()
        {
            string text = txtMessageInput.Text.Trim();
            if (string.IsNullOrEmpty(text)) return;

            if (_client != null && _client.IsConnected)
            {
                var msg = new Message(txtUsername.Text, text, _selectedRecipient);
                _ = _client.SendMessageAsync(msg);
                AppendMessage(msg);
            }
            else if (_server != null && _server.IsRunning)
            {
                var msg = new Message(txtUsername.Text, text, _selectedRecipient);
                OnMessageReceived(msg);
            }
            else
            {
                MessageBox.Show("Подключитесь к серверу или запустите его.", "Ошибка",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            txtMessageInput.Clear();
            txtMessageInput.Focus();
        }

        private void OnMessageReceived(Message msg)
        {
            if (txtChatHistory.InvokeRequired)
                txtChatHistory.Invoke(new Action(() => AppendMessage(msg)));
            else
                AppendMessage(msg);

            if (msg.Author == "Система" && msg.Text.StartsWith("[USERS]"))
            {
                var userListStr = msg.Text.Substring("[USERS]".Length);
                var users = string.IsNullOrEmpty(userListStr)
                    ? new string[0]
                    : userListStr.Split('|');

                if (lstUsers.InvokeRequired)
                    lstUsers.Invoke(new Action(() => UpdateUserListFromServer(users)));
                else
                    UpdateUserListFromServer(users);
                return;
            }

            if (msg.Author == "Система")
            {
                if (msg.Text.Contains("присоединился к чату"))
                {
                    string name = msg.Text.Replace(" присоединился к чату", "");
                    if (lstUsers.InvokeRequired)
                        lstUsers.Invoke(new Action(() => AddUserToList(name)));
                    else
                        AddUserToList(name);
                }
                else if (msg.Text.Contains("покинул чат"))
                {
                    string name = msg.Text.Replace(" покинул чат", "");
                    if (lstUsers.InvokeRequired)
                        lstUsers.Invoke(new Action(() => RemoveUserFromList(name)));
                    else
                        RemoveUserFromList(name);
                }
            }
        }

        private void AppendMessage(Message msg)
        {
            txtChatHistory.AppendText(msg.ToString() + Environment.NewLine);
            txtChatHistory.ScrollToCaret();
        }

        private void AddUserToList(string username)
        {
            if (!lstUsers.Items.Contains(username))
                lstUsers.Items.Add(username);
        }

        private void RemoveUserFromList(string username)
        {
            if (lstUsers.Items.Contains(username))
                lstUsers.Items.Remove(username);
        }

        private void UpdateUserListFromServer(IEnumerable<string> users)
        {
            lstUsers.Items.Clear();
            foreach (var user in users)
            {
                if (!string.IsNullOrEmpty(user))
                    lstUsers.Items.Add(user);
            }
        }

        private void OnClientDisconnected()
        {
            if (this.InvokeRequired)
                this.Invoke(new Action(() => HandleDisconnect()));
            else
                HandleDisconnect();
        }

        private void HandleDisconnect()
        {
            MessageBox.Show("Соединение с сервером потеряно.", "Отключено",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
            btnDisconnect_Click(null, EventArgs.Empty);
        }

        private void UpdateUserList(List<string> users)
        {
            lstUsers.Items.Clear();
            foreach (var user in users)
                lstUsers.Items.Add(user);
        }

        private void SetUiMode(bool? isServer)
        {
            bool connected = (isServer.HasValue);
            txtServerIp.Enabled = !connected;
            txtPort.Enabled = !connected;
            txtUsername.Enabled = !connected;

            btnStartServer.Enabled = !connected;
            btnConnect.Enabled = !connected;
            btnDisconnect.Enabled = connected;

            if (isServer == true)
            {
            }
        }

        private void lstUsers_DoubleClick(object sender, EventArgs e)
        {
            if (lstUsers.SelectedItem is string selectedUser)
            {
                _selectedRecipient = selectedUser;
                txtMessageInput.Focus();
            }
        }
    }
}
