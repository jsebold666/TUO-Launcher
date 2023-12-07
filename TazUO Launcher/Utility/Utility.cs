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
                return string.Empty;
            }
        }

        public static string AskForFolder()
        {
            Ookii.Dialogs.Wpf.VistaFolderBrowserDialog folderBrowserDialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();

            if (folderBrowserDialog.ShowDialog() == true)
            {
                return folderBrowserDialog.SelectedPath;
            } 
            else
            {
                return string.Empty;
            }
        }

        public static void OpenLauncherDownloadLink()
        {
            var destinationurl = "https://github.com/bittiez/TUO-Launcher/releases/latest";
            var sInfo = new System.Diagnostics.ProcessStartInfo(destinationurl)
            {
                UseShellExecute = true,
            };
            System.Diagnostics.Process.Start(sInfo);
        }

        public static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs = true)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, true);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }
    }
}
