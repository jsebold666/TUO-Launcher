using Microsoft.Win32;
using System;
using System.IO;
using System.Reflection;

namespace TazUO_Launcher
{
    class LauncherSettings
    {
        public static Version LauncherVersion
        {
            get
            {
                Version version = Assembly.GetExecutingAssembly().GetName().Version;
                return version == null ? new Version(1, 0, 0) : version;
            }
        }

        public static string LauncherVersionString => "Launcher Version " + LauncherVersion.ToString(3);

        public static string LauncherPath { get; set; } = AppDomain.CurrentDomain.BaseDirectory;

        public static string ProfilesPath { get; set; } = Path.Combine(LauncherPath, "Profiles");

        public static string SettingsPath { get; set; } = Path.Combine(ProfilesPath, "Settings");

        public static string TazUOPath { get; set; } = Path.Combine(LauncherPath, "TazUO");

        public static int LastSelectedProfileIndex
        {
            get
            {
                return lastSelectedProfileIndex;
            }
            set
            {
                lastSelectedProfileIndex = value;
                SaveKey("lastSelectedIndex", value.ToString());
            }
        }

        private static int lastSelectedProfileIndex = ParseInt(GetKey("lastSelectedIndex"));

        private static int ParseInt(string val)
        {
            if (int.TryParse(val, out int v))
            {
                return v;
            }

            return 0;
        }

        private static void SaveKey(string keyName, string value)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey("Software", true);
            if (key != null)
            {
                key = key.CreateSubKey("TazUOLauncher");
                key?.SetValue(keyName, value);
            }
        }

        private static string GetKey(string keyName)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey("Software", false);
            if (key != null)
            {
                key = key.OpenSubKey("TazUOLauncher");
                if (key != null)
                {
                    var result = key?.GetValue(keyName)?.ToString();

                    return result == null ? string.Empty : result;
                }
                return string.Empty;
            }
            return string.Empty;
        }
    }
}
