using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace TazUO_Launcher.Windows
{
    /// <summary>
    /// Interaction logic for ProfileWindow.xaml
    /// </summary>
    public partial class ProfileWindow : Window
    {
        private Profile selectedProfile;

        public ProfileWindow()
        {
            Task<Profile[]> loadProfiles = ProfileManager.GetAllProfiles();

            InitializeComponent();

            if (!loadProfiles.IsCompleted)
            {
                loadProfiles.Wait();
            }

            foreach (Profile profile in loadProfiles.Result)
            {
                ProfileList.Items.Add(new ListBoxItem() { Content = profile.Name });
            }

            ButtonNew.MouseUp += ButtonNew_MouseUp;
            ButtonDelete.MouseUp += ButtonDelete_MouseUp;
            ButtonCopy.MouseUp += ButtonCopy_MouseUp;

            ProfileList.SelectionChanged += ProfileList_SelectionChanged;
        }

        private void SetEntries(Profile profile)
        {
            EntryProfileName.Text = profile.Name;

            EntryAccountName.Text = profile.CUOSettings.Username;
            EntryAccountPass.Text = profile.CUOSettings.Password;
            EntrySavePass.IsChecked = profile.CUOSettings.SaveAccount;

            EntryServerIP.Text = profile.CUOSettings.IP;
            EntryServerPort.Text = profile.CUOSettings.Port.ToString();

            EntryUODirectory.Text = profile.CUOSettings.UltimaOnlineDirectory;
            EntryClientVersion.Text = profile.CUOSettings.ClientVersion;
            EntryEncrypedClient.IsChecked = profile.CUOSettings.Encryption == 0 ? false : true;
        }

        private void ProfileList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(ProfileManager.TryFindProfile(((ListBoxItem)ProfileList.SelectedItem).Content.ToString(), out Profile profile))
            {
                if(profile != null)
                {
                    selectedProfile = profile;
                    SetEntries(profile);
                }
            }
        }

        private void ButtonCopy_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            throw new System.NotImplementedException();
        }

        private void ButtonDelete_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            throw new System.NotImplementedException();
        }

        private void ButtonNew_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            throw new System.NotImplementedException();
        }
    }
}
