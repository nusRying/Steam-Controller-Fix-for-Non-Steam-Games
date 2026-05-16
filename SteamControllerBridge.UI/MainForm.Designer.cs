namespace SteamControllerBridge.UI
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.NotifyIcon notifyIcon;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem startToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem stopToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.Label statusLabel;
        private System.Windows.Forms.Button startButton;
        private System.Windows.Forms.Button stopButton;
        private System.Windows.Forms.Button showLogsButton;
        private System.Windows.Forms.Panel statusPanel;
        private System.Windows.Forms.TextBox logTextBox;
        private System.Windows.Forms.ListBox devicesListBox;
        private System.Windows.Forms.Button refreshButton;
        private System.Windows.Forms.CheckBox autoRefreshCheckBox;
        private System.Windows.Forms.NumericUpDown intervalUpDown;
        private System.Windows.Forms.Timer autoRefreshTimer;
        private System.Windows.Forms.GroupBox deviceDetailsGroup;
        private System.Windows.Forms.Label lblInstance;
        private System.Windows.Forms.TextBox txtInstance;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label lblVendor;
        private System.Windows.Forms.TextBox txtVendor;
        private System.Windows.Forms.Label lblProduct;
        private System.Windows.Forms.TextBox txtProduct;
        private System.Windows.Forms.Label lblTouchpads;
        private System.Windows.Forms.TextBox txtTouchpads;
        private System.Windows.Forms.Button copyDeviceButton;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.startToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusLabel = new System.Windows.Forms.Label();
            this.startButton = new System.Windows.Forms.Button();
            this.stopButton = new System.Windows.Forms.Button();
            this.showLogsButton = new System.Windows.Forms.Button();
            this.statusPanel = new System.Windows.Forms.Panel();
            this.logTextBox = new System.Windows.Forms.TextBox();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // notifyIcon
            // 
            this.notifyIcon.Text = "Steam Controller Bridge";
            this.notifyIcon.Visible = true;
            this.notifyIcon.ContextMenuStrip = this.contextMenuStrip1;
            this.notifyIcon.DoubleClick += new System.EventHandler(this.notifyIcon_DoubleClick);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.startToolStripMenuItem,
            this.stopToolStripMenuItem,
            this.settingsToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(181, 114);
            // 
            // startToolStripMenuItem
            // 
            this.startToolStripMenuItem.Name = "startToolStripMenuItem";
            this.startToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.startToolStripMenuItem.Text = "Start";
            this.startToolStripMenuItem.Click += new System.EventHandler(this.startToolStripMenuItem_Click);
            // 
            // stopToolStripMenuItem
            // 
            this.stopToolStripMenuItem.Name = "stopToolStripMenuItem";
            this.stopToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.stopToolStripMenuItem.Text = "Stop";
            this.stopToolStripMenuItem.Click += new System.EventHandler(this.stopToolStripMenuItem_Click);
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.settingsToolStripMenuItem.Text = "Settings";
            this.settingsToolStripMenuItem.Click += new System.EventHandler(this.settingsToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // statusLabel
            // 
            this.statusLabel.AutoSize = true;
            this.statusLabel.Location = new System.Drawing.Point(48, 12);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(37, 13);
            this.statusLabel.TabIndex = 0;
            this.statusLabel.Text = "Stopped";
            // 
            // statusPanel
            // 
            this.statusPanel.Location = new System.Drawing.Point(12, 8);
            this.statusPanel.Name = "statusPanel";
            this.statusPanel.Size = new System.Drawing.Size(28, 20);
            this.statusPanel.TabIndex = 1;
            this.statusPanel.BackColor = System.Drawing.Color.Red;
            // 
            // startButton
            // 
            this.startButton.Location = new System.Drawing.Point(12, 36);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(60, 24);
            this.startButton.TabIndex = 2;
            this.startButton.Text = "Start";
            this.startButton.UseVisualStyleBackColor = true;
            this.startButton.Click += new System.EventHandler(this.startButton_Click);
            // 
            // stopButton
            // 
            this.stopButton.Location = new System.Drawing.Point(78, 36);
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(60, 24);
            this.stopButton.TabIndex = 3;
            this.stopButton.Text = "Stop";
            this.stopButton.UseVisualStyleBackColor = true;
            this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
            // 
            // showLogsButton
            // 
            this.showLogsButton.Location = new System.Drawing.Point(144, 36);
            this.showLogsButton.Name = "showLogsButton";
            this.showLogsButton.Size = new System.Drawing.Size(80, 24);
            this.showLogsButton.TabIndex = 4;
            this.showLogsButton.Text = "Show logs";
            this.showLogsButton.UseVisualStyleBackColor = true;
            this.showLogsButton.Click += new System.EventHandler(this.showLogsButton_Click);
            // 
            // logTextBox
            // 
            this.logTextBox.Location = new System.Drawing.Point(12, 66);
            this.logTextBox.Multiline = true;
            this.logTextBox.Name = "logTextBox";
            this.logTextBox.ReadOnly = true;
            this.logTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.logTextBox.Size = new System.Drawing.Size(400, 180);
            this.logTextBox.TabIndex = 5;
            this.logTextBox.Visible = false;
            // 
            // devicesListBox
            // 
            this.devicesListBox = new System.Windows.Forms.ListBox();
            this.devicesListBox.Location = new System.Drawing.Point(420, 12);
            this.devicesListBox.Name = "devicesListBox";
            this.devicesListBox.Size = new System.Drawing.Size(260, 200);
            this.devicesListBox.TabIndex = 6;
            this.devicesListBox.SelectedIndexChanged += new System.EventHandler(this.devicesListBox_SelectedIndexChanged);
            // 
            // refreshButton
            // 
            this.refreshButton = new System.Windows.Forms.Button();
            this.refreshButton.Location = new System.Drawing.Point(420, 220);
            this.refreshButton.Name = "refreshButton";
            this.refreshButton.Size = new System.Drawing.Size(80, 24);
            this.refreshButton.TabIndex = 7;
            this.refreshButton.Text = "Refresh";
            this.refreshButton.UseVisualStyleBackColor = true;
            this.refreshButton.Click += new System.EventHandler(this.refreshButton_Click);
            // 
            // autoRefreshCheckBox
            // 
            this.autoRefreshCheckBox = new System.Windows.Forms.CheckBox();
            this.autoRefreshCheckBox.Location = new System.Drawing.Point(508, 220);
            this.autoRefreshCheckBox.Name = "autoRefreshCheckBox";
            this.autoRefreshCheckBox.Size = new System.Drawing.Size(120, 24);
            this.autoRefreshCheckBox.TabIndex = 8;
            this.autoRefreshCheckBox.Text = "Auto-refresh";
            this.autoRefreshCheckBox.UseVisualStyleBackColor = true;
            this.autoRefreshCheckBox.CheckedChanged += new System.EventHandler(this.autoRefreshCheckBox_CheckedChanged);
            // 
            // intervalUpDown
            // 
            this.intervalUpDown = new System.Windows.Forms.NumericUpDown();
            this.intervalUpDown.Location = new System.Drawing.Point(634, 220);
            this.intervalUpDown.Name = "intervalUpDown";
            this.intervalUpDown.Size = new System.Drawing.Size(46, 22);
            this.intervalUpDown.TabIndex = 9;
            this.intervalUpDown.Minimum = 1;
            this.intervalUpDown.Maximum = 3600;
            this.intervalUpDown.Value = 5;
            this.intervalUpDown.ValueChanged += new System.EventHandler(this.intervalUpDown_ValueChanged);
            // 
            // autoRefreshTimer
            // 
            this.autoRefreshTimer = new System.Windows.Forms.Timer();
            this.autoRefreshTimer.Interval = 5000;
            this.autoRefreshTimer.Tick += new System.EventHandler(this.autoRefreshTimer_Tick);
            // 
            // deviceDetailsGroup
            // 
            this.deviceDetailsGroup = new System.Windows.Forms.GroupBox();
            this.deviceDetailsGroup.Location = new System.Drawing.Point(12, 252);
            this.deviceDetailsGroup.Name = "deviceDetailsGroup";
            this.deviceDetailsGroup.Size = new System.Drawing.Size(668, 100);
            this.deviceDetailsGroup.TabIndex = 10;
            this.deviceDetailsGroup.TabStop = false;
            this.deviceDetailsGroup.Text = "Selected Device";
            // 
            // lblInstance
            // 
            this.lblInstance = new System.Windows.Forms.Label();
            this.lblInstance.Location = new System.Drawing.Point(8, 22);
            this.lblInstance.Name = "lblInstance";
            this.lblInstance.Size = new System.Drawing.Size(60, 20);
            this.lblInstance.Text = "Instance:";
            // 
            // txtInstance
            // 
            this.txtInstance = new System.Windows.Forms.TextBox();
            this.txtInstance.Location = new System.Drawing.Point(70, 20);
            this.txtInstance.Name = "txtInstance";
            this.txtInstance.ReadOnly = true;
            this.txtInstance.Size = new System.Drawing.Size(120, 22);
            // 
            // lblName
            // 
            this.lblName = new System.Windows.Forms.Label();
            this.lblName.Location = new System.Drawing.Point(200, 22);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(40, 20);
            this.lblName.Text = "Name:";
            // 
            // txtName
            // 
            this.txtName = new System.Windows.Forms.TextBox();
            this.txtName.Location = new System.Drawing.Point(240, 20);
            this.txtName.Name = "txtName";
            this.txtName.ReadOnly = true;
            this.txtName.Size = new System.Drawing.Size(300, 22);
            // 
            // lblVendor
            // 
            this.lblVendor = new System.Windows.Forms.Label();
            this.lblVendor.Location = new System.Drawing.Point(8, 52);
            this.lblVendor.Name = "lblVendor";
            this.lblVendor.Size = new System.Drawing.Size(50, 20);
            this.lblVendor.Text = "Vendor:";
            // 
            // txtVendor
            // 
            this.txtVendor = new System.Windows.Forms.TextBox();
            this.txtVendor.Location = new System.Drawing.Point(70, 50);
            this.txtVendor.Name = "txtVendor";
            this.txtVendor.ReadOnly = true;
            this.txtVendor.Size = new System.Drawing.Size(120, 22);
            // 
            // lblProduct
            // 
            this.lblProduct = new System.Windows.Forms.Label();
            this.lblProduct.Location = new System.Drawing.Point(200, 52);
            this.lblProduct.Name = "lblProduct";
            this.lblProduct.Size = new System.Drawing.Size(50, 20);
            this.lblProduct.Text = "Product:";
            // 
            // txtProduct
            // 
            this.txtProduct = new System.Windows.Forms.TextBox();
            this.txtProduct.Location = new System.Drawing.Point(260, 50);
            this.txtProduct.Name = "txtProduct";
            this.txtProduct.ReadOnly = true;
            this.txtProduct.Size = new System.Drawing.Size(120, 22);
            // 
            // lblTouchpads
            // 
            this.lblTouchpads = new System.Windows.Forms.Label();
            this.lblTouchpads.Location = new System.Drawing.Point(400, 52);
            this.lblTouchpads.Name = "lblTouchpads";
            this.lblTouchpads.Size = new System.Drawing.Size(70, 20);
            this.lblTouchpads.Text = "Touchpads:";
            // 
            // txtTouchpads
            // 
            this.txtTouchpads = new System.Windows.Forms.TextBox();
            this.txtTouchpads.Location = new System.Drawing.Point(476, 50);
            this.txtTouchpads.Name = "txtTouchpads";
            this.txtTouchpads.ReadOnly = true;
            this.txtTouchpads.Size = new System.Drawing.Size(40, 22);
            // 
            // copyDeviceButton
            // 
            this.copyDeviceButton = new System.Windows.Forms.Button();
            this.copyDeviceButton.Location = new System.Drawing.Point(540, 20);
            this.copyDeviceButton.Name = "copyDeviceButton";
            this.copyDeviceButton.Size = new System.Drawing.Size(120, 24);
            this.copyDeviceButton.TabIndex = 11;
            this.copyDeviceButton.Text = "Copy details";
            this.copyDeviceButton.UseVisualStyleBackColor = true;
            this.copyDeviceButton.Click += new System.EventHandler(this.copyDeviceButton_Click);
            // 
            // add controls to group
            // 
            this.deviceDetailsGroup.Controls.Add(this.lblInstance);
            this.deviceDetailsGroup.Controls.Add(this.txtInstance);
            this.deviceDetailsGroup.Controls.Add(this.lblName);
            this.deviceDetailsGroup.Controls.Add(this.txtName);
            this.deviceDetailsGroup.Controls.Add(this.lblVendor);
            this.deviceDetailsGroup.Controls.Add(this.txtVendor);
            this.deviceDetailsGroup.Controls.Add(this.lblProduct);
            this.deviceDetailsGroup.Controls.Add(this.txtProduct);
            this.deviceDetailsGroup.Controls.Add(this.lblTouchpads);
            this.deviceDetailsGroup.Controls.Add(this.txtTouchpads);
            this.deviceDetailsGroup.Controls.Add(this.copyDeviceButton);
            // 
            // MainForm
            // 
            this.ClientSize = new System.Drawing.Size(700, 380);
            this.Controls.Add(this.deviceDetailsGroup);
            this.Controls.Add(this.logTextBox);
            this.Controls.Add(this.devicesListBox);
            this.Controls.Add(this.refreshButton);
            this.Controls.Add(this.autoRefreshCheckBox);
            this.Controls.Add(this.intervalUpDown);
            this.Controls.Add(this.showLogsButton);
            this.Controls.Add(this.stopButton);
            this.Controls.Add(this.startButton);
            this.Controls.Add(this.statusPanel);
            this.Controls.Add(this.statusLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = true;
            this.Name = "MainForm";
            this.ShowInTaskbar = true;
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
