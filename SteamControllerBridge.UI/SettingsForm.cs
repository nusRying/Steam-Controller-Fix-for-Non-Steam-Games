using System;
using System.IO;
using System.Windows.Forms;

namespace SteamControllerBridge.UI
{
    public partial class SettingsForm : Form
    {
        private readonly string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "bridge-settings.json");

        private readonly SettingsStore store = new SettingsStore();

        public SettingsForm()
        {
            InitializeComponent();
            LoadSettings();
        }

        private void LoadSettings()
        {
            var s = store.Load();
            chkStartOnLogin.Checked = s.StartOnLogin;
            chkAutoRefresh.Checked = s.AutoRefresh;
            numInterval.Value = Math.Max(numInterval.Minimum, Math.Min(numInterval.Maximum, s.AutoRefreshIntervalSeconds));
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                var s = new Settings { StartOnLogin = chkStartOnLogin.Checked, AutoRefresh = chkAutoRefresh.Checked, AutoRefreshIntervalSeconds = (int)numInterval.Value };
                store.Save(s);

                // Manage startup registry
                try
                {
                    string exePath = Environment.ProcessPath ?? throw new InvalidOperationException("Could not resolve current executable path.");
                    string quoted = exePath.Contains(' ') ? $"\"{exePath}\"" : exePath;
                    if (s.StartOnLogin)
                    {
                        Microsoft.Win32.Registry.CurrentUser.CreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run")?.SetValue("SteamControllerBridge", quoted + " run", Microsoft.Win32.RegistryValueKind.String);
                    }
                    else
                    {
                        using var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run", writable: true);
                        key?.DeleteValue("SteamControllerBridge", throwOnMissingValue: false);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Warning: could not update startup setting: " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                MessageBox.Show("Settings saved.", "Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to save settings: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
