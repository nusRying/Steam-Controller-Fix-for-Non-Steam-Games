namespace SteamControllerBridge.UI
{
    partial class SettingsForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.CheckBox chkStartOnLogin;
        private System.Windows.Forms.CheckBox chkAutoRefresh;
        private System.Windows.Forms.Label lblInterval;
        private System.Windows.Forms.NumericUpDown numInterval;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;

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
            this.chkStartOnLogin = new System.Windows.Forms.CheckBox();
            this.chkAutoRefresh = new System.Windows.Forms.CheckBox();
            this.lblInterval = new System.Windows.Forms.Label();
            this.numInterval = new System.Windows.Forms.NumericUpDown();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.numInterval)).BeginInit();
            this.SuspendLayout();
            // 
            // chkStartOnLogin
            // 
            this.chkStartOnLogin.Location = new System.Drawing.Point(12, 12);
            this.chkStartOnLogin.Name = "chkStartOnLogin";
            this.chkStartOnLogin.Size = new System.Drawing.Size(200, 24);
            this.chkStartOnLogin.TabIndex = 0;
            this.chkStartOnLogin.Text = "Start on login";
            this.chkStartOnLogin.UseVisualStyleBackColor = true;
            // 
            // chkAutoRefresh
            // 
            this.chkAutoRefresh.Location = new System.Drawing.Point(12, 42);
            this.chkAutoRefresh.Name = "chkAutoRefresh";
            this.chkAutoRefresh.Size = new System.Drawing.Size(200, 24);
            this.chkAutoRefresh.TabIndex = 1;
            this.chkAutoRefresh.Text = "Auto-refresh devices";
            this.chkAutoRefresh.UseVisualStyleBackColor = true;
            // 
            // lblInterval
            // 
            this.lblInterval.Location = new System.Drawing.Point(12, 74);
            this.lblInterval.Name = "lblInterval";
            this.lblInterval.Size = new System.Drawing.Size(140, 20);
            this.lblInterval.Text = "Refresh interval (seconds):";
            // 
            // numInterval
            // 
            this.numInterval.Location = new System.Drawing.Point(158, 72);
            this.numInterval.Minimum = 1;
            this.numInterval.Maximum = 3600;
            this.numInterval.Value = 5;
            this.numInterval.Name = "numInterval";
            this.numInterval.Size = new System.Drawing.Size(80, 22);
            this.numInterval.TabIndex = 2;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(316, 110);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 3;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(397, 110);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // SettingsForm
            // 
            this.ClientSize = new System.Drawing.Size(484, 145);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.numInterval);
            this.Controls.Add(this.lblInterval);
            this.Controls.Add(this.chkAutoRefresh);
            this.Controls.Add(this.chkStartOnLogin);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Settings";
            ((System.ComponentModel.ISupportInitialize)(this.numInterval)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
