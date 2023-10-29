using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace TazUO_Launcher
{
    static class ProfileManager
    {
        public static Task<Profile[]> GetAllProfiles()
        {
            return Task<Profile[]>.Factory.StartNew(() =>
            {
                if (Directory.Exists(LauncherSettings.ProfilesPath))
                {
                    string[] profiles = Directory.GetFiles(LauncherSettings.ProfilesPath, "*.json", SearchOption.TopDirectoryOnly);

                    List<Profile> list = new List<Profile>();

                    foreach (string profile in profiles)
                    {
                        try
                        {
                            var loadedProfile = JsonSerializer.Deserialize<Profile>(
                                File.ReadAllText(profile)
                                );
                            if (loadedProfile != null)
                            {
                                list.Add(loadedProfile);
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine($"---- Error loading profile from [ {profile} ] ---");
                            Console.WriteLine(e.StackTrace);
                            Console.WriteLine();
                        }
                    }

                    if(list.Count == 0)
                    {
                        Profile blank = new Profile();
                        SaveProfile(blank);
                        list.Add(blank);
                    }

                    return list.ToArray();
                }
                else //Create new profile directory and a blank profile
                {
                    try
                    {
                        Directory.CreateDirectory(LauncherSettings.ProfilesPath);
                        Profile blank = new Profile();
                        SaveProfile(blank);
                        return new Profile[] { blank };

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("---- Error creating profile directory ---");
                        Console.WriteLine(e.StackTrace);
                        Console.WriteLine();
                    }
                    return Array.Empty<Profile>();
                }
            });
        }

        public static void SaveProfile(Profile profile)
        {
            try
            {
                var data = JsonSerializer.Serialize(profile);
                File.WriteAllText(GetFilePathForProfile(profile), data);
            }catch(Exception e)
            {
                Console.WriteLine($"---- Failed to save profile [ {profile.Name} ] ---");
                Console.WriteLine(e.StackTrace);
                Console.WriteLine();
            }
        }

        private static string GetFilePathForProfile(Profile profile)
        {
            return Path.Combine(LauncherSettings.ProfilesPath, profile.Name + ".json");
        }
    }
}
