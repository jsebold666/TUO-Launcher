using System.IO;

namespace TazUO_Launcher.Utility
{
    class Utility
    {
        public static bool FindTazUO()
        {
            string tuoPath = Path.Combine(LauncherSettings.LauncherPath, "TazUO", "ClassicUO.exe");

            if(File.Exists(tuoPath))
            {
                return true;
            }

            return false;
        }

        public static string GetTazUOExecutable()
        {
            return Path.Combine(LauncherSettings.LauncherPath, "TazUO", "ClassicUO.exe");
        }
    }
}
