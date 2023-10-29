using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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

        public static string LauncherPath { get; set; } = AppDomain.CurrentDomain.BaseDirectory;

        public static string ProfilesPath { get; set; } = Path.Combine(LauncherPath, "Profiles");
    }
}
