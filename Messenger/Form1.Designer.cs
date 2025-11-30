namespace Messenger
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            groupBoxNetwork = new GroupBox();
            btnDisconnect = new Button();
            btnConnect = new Button();
            btnStartServer = new Button();
            label4 = new Label();
            lstUsers = new ListBox();
            label3 = new Label();
            txtUsername = new TextBox();
            txtPort = new TextBox();
            txtServerIp = new TextBox();
            label2 = new Label();
            label1 = new Label();
            txtChatHistory = new TextBox();
            txtMessageInput = new TextBox();
            btnSend = new Button();
            groupBoxNetwork.SuspendLayout();
            SuspendLayout();
            // 
            // groupBoxNetwork
            // 
            groupBoxNetwork.Controls.Add(btnDisconnect);
            groupBoxNetwork.Controls.Add(btnConnect);
            groupBoxNetwork.Controls.Add(btnStartServer);
            groupBoxNetwork.Controls.Add(label4);
            groupBoxNetwork.Controls.Add(lstUsers);
            groupBoxNetwork.Controls.Add(label3);
            groupBoxNetwork.Controls.Add(txtUsername);
            groupBoxNetwork.Controls.Add(txtPort);
            groupBoxNetwork.Controls.Add(txtServerIp);
            groupBoxNetwork.Controls.Add(label2);
            groupBoxNetwork.Controls.Add(label1);
            groupBoxNetwork.Location = new Point(12, 331);
            groupBoxNetwork.Name = "groupBoxNetwork";
            groupBoxNetwork.Size = new Size(730, 289);
            groupBoxNetwork.TabIndex = 0;
            groupBoxNetwork.TabStop = false;
            groupBoxNetwork.Text = "Управление";
            // 
            // btnDisconnect
            // 
            btnDisconnect.BackColor = Color.PeachPuff;
            btnDisconnect.Enabled = false;
            btnDisconnect.FlatStyle = FlatStyle.Flat;
            btnDisconnect.Location = new Point(476, 200);
            btnDisconnect.Name = "btnDisconnect";
            btnDisconnect.Size = new Size(224, 45);
            btnDisconnect.TabIndex = 9;
            btnDisconnect.Text = "Отключиться";
            btnDisconnect.UseVisualStyleBackColor = false;
            // 
            // btnConnect
            // 
            btnConnect.BackColor = Color.PowderBlue;
            btnConnect.FlatStyle = FlatStyle.Flat;
            btnConnect.Location = new Point(476, 126);
            btnConnect.Name = "btnConnect";
            btnConnect.Size = new Size(224, 45);
            btnConnect.TabIndex = 8;
            btnConnect.Text = "Подключиться";
            btnConnect.UseVisualStyleBackColor = false;
            // 
            // btnStartServer
            // 
            btnStartServer.BackColor = Color.PowderBlue;
            btnStartServer.FlatStyle = FlatStyle.Flat;
            btnStartServer.Location = new Point(476, 47);
            btnStartServer.Name = "btnStartServer";
            btnStartServer.Size = new Size(224, 45);
            btnStartServer.TabIndex = 7;
            btnStartServer.Text = "Запустить сервер";
            btnStartServer.UseVisualStyleBackColor = false;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(24, 182);
            label4.Name = "label4";
            label4.Size = new Size(117, 40);
            label4.TabIndex = 6;
            label4.Text = "Подключенные\r\nпользователи:";
            // 
            // lstUsers
            // 
            lstUsers.FormattingEnabled = true;
            lstUsers.Location = new Point(179, 182);
            lstUsers.Name = "lstUsers";
            lstUsers.Size = new Size(250, 84);
            lstUsers.TabIndex = 4;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(24, 138);
            label3.Name = "label3";
            label3.Size = new Size(142, 20);
            label3.TabIndex = 5;
            label3.Text = "Имя пользователя:";
            // 
            // txtUsername
            // 
            txtUsername.Location = new Point(179, 138);
            txtUsername.Name = "txtUsername";
            txtUsername.Size = new Size(250, 27);
            txtUsername.TabIndex = 4;
            // 
            // txtPort
            // 
            txtPort.Location = new Point(179, 91);
            txtPort.Name = "txtPort";
            txtPort.Size = new Size(250, 27);
            txtPort.TabIndex = 3;
            // 
            // txtServerIp
            // 
            txtServerIp.Location = new Point(179, 47);
            txtServerIp.Name = "txtServerIp";
            txtServerIp.Size = new Size(250, 27);
            txtServerIp.TabIndex = 2;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(24, 91);
            label2.Name = "label2";
            label2.Size = new Size(47, 20);
            label2.TabIndex = 1;
            label2.Text = "Порт:";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(24, 47);
            label1.Name = "label1";
            label1.Size = new Size(70, 20);
            label1.TabIndex = 0;
            label1.Text = "IP-адрес:";
            // 
            // txtChatHistory
            // 
            txtChatHistory.BackColor = Color.White;
            txtChatHistory.Dock = DockStyle.Top;
            txtChatHistory.Location = new Point(0, 0);
            txtChatHistory.Multiline = true;
            txtChatHistory.Name = "txtChatHistory";
            txtChatHistory.ReadOnly = true;
            txtChatHistory.ScrollBars = ScrollBars.Vertical;
            txtChatHistory.Size = new Size(982, 236);
            txtChatHistory.TabIndex = 1;
            // 
            // txtMessageInput
            // 
            txtMessageInput.Location = new Point(12, 264);
            txtMessageInput.Name = "txtMessageInput";
            txtMessageInput.Size = new Size(958, 27);
            txtMessageInput.TabIndex = 2;
            // 
            // btnSend
            // 
            btnSend.BackColor = Color.FromArgb(192, 192, 255);
            btnSend.FlatStyle = FlatStyle.Flat;
            btnSend.Location = new Point(769, 306);
            btnSend.Name = "btnSend";
            btnSend.Size = new Size(201, 41);
            btnSend.TabIndex = 3;
            btnSend.Text = "Отправить";
            btnSend.UseVisualStyleBackColor = false;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(982, 632);
            Controls.Add(btnSend);
            Controls.Add(txtMessageInput);
            Controls.Add(txtChatHistory);
            Controls.Add(groupBoxNetwork);
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Мессенджер";
            groupBoxNetwork.ResumeLayout(false);
            groupBoxNetwork.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private GroupBox groupBoxNetwork;
        private TextBox txtChatHistory;
        private TextBox txtMessageInput;
        private Button btnSend;
        private Label label1;
        private TextBox txtUsername;
        private TextBox txtPort;
        private TextBox txtServerIp;
        private Label label2;
        private ListBox lstUsers;
        private Label label3;
        private Button btnStartServer;
        private Label label4;
        private Button btnDisconnect;
        private Button btnConnect;
    }
}
