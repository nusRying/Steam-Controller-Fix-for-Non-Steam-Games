using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace SteamControllerBridge.UI
{
    public partial class MainForm : Form
    {
        private Process? bridgeProcess;

        public MainForm()
        {
            InitializeComponent();
            UpdateUiState();
        }

        private void StartBridge()
        {
            if (bridgeProcess != null && !bridgeProcess.HasExited)
                return;

            string exe = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "SteamControllerBridge.exe");
            exe = Path.GetFullPath(exe);

            if (!File.Exists(exe))
            {
                MessageBox.Show("Bridge executable not found: " + exe, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var startInfo = new ProcessStartInfo(exe, "run") { UseShellExecute = false, CreateNoWindow = true };
            bridgeProcess = Process.Start(startInfo);
            UpdateUiState();
        }

        private void StopBridge()
        {
            try
            {
                if (bridgeProcess != null && !bridgeProcess.HasExited)
                {
                    bridgeProcess.Kill(true);
                    bridgeProcess.WaitForExit(2000);
                }
            }
            catch
            {
                // ignore
            }

            bridgeProcess = null;
            UpdateUiState();
        }

        private void UpdateUiState()
        {
            bool running = bridgeProcess != null && !bridgeProcess.HasExited;
            startToolStripMenuItem.Enabled = !running;
            stopToolStripMenuItem.Enabled = running;
            statusLabel.Text = running ? "Running" : "Stopped";
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StopBridge();
            notifyIcon.Visible = false;
            Application.Exit();
        }

        private void startToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StartBridge();
        }

        private void stopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StopBridge();
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using var f = new SettingsForm();
            f.ShowDialog(this);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            Hide();
        }

        private void notifyIcon_DoubleClick(object sender, EventArgs e)
        {
            // open settings
            settingsToolStripMenuItem_Click(sender, e);
        }
    }
}
