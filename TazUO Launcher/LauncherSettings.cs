using System;
using System.Collections.Generic;
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
    }
}
