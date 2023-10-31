using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TazUO_Launcher.Utility;
using TazUO_Launcher.Windows;

namespace TazUO_Launcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Profile[] allProfiles;

        public MainWindow()
        {
            Task<Profile[]> getProfiles = ProfileManager.GetAllProfiles();
            bool tuoInstalled = Utility.Utility.FindTazUO();

            if (!tuoInstalled)
            {
                UpdateManager.Instance.DownloadTUO((p) =>
                {
                    Console.WriteLine(p.ToString());
                    if (p > 0)
                    {
                        DownloadProgressBar.Value = p;
                        DownloadProgressBar.Visibility = Visibility.Visible;
                        DownloadProgressLabel.Visibility = Visibility.Visible;
                    }

                    if (p >= 100)
                    {
                        DownloadProgressBar.Visibility = Visibility.Hidden;
                        DownloadProgressLabel.Visibility = Visibility.Hidden;
                    }
                });
            } //Start downloading TUO if it's not installed.

            InitializeComponent();

            UpdateManager.Instance.GetRemoteTazUOVersion(() =>
            {
                if (UpdateManager.Instance.RemoteVersion != null)
                {
                    RemoteVersionText.Content = $"Latest TazUO version: {UpdateManager.Instance.RemoteVersion.ToString(3)}";
                    RemoteVersionText.Visibility = Visibility.Visible;
                }
            });

            if(tuoInstalled)
            {
                Version l = UpdateManager.Instance.GetInstalledTazUOVersion(Utility.Utility.GetTazUOExecutable());
                if (l != null) {
                    LocalVersionText.Content = $"Your TazUO version: {l.ToString(3)}";
                    LocalVersionText.Visibility = Visibility.Visible;
                }
            }

            if (!getProfiles.IsCompleted) //This should be extremely fast
            {
                getProfiles.Wait();
            }

            allProfiles = getProfiles.Result;

            foreach (Profile profile in allProfiles)
            {
                ProfileSelector.Items.Add(new ComboBoxItem() { Content = profile.Name, Foreground = new SolidColorBrush(Color.FromRgb(20, 20, 20)) });
            }

            ProfileSelector.SelectedIndex = LauncherSettings.LastSelectedProfileIndex;

            ProfileSelector.SelectionChanged += (s, e) =>
            {
                LauncherSettings.LastSelectedProfileIndex = ProfileSelector.SelectedIndex;
            };
        }

        private void ProfileSettingsButtonMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ProfileWindow profileWindow = new ProfileWindow();
            profileWindow.Show();
            profileWindow.Closed += (s, e) =>
            {
                Task<Profile[]> getProfiles = ProfileManager.GetAllProfiles();

                if (!getProfiles.IsCompleted) //This should be extremely fast
                {
                    getProfiles.Wait();
                }

                allProfiles = getProfiles.Result;

                ProfileSelector.Items.Clear();

                foreach (Profile profile in allProfiles)
                {
                    ProfileSelector.Items.Add(new ComboBoxItem() { Content = profile.Name, Foreground = new SolidColorBrush(Color.FromRgb(20, 20, 20)) });
                }

                ProfileSelector.SelectedIndex = LauncherSettings.LastSelectedProfileIndex;
            };
        }

        private void PlayButtonMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

            if (Utility.Utility.FindTazUO())
            {
                if (ProfileSelector.SelectedIndex > -1)
                {
                    string tuoExecutable = Utility.Utility.GetTazUOExecutable();

                    if (ProfileManager.TryFindProfile(((ComboBoxItem)ProfileSelector.SelectedItem).Content.ToString(), out Profile? profile))
                    {
                        try
                        {
                            var proc = new ProcessStartInfo(tuoExecutable, $"-settings \"{profile.GetSettingsFilePath()}\"");
                            Process.Start(proc);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                        }
                    }
                }
            }
            else
            {
                //Do update stuff here
            }

        }

        private void DiscordIconMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var destinationurl = "https://discord.gg/SqwtB5g95H";
            var sInfo = new System.Diagnostics.ProcessStartInfo(destinationurl)
            {
                UseShellExecute = true,
            };
            System.Diagnostics.Process.Start(sInfo);
        }

        private void GithubIconMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var destinationurl = "https://github.com/bittiez/TazUO";
            var sInfo = new System.Diagnostics.ProcessStartInfo(destinationurl)
            {
                UseShellExecute = true,
            };
            System.Diagnostics.Process.Start(sInfo);
        }

        private void WikiMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var destinationurl = "https://github.com/bittiez/TazUO/wiki";
            var sInfo = new System.Diagnostics.ProcessStartInfo(destinationurl)
            {
                UseShellExecute = true,
            };
            System.Diagnostics.Process.Start(sInfo);
        }
    }
}
