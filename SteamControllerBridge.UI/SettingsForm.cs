using System;
using System.IO;
using System.Windows.Forms;

namespace SteamControllerBridge.UI
{
    public partial class SettingsForm : Form
    {
        private readonly string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "bridge-settings.json");

        public SettingsForm()
        {
            InitializeComponent();
            LoadSettings();
        }

        private void LoadSettings()
        {
            if (File.Exists(configPath))
            {
                txtConfig.Text = File.ReadAllText(configPath);
            }
            else
            {
                txtConfig.Text = "{\n  \"startOnLogin\": false\n}";
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                File.WriteAllText(configPath, txtConfig.Text);
                MessageBox.Show("Settings saved.", "Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to save settings: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
