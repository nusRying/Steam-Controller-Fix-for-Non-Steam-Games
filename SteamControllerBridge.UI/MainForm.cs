using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace SteamControllerBridge.UI
{
    public partial class MainForm : Form
    {
        private Process? bridgeProcess;
        private bool logsVisible = false;
        private readonly HashSet<string> connectedDevices = new();
        private readonly Regex statusLineRegex = new(@"^(?<id>\d+): (?<name>.+?) \[vendor=0x(?<vendor>[0-9A-Fa-f]+), product=0x(?<product>[0-9A-Fa-f]+), touchpads=(?<touchpads>\d+)\]", RegexOptions.Compiled);

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

        private async void RefreshDeviceListFromStatus()
        {
            try
            {
                string exe = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "SteamControllerBridge.exe");
                exe = Path.GetFullPath(exe);
                if (!File.Exists(exe))
                {
                    AppendLog("Status: bridge executable not found for status check: " + exe);
                    return;
                }

                var psi = new ProcessStartInfo(exe, "status") { UseShellExecute = false, RedirectStandardOutput = true, RedirectStandardError = true, CreateNoWindow = true };
                using var p = Process.Start(psi);
                if (p == null) return;
                string outStr = await p.StandardOutput.ReadToEndAsync();
                string errStr = await p.StandardError.ReadToEndAsync();
                if (!string.IsNullOrEmpty(outStr))
                {
                    // clear and repopulate from status output
                    connectedDevices.Clear();
                    foreach (string line in outStr.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        AppendLog(line);
                        var m = statusLineRegex.Match(line);
                        if (m.Success)
                        {
                            string id = m.Groups["id"].Value;
                            string name = m.Groups["name"].Value;
                            string vendor = m.Groups["vendor"].Value;
                            string product = m.Groups["product"].Value;
                            string touchpads = m.Groups["touchpads"].Value;
                            string display = $"{id}: {name} [vendor=0x{vendor}, product=0x{product}, touchpads={touchpads}]";
                            connectedDevices.Add(display);
                        }
                    }
                    UpdateDeviceListBox();
                }
                if (!string.IsNullOrEmpty(errStr))
                {
                    foreach (string line in errStr.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
                        AppendLog("ERR: " + line);
                }
            }
            catch (Exception ex)
            {
                AppendLog("ERR: " + ex.Message);
            }
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
            // Load persisted settings
            try
            {
                var store = new SettingsStore();
                var s = store.Load();
                autoRefreshCheckBox.Checked = s.AutoRefresh;
                intervalUpDown.Value = Math.Max(intervalUpDown.Minimum, Math.Min(intervalUpDown.Maximum, s.AutoRefreshIntervalSeconds));
                autoRefreshTimer.Interval = (int)intervalUpDown.Value * 1000;
                autoRefreshTimer.Enabled = s.AutoRefresh;
            }
            catch
            {
                // ignore
            }

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
            // Parse connection events
            if (line.StartsWith("Connected:", StringComparison.OrdinalIgnoreCase))
            {
                string name = line.Substring("Connected:".Length).Trim();
                if (!connectedDevices.Contains(name))
                {
                    connectedDevices.Add(name);
                    UpdateDeviceListBox();
                }
            }
            else if (line.StartsWith("Disconnected:", StringComparison.OrdinalIgnoreCase))
            {
                string rest = line.Substring("Disconnected:".Length).Trim();
                // The program logs "Disconnected: <instanceId>" — remove any matching by instance id or name
                // Try to remove any device that starts with the id or contains the token
                if (!string.IsNullOrEmpty(rest))
                {
                    var toRemove = connectedDevices.Where(d => d.Contains(rest, StringComparison.OrdinalIgnoreCase)).ToArray();
                    foreach (var r in toRemove) connectedDevices.Remove(r);
                    UpdateDeviceListBox();
                }
            }
            // keep the latest visible
            logTextBox.SelectionStart = logTextBox.Text.Length;
            logTextBox.ScrollToCaret();
        }

        private void UpdateDeviceListBox()
        {
            if (devicesListBox.InvokeRequired)
            {
                devicesListBox.Invoke(new Action(UpdateDeviceListBox));
                return;
            }
            devicesListBox.Items.Clear();
            foreach (var d in connectedDevices)
                devicesListBox.Items.Add(d);
        }

        private void devicesListBox_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (devicesListBox.SelectedItem == null)
            {
                ClearDeviceDetails();
                return;
            }

            string selected = devicesListBox.SelectedItem.ToString() ?? string.Empty;
            // expected format: "{id}: {name} [vendor=0x{vendor}, product=0x{product}, touchpads={touchpads}]"
            var m = statusLineRegex.Match(selected);
            if (m.Success)
            {
                txtInstance.Text = m.Groups["id"].Value;
                txtName.Text = m.Groups["name"].Value;
                txtVendor.Text = "0x" + m.Groups["vendor"].Value;
                txtProduct.Text = "0x" + m.Groups["product"].Value;
                txtTouchpads.Text = m.Groups["touchpads"].Value;
            }
            else
            {
                // fallback: set full text into Name
                txtInstance.Text = string.Empty;
                txtName.Text = selected;
                txtVendor.Text = string.Empty;
                txtProduct.Text = string.Empty;
                txtTouchpads.Text = string.Empty;
            }
        }

        private void ClearDeviceDetails()
        {
            txtInstance.Text = string.Empty;
            txtName.Text = string.Empty;
            txtVendor.Text = string.Empty;
            txtProduct.Text = string.Empty;
            txtTouchpads.Text = string.Empty;
        }

        private void copyDeviceButton_Click(object? sender, EventArgs e)
        {
            var parts = new List<string>();
            if (!string.IsNullOrEmpty(txtInstance.Text)) parts.Add($"Instance: {txtInstance.Text}");
            if (!string.IsNullOrEmpty(txtName.Text)) parts.Add($"Name: {txtName.Text}");
            if (!string.IsNullOrEmpty(txtVendor.Text)) parts.Add($"Vendor: {txtVendor.Text}");
            if (!string.IsNullOrEmpty(txtProduct.Text)) parts.Add($"Product: {txtProduct.Text}");
            if (!string.IsNullOrEmpty(txtTouchpads.Text)) parts.Add($"Touchpads: {txtTouchpads.Text}");
            string text = string.Join("\t", parts);
            try
            {
                Clipboard.SetText(text);
            }
            catch
            {
                AppendLog("ERR: Could not copy to clipboard");
            }
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

        private void refreshButton_Click(object? sender, EventArgs e)
        {
            RefreshDeviceListFromStatus();
        }

        private void autoRefreshCheckBox_CheckedChanged(object? sender, EventArgs e)
        {
            bool enabled = autoRefreshCheckBox.Checked;
            autoRefreshTimer.Enabled = enabled;
            if (enabled)
            {
                autoRefreshTimer.Interval = (int)intervalUpDown.Value * 1000;
            }
            SaveSettings();
        }

        private void intervalUpDown_ValueChanged(object? sender, EventArgs e)
        {
            autoRefreshTimer.Interval = (int)intervalUpDown.Value * 1000;
            SaveSettings();
        }

        private void autoRefreshTimer_Tick(object? sender, EventArgs e)
        {
            RefreshDeviceListFromStatus();
        }

        private void SaveSettings()
        {
            try
            {
                var store = new SettingsStore();
                var s = store.Load();
                s.AutoRefresh = autoRefreshCheckBox.Checked;
                s.AutoRefreshIntervalSeconds = (int)intervalUpDown.Value;
                store.Save(s);
            }
            catch (Exception ex)
            {
                AppendLog("ERR: Failed to save settings: " + ex.Message);
            }
        }
    }
}
