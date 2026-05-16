using System;
using System.IO;
using System.Text.Json;

namespace SteamControllerBridge.UI
{
    internal sealed class SettingsStore
    {
        private readonly string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "bridge-settings.json");

        public SettingsStore()
        {
        }

        public Settings Load()
        {
            try
            {
                if (!File.Exists(path)) return Settings.Default;
                string txt = File.ReadAllText(path);
                var s = JsonSerializer.Deserialize<Settings>(txt);
                return s ?? Settings.Default;
            }
            catch
            {
                return Settings.Default;
            }
        }

        public void Save(Settings s)
        {
            try
            {
                var opts = new JsonSerializerOptions { WriteIndented = true };
                string txt = JsonSerializer.Serialize(s, opts);
                File.WriteAllText(path, txt);
            }
            catch
            {
                // ignore
            }
        }
    }

    internal sealed class Settings
    {
        public bool StartOnLogin { get; set; }
        public bool AutoRefresh { get; set; }
        public int AutoRefreshIntervalSeconds { get; set; }

        public static Settings Default => new Settings { StartOnLogin = false, AutoRefresh = false, AutoRefreshIntervalSeconds = 5 };
    }
}
