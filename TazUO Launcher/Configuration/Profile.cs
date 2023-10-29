﻿using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using static System.Net.Mime.MediaTypeNames;
using System.Text.RegularExpressions;
using TazUO_Launcher.Utility;

namespace TazUO_Launcher
{
    class Profile
    {
        [JsonIgnore]
        private Settings cUOSettings;

        public string Name { get; set; } = RandomWord.GenerateName(Random.Shared.Next(5, 20));
        public string SettingsFile { get; set; } = Guid.NewGuid().ToString();

        [JsonIgnore]
        public Settings CUOSettings
        {
            get
            {
                if (cUOSettings == null)
                {
                    LoadCUOSettings();
                    return cUOSettings;
                }
                else
                { 
                    return cUOSettings; }
            }
            private set => cUOSettings = value;
        }

        private void LoadCUOSettings()
        {
            if (File.Exists(GetSettingsFilePath()))
            {
                try
                {
                    var data = JsonSerializer.Deserialize<Settings>(File.ReadAllText(GetSettingsFilePath()));
                    if (data != null)
                    {
                        CUOSettings = data;
                    }
                    else
                    {
                        CUOSettings = new Settings();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    CUOSettings = new Settings();
                }
            }
            else
            {
                CUOSettings = new Settings();
            }
        }

        public void OverrideSettings(Settings settings)
        {
            cUOSettings = settings;
        }

        public void Save()
        {
            try
            {
                var data = JsonSerializer.Serialize(this, typeof(Profile));
                Directory.CreateDirectory(LauncherSettings.ProfilesPath);
                File.WriteAllText(GetProfileFilePath(), data);

                var settingsData = CUOSettings.GetSaveData();
                Directory.CreateDirectory(LauncherSettings.SettingsPath);
                File.WriteAllText(GetSettingsFilePath(), settingsData);
            }
            catch (Exception e)
            {
                Console.WriteLine($"---- Failed to save profile [ {Name} ] ---");
                Console.WriteLine(e.ToString());
                Console.WriteLine();
            }
        }

        public string GetSettingsFilePath()
        {
            return Path.Combine(LauncherSettings.SettingsPath, SettingsFile + ".json");
        }

        public string GetProfileFilePath()
        {
            return Path.Combine(LauncherSettings.ProfilesPath, Name + ".json");
        }
    }
}