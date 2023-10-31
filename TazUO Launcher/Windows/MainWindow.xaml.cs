using System;
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

            if (!Utility.Utility.FindTazUO())
            {
                UpdateManager.Instance.DownloadTUO((p) => 
                {
                    Console.WriteLine(p.ToString());
                    if(p > 0)
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
            }

            InitializeComponent();

            if (!getProfiles.IsCompleted) //This should be extremely fast
            {
                getProfiles.Wait();
            }

            allProfiles = getProfiles.Result;

            foreach (Profile profile in allProfiles)
            {
                ProfileSelector.Items.Add(new ComboBoxItem() { Content = profile.Name, Foreground = new SolidColorBrush(Color.FromRgb(20, 20, 20)) }) ;
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

                    if (ProfileManager.TryFindProfile(((ComboBoxItem)ProfileSelector.SelectedItem).Content.ToString(), out Profile? profile)                    )
                    {
                        try
                        {
                            System.Diagnostics.Process.Start(tuoExecutable, $"-settings \"{profile.GetSettingsFilePath()}\"");
                        } catch(Exception ex)
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
