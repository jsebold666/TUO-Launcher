using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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
        }

        private void ProfileSettingsButtonMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ProfileWindow profileWindow = new ProfileWindow();
            profileWindow.Show();
        }
    }
}
