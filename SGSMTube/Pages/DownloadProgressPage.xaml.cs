using SGSMTube.ViewModels;
using SGSMTube_Lib.Models;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SGSMTube.Pages
{
    public partial class DownloadProgressPage : Page
    {
        private const int DelayAfterDownload = 200;
        private const int DelayAfterCompletion = 300;

        private readonly List<YoutubeVideoVM> _selectedVideos;
        private readonly bool _isAudioOnlyDownload;
        private readonly YTDownloader _downloader;

        public DownloadProgressPage()
        {
            InitializeComponent();
            _downloader = new YTDownloader();
            _selectedVideos = new List<YoutubeVideoVM>();
        }

        public DownloadProgressPage(List<YoutubeVideoVM> selectedVideos, bool isAudioOnlyDownload)
        {
            InitializeComponent();
            _downloader = new YTDownloader();
            _selectedVideos = selectedVideos;
            _isAudioOnlyDownload = isAudioOnlyDownload;
        }

        private async void DownlaodProgressPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (_isAudioOnlyDownload)
                await DownloadAudio();
            else
                await DownloadMuxedVideo();
        }

        private async Task DownloadAudio()
        {
            int counter = 1;
            IProgress<double> progress = new Progress<double>(val =>
            {
                lblCurrentTrackStatus.Text = "Downloading video...";
                prgProgress.Value = Convert.ToInt32(val * 100);
            });

            foreach (var item in _selectedVideos)
            {
                UpdateUIForDownload(item, counter);
                string videoPath = await _downloader.DownloadAudio(item.VideoUrl, progress);
                await Task.Delay(DelayAfterDownload);
                lblCurrentTrackStatus.Text = "Converting video to Mp3...";
                string mp3FilePath = await _downloader.ConvertVideoToMp3(videoPath);
                lblCurrentTrackStatus.Text = "Completed";
                await Task.Delay(DelayAfterCompletion);
                counter++;
            }

            lblCurrentTrackStatus.Text = "All download tasks finished, Check 'Music' Folder.";
            btnClose.IsEnabled = true;
        }

        private void UpdateUIForDownload(YoutubeVideoVM item, int counter)
        {
            prgProgress.Value = 0;
            lblCounter.Text = $"Downloading {counter}/{_selectedVideos.Count}";
            lblCurrentTrackStatus.Text = "Fetching video information...";
            lblTitle.Text = item.Title;
            lblAuthor.Text = item.Author;
            imgThumbnail.Source = new BitmapImage(new Uri(item.ThumbnailImage));
            lblDuration.Text = item.Duration;
            lblUrl.Text = item.VideoUrl;
        }

        private async Task DownloadMuxedVideo()
        {
            int counter = 1;
            IProgress<double> progress = new Progress<double>(val =>
            {
                lblCurrentTrackStatus.Text = "Downloading video...";
                prgProgress.Value = Convert.ToInt32(val * 100);
            });

            foreach (var item in _selectedVideos)
            {
                UpdateUIForDownload(item, counter);
                string videoPath = await _downloader.DownloadMuxedVideo(item.VideoUrl, progress);
                lblCurrentTrackStatus.Text = "Completed";
                await Task.Delay(DelayAfterCompletion);
                counter++;
            }

            lblCurrentTrackStatus.Text = "All download tasks finished, Check 'Music' Folder.";
            btnClose.IsEnabled = true;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            while (NavigationService.CanGoBack)
            {
                NavigationService.RemoveBackEntry();
            }
            NavigationService.Navigate(new YoutubeToMp3());
        }
    }
}
