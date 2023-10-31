using System;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Reflection;
using System.Threading;
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
        private static HttpClient httpClient = new HttpClient();
        public Version RemoteVersion { get; private set; } = null;
        public Version LocalVersion { get; private set; } = null;

        public Task DownloadTUO(Action<int>? action = null)
        {
            if (DownloadInProgress)
            {
                return Task.CompletedTask;
            }

            httpClient.Timeout = TimeSpan.FromMinutes(5);

            DownloadProgress downloadProgress = new DownloadProgress();

            downloadProgress.DownloadProgressChanged += (s, e) =>
            {
                dispatcher.InvokeAsync(() =>
                {
                    action?.Invoke((int)(downloadProgress.ProgressPercentage * 100));
                });
            };

            Task download = Task.Factory.StartNew(() =>
            {
                string tempFilePath = Path.GetTempFileName();
                using (var file = new FileStream(tempFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    httpClient.DownloadAsync(UPDATE_ZIP_URL, file, downloadProgress).Wait();
                }

                try
                {
                    ZipFile.ExtractToDirectory(tempFilePath, Path.Combine(LauncherSettings.LauncherPath, "TazUO"));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }

                DownloadInProgress = false;

            });

            return download;
        }

        public void GetRemoteTazUOVersion(Action? onVersionFound = null)
        {
            Task.Factory.StartNew(() =>
            {
                HttpRequestMessage request = new HttpRequestMessage()
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri("https://github.com/bittiez/TazUO/raw/main/tazuoversioninfo.txt"),
                };
                HttpContent response = httpClient.Send(request).Content;
                string result = response.ReadAsStringAsync().Result;

                if (Version.TryParse(result, out Version rv))
                {
                    RemoteVersion = rv;
                    dispatcher.InvokeAsync(() =>
                    {
                        onVersionFound?.Invoke();
                    });

                }
            });
        }

        public Version? GetInstalledTazUOVersion(string exePath)
        {
            return LocalVersion = AssemblyName.GetAssemblyName(exePath).Version;
        }

        public class DownloadProgress : IProgress<float>
        {
            public event EventHandler DownloadProgressChanged;

            public float ProgressPercentage { get; set; }

            public void Report(float value)
            {
                ProgressPercentage = value;
                DownloadProgressChanged?.Invoke(this, EventArgs.Empty);
            }
        }

    }

    public static class HttpClientExtensions
    {
        public static async Task DownloadAsync(this HttpClient client, string requestUri, Stream destination, IProgress<float> progress = null, CancellationToken cancellationToken = default)
        {
            // Get the http headers first to examine the content length
            using (var response = await client.GetAsync(requestUri, HttpCompletionOption.ResponseHeadersRead))
            {
                var contentLength = response.Content.Headers.ContentLength;

                using (var download = await response.Content.ReadAsStreamAsync())
                {

                    // Ignore progress reporting when no progress reporter was 
                    // passed or when the content length is unknown
                    if (progress == null || !contentLength.HasValue)
                    {
                        await download.CopyToAsync(destination);
                        return;
                    }

                    // Convert absolute progress (bytes downloaded) into relative progress (0% - 100%)
                    var relativeProgress = new Progress<long>(totalBytes => progress.Report((float)totalBytes / contentLength.Value));
                    // Use extension method to report progress while downloading
                    await download.CopyToAsync(destination, 81920, relativeProgress, cancellationToken);
                    progress.Report(1);
                }
            }
        }
    }

    public static class StreamExtensions
    {
        public static async Task CopyToAsync(this Stream source, Stream destination, int bufferSize, IProgress<long> progress = null, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (!source.CanRead)
                throw new ArgumentException("Has to be readable", nameof(source));
            if (destination == null)
                throw new ArgumentNullException(nameof(destination));
            if (!destination.CanWrite)
                throw new ArgumentException("Has to be writable", nameof(destination));
            if (bufferSize < 0)
                throw new ArgumentOutOfRangeException(nameof(bufferSize));

            var buffer = new byte[bufferSize];
            long totalBytesRead = 0;
            int bytesRead;
            while ((bytesRead = await source.ReadAsync(buffer, 0, buffer.Length, cancellationToken).ConfigureAwait(false)) != 0)
            {
                await destination.WriteAsync(buffer, 0, bytesRead, cancellationToken).ConfigureAwait(false);
                totalBytesRead += bytesRead;
                progress?.Report(totalBytesRead);
            }
        }
    }
}
