using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using TazUO_Launcher.Utility;

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

            ButtonNew.Click += ButtonNew_MouseUp;
            ButtonDelete.Click += ButtonDelete_MouseUp;
            ButtonCopy.Click += ButtonCopy_MouseUp;

            ProfileList.SelectionChanged += ProfileList_SelectionChanged;

            LocateUOButton.Click += (s, e) =>
            {
                if (selectedProfile != null)
                {

                    string selectDir = AskForFile(EntryUODirectory.Text, "UO Client (*.exe)|*.exe");
                    if (!string.IsNullOrEmpty(selectDir))
                    {
                        EntryUODirectory.Text = Path.GetDirectoryName(selectDir);
                        selectedProfile.CUOSettings.UltimaOnlineDirectory = EntryUODirectory.Text;

                        if (ClientVersionHelper.TryParseFromFile(Path.Combine(selectedProfile.CUOSettings.UltimaOnlineDirectory, "client.exe"), out string version))
                        {
                            EntryClientVersion.Text = version;
                            selectedProfile.CUOSettings.ClientVersion = version;
                        }

                        selectedProfile.Save();
                    }
                }
            };

            PluginListButtonAdd.Click += (s, e) =>
            {
                if (selectedProfile != null)
                {
                    string path = AskForFile("", "Assistants (*.exe, *.dll)|*.exe;*.dll");
                    if (!string.IsNullOrEmpty(path))
                    {
                        EntryPluginList.Items.Add(new ListBoxItem() { Content = path });
                        SavePluginList();
                    }
                }
            };

            PluginListButtonRemove.Click += (s, e) =>
            {
                if (selectedProfile != null)
                {
                    if (EntryPluginList.SelectedIndex > -1)
                    {
                        EntryPluginList.Items.RemoveAt(EntryPluginList.SelectedIndex);
                        SavePluginList();
                    }
                }
            };

            SetUpSaveMethods();
        }

        private void SetUpSaveMethods()
        {
            EntryProfileName.LostFocus += (s, e) =>
            {
                if (selectedProfile != null && !EntryProfileName.Text.Equals(selectedProfile.Name) && EntryProfileName.Text == "")
                {
                    ProfileManager.DeleteProfileFile(selectedProfile);
                    selectedProfile.Name = EntryProfileName.Text;
                    selectedProfile.Save();
                    ((ListBoxItem)ProfileList.SelectedItem).Content = selectedProfile.Name;
                }
            };

            EntryAccountName.LostFocus += (s, e) =>
            {
                if (selectedProfile != null)
                {
                    if (!selectedProfile.CUOSettings.Username.Equals(EntryAccountName.Text))
                    {
                        selectedProfile.CUOSettings.Username = EntryAccountName.Text;
                        selectedProfile.Save();
                    }
                }
            };
            EntryAccountPass.LostFocus += (s, e) =>
            {
                if (selectedProfile != null)
                {
                    if (!Crypter.Decrypt(selectedProfile.CUOSettings.Password).Equals(EntryAccountPass.Password))
                    {
                        selectedProfile.CUOSettings.Password = Crypter.Encrypt(EntryAccountPass.Password);
                        selectedProfile.Save();
                    }
                }
            };
            EntrySavePass.Click += (s, e) =>
            {
                if (selectedProfile != null)
                {
                    if (!selectedProfile.CUOSettings.SaveAccount.Equals(EntrySavePass.IsChecked))
                    {
                        selectedProfile.CUOSettings.SaveAccount = (bool)(EntrySavePass.IsChecked == null ? false : EntrySavePass.IsChecked);
                        selectedProfile.Save();
                    }
                }
            };

            EntryServerIP.LostFocus += (s, e) =>
            {
                if (selectedProfile != null)
                {
                    if (!selectedProfile.CUOSettings.IP.Equals(EntryServerIP.Text))
                    {
                        selectedProfile.CUOSettings.IP = EntryServerIP.Text;
                        selectedProfile.Save();
                    }
                }
            };
            EntryServerPort.LostFocus += (s, e) =>
            {
                if (selectedProfile != null)
                {
                    if (!selectedProfile.CUOSettings.Port.ToString().Equals(EntryServerPort.Text))
                    {
                        if (ushort.TryParse(EntryServerPort.Text, out var port))
                        {
                            selectedProfile.CUOSettings.Port = port;
                            selectedProfile.Save();
                        }
                    }
                }
            };

            EntryUODirectory.LostFocus += (s, e) =>
            {
                if (selectedProfile != null)
                {
                    if (!selectedProfile.CUOSettings.UltimaOnlineDirectory.Equals(EntryUODirectory.Text))
                    {
                        selectedProfile.CUOSettings.UltimaOnlineDirectory = EntryUODirectory.Text;

                        if (ClientVersionHelper.TryParseFromFile(Path.Combine(selectedProfile.CUOSettings.UltimaOnlineDirectory, "client.exe"), out string version))
                        {
                            EntryClientVersion.Text = version;
                            selectedProfile.CUOSettings.ClientVersion = version;
                        }

                        selectedProfile.Save();
                    }
                }
            };
            EntryClientVersion.LostFocus += (s, e) =>
            {
                if (selectedProfile != null)
                {
                    if (!selectedProfile.CUOSettings.ClientVersion.Equals(EntryClientVersion.Text))
                    {
                        selectedProfile.CUOSettings.ClientVersion = EntryClientVersion.Text;
                        selectedProfile.Save();
                    }
                }
            };
            EntryClientVersion.TextChanged += (s, e) =>
            {
                if (ClientVersionHelper.IsClientVersionValid(EntryClientVersion.Text, out var version))
                {
                    EntryClientVersion.BorderBrush = Brushes.DarkGreen;
                }
                else
                {
                    EntryClientVersion.BorderBrush = Brushes.DarkRed;
                }
            };
            EntryEncrypedClient.Click += (s, e) =>
            {
                if (selectedProfile != null)
                {
                    //Need to fix, this will never be true
                    if (!selectedProfile.CUOSettings.Encryption.Equals(EntryEncrypedClient.IsChecked))
                    {
                        if (EntryEncrypedClient.IsChecked != null && (bool)EntryEncrypedClient.IsChecked)
                        {
                            selectedProfile.CUOSettings.Encryption = 1;
                        }
                        else
                        {
                            selectedProfile.CUOSettings.Encryption = 0;
                        }

                        selectedProfile.Save();
                    }
                }
            };

            EntryAutoLogin.Click += (s, e) =>
            {
                if (selectedProfile != null)
                {
                    if (!selectedProfile.CUOSettings.AutoLogin.Equals(EntryAutoLogin.IsChecked))
                    {
                        if (EntryAutoLogin.IsChecked == true)
                        {
                            selectedProfile.CUOSettings.AutoLogin = true;
                        }
                        else
                        {
                            selectedProfile.CUOSettings.AutoLogin = false;
                        }
                        selectedProfile.Save();
                    }
                }
            };
            EntryReconnect.Click += (s, e) =>
            {
                if (selectedProfile != null)
                {
                    if (!selectedProfile.CUOSettings.Reconnect.Equals(EntryReconnect.IsChecked))
                    {
                        if (EntryReconnect.IsChecked == true)
                        {
                            selectedProfile.CUOSettings.Reconnect = true;
                        }
                        else
                        {
                            selectedProfile.CUOSettings.Reconnect = false;
                        }
                        selectedProfile.Save();
                    }
                }
            };
            EntryReconnectTime.LostFocus += (s, e) =>
            {
                if (selectedProfile != null)
                {
                    if(!selectedProfile.CUOSettings.Reconnect.ToString().Equals(EntryReconnectTime.Text.ToString())) 
                    {
                        if(int.TryParse(EntryReconnectTime.Text, out int ms))
                        {
                            selectedProfile.CUOSettings.ReconnectTime = ms;
                            selectedProfile.Save();
                        }
                    }
                }
            };
            EntryLoginMusic.Click += (s, e) =>
            {
                if (selectedProfile != null)
                {
                    if (!selectedProfile.CUOSettings.LoginMusic.Equals(EntryLoginMusic.IsChecked))
                    {
                        if (EntryLoginMusic.IsChecked == true)
                        {
                            selectedProfile.CUOSettings.LoginMusic = true;
                        }
                        else
                        {
                            selectedProfile.CUOSettings.LoginMusic = false;
                        }
                        selectedProfile.Save();
                    }
                }
            };
            EntryMusicVolume.ValueChanged += (s, e) => 
            {
                if(selectedProfile != null)
                {
                    if (!selectedProfile.CUOSettings.LoginMusicVolume.Equals((int)EntryMusicVolume.Value))
                    {
                        selectedProfile.CUOSettings.LoginMusicVolume = (int)EntryMusicVolume.Value;
                        selectedProfile.Save();
                    }
                }
            };
        }

        private void SavePluginList()
        {
            if (selectedProfile != null)
            {
                List<string> list = new List<string>();

                foreach (var entry in EntryPluginList.Items)
                {
                    list.Add(((ListBoxItem)entry).Content.ToString());
                }

                selectedProfile.CUOSettings.Plugins = list.ToArray();
                selectedProfile.Save();
            }
        }

        private void SetEntries(Profile profile)
        {
            EntryProfileName.Text = profile.Name;

            EntryAccountName.Text = profile.CUOSettings.Username;
            EntryAccountPass.Password = profile.CUOSettings.Password;
            EntrySavePass.IsChecked = profile.CUOSettings.SaveAccount;

            EntryServerIP.Text = profile.CUOSettings.IP;
            EntryServerPort.Text = profile.CUOSettings.Port.ToString();

            EntryUODirectory.Text = profile.CUOSettings.UltimaOnlineDirectory;
            EntryClientVersion.Text = profile.CUOSettings.ClientVersion;
            EntryEncrypedClient.IsChecked = profile.CUOSettings.Encryption == 0 ? false : true;

            EntryPluginList.Items.Clear();
            foreach (var entry in profile.CUOSettings.Plugins)
            {
                EntryPluginList.Items.Add(new ListBoxItem() { Content = entry });
            }

            EntryAutoLogin.IsChecked = profile.CUOSettings.AutoLogin;
            EntryReconnect.IsChecked = profile.CUOSettings.Reconnect;
            EntryReconnectTime.Text = profile.CUOSettings.ReconnectTime.ToString();
            EntryLoginMusic.IsChecked = profile.CUOSettings.LoginMusic;
            EntryMusicVolume.Value = profile.CUOSettings.LoginMusicVolume;
        }

        private void ProfileList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ProfileManager.TryFindProfile(((ListBoxItem)ProfileList.SelectedItem).Content.ToString(), out Profile profile))
            {
                if (profile != null)
                {
                    selectedProfile = profile;
                    SetEntries(profile);
                }
            }
        }

        private void ButtonCopy_MouseUp(object sender, RoutedEventArgs e)
        {
            throw new System.NotImplementedException();
        }

        private void ButtonDelete_MouseUp(object sender, RoutedEventArgs e)
        {
            throw new System.NotImplementedException();
        }

        private void ButtonNew_MouseUp(object sender, RoutedEventArgs e)
        {
            throw new System.NotImplementedException();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e) //Loose focus on textbox
        {
            MainCanvas.Focus();
        }

        private string AskForFile(string intialDirectory, string fileFilter)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = intialDirectory;
            openFileDialog.Filter = fileFilter;
            openFileDialog.CheckFileExists = true;
            openFileDialog.CheckPathExists = true;

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

        private void ReconnectUP(object sender, RoutedEventArgs e)
        {
            if(selectedProfile != null && int.TryParse(EntryReconnectTime.Text, out int v))
            {
                v += 100;
                EntryReconnectTime.Text = v.ToString();
                selectedProfile.CUOSettings.ReconnectTime = v;
                selectedProfile.Save();
            }
        }

        private void ReconnectDOWN(object sender, RoutedEventArgs e)
        {
            if (selectedProfile != null && int.TryParse(EntryReconnectTime.Text, out int v))
            {
                v -= 100;
                EntryReconnectTime.Text = v.ToString();
                selectedProfile.CUOSettings.ReconnectTime = v;
                selectedProfile.Save();
            }
        }
    }
}
