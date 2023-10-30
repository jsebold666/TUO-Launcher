﻿using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace TazUO_Launcher.Utility
{
    class UpdateManager
    {
        private const string UPDATE_ZIP_URL = "https://github.com/bittiez/ClassicUO/releases/latest/download/ClassicUO.zip";

        public static UpdateManager Instance { get; private set; } = new UpdateManager();

        public bool DownloadInProgress { get; private set; } = false;

        private static Dispatcher dispatcher = Dispatcher.CurrentDispatcher;

        public Task DownloadTUO(Action<int>? action = null)
        {
            if (DownloadInProgress)
            {
                return Task.CompletedTask;
            }
            var client = new WebClient();
            client.DownloadProgressChanged += (s, p) =>
            {
                dispatcher.InvokeAsync(() =>
                {
                    action?.Invoke(p.ProgressPercentage);
                });
            };

            Task dl = Task.Factory.StartNew(() =>
            {
                DownloadInProgress = true;

                string zipPath = System.IO.Path.GetTempFileName();

                client.DownloadFileAsync(new Uri(UPDATE_ZIP_URL), zipPath);

                client.DownloadFileCompleted += (s, p) =>
                {
                    ZipFile.ExtractToDirectory(zipPath, Path.Combine(LauncherSettings.LauncherPath, "TazUO"));

                    DownloadInProgress = false;
                };
            });

            return dl;
        }
    }
}