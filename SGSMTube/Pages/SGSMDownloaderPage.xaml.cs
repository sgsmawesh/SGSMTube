using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SGSMTube.Pages
{
    public partial class SGSMDownloaderPage : Page
    {
        private const int MaxConnections = 8;
        private readonly HttpClient httpClient = new HttpClient();
        private readonly string downloadUrl = "https://archive.org/download/bolleyBall_bollywood/Aagey%20Se%20Right%202009%20Hindi%20www.downloadhub.lol%20720p%20HDRip%20ESubs.mp4";
        private readonly string savePath = "D:\\aagey-se-right.mp4";
        private bool isDownloading = false;
        private readonly ConcurrentDictionary<int, int> segmentProgress = new ConcurrentDictionary<int, int>();
        Stopwatch stopwatch = new Stopwatch();

        public SGSMDownloaderPage()
        {
            InitializeComponent();
        }

        private async void btnStartDownload_Click(object sender, RoutedEventArgs e)
        {
            if (!isDownloading)
            {
                await StartDownload();
            }
            else
            {
                PauseDownload();
            }
        }

        private async Task StartDownload()
        {
            string[] segmentPaths = Enumerable.Range(0, MaxConnections)
                .Select(i => Path.Combine(Path.GetTempPath(), $"segment{i}.tmp"))
                .ToArray();

            try
            {
                isDownloading = true;
                btnStartDownload.Content = "Pause";
                long fileSize = await GetFileSize(downloadUrl);
                lblFileSize.Text = ConvertBytes(fileSize);
                long segmentSize = fileSize / MaxConnections;
                stopwatch.Start();
                Task[] downloadTasks = Enumerable.Range(0, MaxConnections)
                    .Select(i =>
                    {
                        long startRange = i * segmentSize;
                        long endRange = (i == MaxConnections - 1) ? fileSize : (i + 1) * segmentSize - 1;
                        return DownloadSegmentAsync(startRange, endRange, segmentPaths[i], i);
                    })
                    .ToArray();

                await Task.WhenAll(downloadTasks);
                CombineSegments(segmentPaths, savePath);
                stopwatch.Stop();
                MessageBox.Show($"Download complete in {stopwatch.ElapsedMilliseconds/1000} s!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Download failed: " + ex.Message);
            }
            finally
            {
                isDownloading = false;
                btnStartDownload.Content = "Download";
            }
        }

        private async Task<long> GetFileSize(string url)
        {
            using (var response = await httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead))
            {
                response.EnsureSuccessStatusCode();
                return response.Content.Headers.ContentLength ?? 0;
            }
        }

        private void ReportOverallProgress()
        {
            int overallProgress = segmentProgress.Values.Sum() / MaxConnections;
            progressBar.Value = overallProgress;
            lblProgress.Text = $"{overallProgress}%";
        }

        private void ReportProgress(int segmentIndex, int progress)
        {
            segmentProgress[segmentIndex] = progress;
            ReportOverallProgress();
        }

        public static string ConvertBytes(long bytes)
        {
            const long kilobyte = 1024;
            const long megabyte = kilobyte * 1024;
            const long gigabyte = megabyte * 1024;

            if (bytes >= gigabyte)
            {
                return $"{(double)bytes / gigabyte:F2} GB";
            }
            else if (bytes >= megabyte)
            {
                return $"{(double)bytes / megabyte:F2} MB";
            }
            else if (bytes >= kilobyte)
            {
                return $"{(double)bytes / kilobyte:F2} KB";
            }
            else
            {
                return $"{bytes} bytes";
            }
        }

        private async Task DownloadSegmentAsync(long startRange, long endRange, string segmentPath, int segmentIndex)
        {
            using (var response = await httpClient.GetAsync(downloadUrl, HttpCompletionOption.ResponseHeadersRead))
            {
                response.EnsureSuccessStatusCode();

                using (var contentStream = await response.Content.ReadAsStreamAsync())
                using (var fileStream = File.Create(segmentPath))
                {
                    var buffer = new byte[81920];
                    var bytesRead = 0;
                    var totalBytesRead = 0;
                    var totalBytesToRead = endRange - startRange + 1;

                    while (totalBytesRead < startRange && (bytesRead = await contentStream.ReadAsync(buffer, 0, (int)Math.Min(buffer.Length, startRange - totalBytesRead))) > 0)
                    {
                        totalBytesRead += bytesRead;
                    }

                    while (totalBytesRead < endRange && (bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                    {
                        await fileStream.WriteAsync(buffer, 0, bytesRead);
                        totalBytesRead += bytesRead;

                        var progressPercentage = (int)(((double)totalBytesRead - startRange) / totalBytesToRead * 100);
                        ReportProgress(segmentIndex, progressPercentage);
                    }
                }
            }
        }

        private void CombineSegments(string[] segmentPaths, string finalFilePath)
        {
            using (FileStream finalFileStream = File.Create(finalFilePath))
            {
                foreach (string segmentPath in segmentPaths)
                {
                    byte[] buffer = File.ReadAllBytes(segmentPath);
                    finalFileStream.Write(buffer, 0, buffer.Length);
                }
            }
        }

        private void PauseDownload()
        {
            // Implement pause functionality if needed
        }
    }
}
