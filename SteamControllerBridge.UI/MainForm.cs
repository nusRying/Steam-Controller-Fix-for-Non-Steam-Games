using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace SteamControllerBridge.UI
{
    public partial class MainForm : Form
    {
        private Process? bridgeProcess;
        private bool logsVisible = true;

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

            var startInfo = new ProcessStartInfo(exe, "run")
            {
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            bridgeProcess = Process.Start(startInfo);
            if (bridgeProcess != null)
            {
                bridgeProcess.OutputDataReceived += (s, e) => { if (e.Data != null) AppendLog(e.Data); };
                bridgeProcess.ErrorDataReceived += (s, e) => { if (e.Data != null) AppendLog("ERR: " + e.Data); };
                try { bridgeProcess.BeginOutputReadLine(); bridgeProcess.BeginErrorReadLine(); } catch { }
            }
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
            startButton.Enabled = !running;
            stopButton.Enabled = running;
            statusLabel.Text = running ? "Running" : "Stopped";
            statusPanel.BackColor = running ? System.Drawing.Color.LimeGreen : System.Drawing.Color.Red;
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
            // toggle main window
            if (this.Visible && this.WindowState != FormWindowState.Minimized)
            {
                Hide();
            }
            else
            {
                Show();
                this.WindowState = FormWindowState.Normal;
                this.BringToFront();
            }
        }

        private void AppendLog(string line)
        {
            if (string.IsNullOrEmpty(line)) return;
            if (logTextBox.InvokeRequired)
            {
                logTextBox.Invoke(new Action<string>(AppendLog), line);
                return;
            }
            logTextBox.AppendText(line + Environment.NewLine);
            // keep the latest visible
            logTextBox.SelectionStart = logTextBox.Text.Length;
            logTextBox.ScrollToCaret();
        }

        private void startButton_Click(object? sender, EventArgs e)
        {
            StartBridge();
        }

        private void stopButton_Click(object? sender, EventArgs e)
        {
            StopBridge();
        }

        private void showLogsButton_Click(object? sender, EventArgs e)
        {
            logsVisible = !logsVisible;
            logTextBox.Visible = logsVisible;
            showLogsButton.Text = logsVisible ? "Logs" : "Show";
        }
    }
}
