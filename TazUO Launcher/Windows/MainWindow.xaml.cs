using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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

            InitializeComponent();

            if (!getProfiles.IsCompleted) //This should be extremely fast
            {
                getProfiles.Wait();
            }

            allProfiles = getProfiles.Result;

            foreach (Profile profile in allProfiles)
            {
                ProfileSelector.Items.Add(new ComboBoxItem() { Content = profile.Name });
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
                    ProfileSelector.Items.Add(new ComboBoxItem() { Content = profile.Name });
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
    }
}
