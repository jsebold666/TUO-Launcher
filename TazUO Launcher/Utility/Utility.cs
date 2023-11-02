using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Threading;

namespace TazUO_Launcher.Utility
{
    class Utility
    {
        public static Dispatcher UIDispatcher = Application.Current.Dispatcher;

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

        public static string AskForFile(string intialDirectory, string fileFilter)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                InitialDirectory = intialDirectory,
                Filter = fileFilter,
                CheckFileExists = true,
                CheckPathExists = true
            };

            var result = openFileDialog.ShowDialog();
            if (result == true)
            {
                return openFileDialog.FileName;
            }
            else
            {
                return "";
            }
        }
    }
}
