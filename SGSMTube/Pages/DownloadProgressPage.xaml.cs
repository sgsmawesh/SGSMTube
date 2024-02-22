using SGSMTube.ViewModels;
using SGSMTube_Lib.Models;
using System.Windows;
using System.Windows.Controls;

namespace SGSMTube.Pages
{
    /// <summary>
    /// Interaction logic for DownloadProgressPage.xaml
    /// </summary>
    public partial class DownloadProgressPage : Page
    {
        private const int DelayAfterDownload = 200;
        private const int DelayAfterCompletion = 300;

        private readonly List<YoutubeVideoVM> _selectedVideos;
        private readonly YTDownloader _downloader;

        public DownloadProgressPage()
        {
            InitializeComponent();
            _downloader = new YTDownloader();
            _selectedVideos = new List<YoutubeVideoVM>();
        }

        public DownloadProgressPage(List<YoutubeVideoVM> selectedVideos)
        {
            InitializeComponent();
            _downloader = new YTDownloader();
            _selectedVideos = selectedVideos;
        }

        private async void DownlaodProgressPage_Loaded(object sender, RoutedEventArgs e)
        {
            int counter = 1;
            foreach (var item in _selectedVideos)
            {
                prgProgress.Value = 0;
                lblCounter.Text = $"Downloading {counter}/{_selectedVideos.Count}";
                lblCurrentTrackStatus.Text = "Fetching video information...";
                lblTitle.Text = item.Title;
                lblAuthor.Text = item.Author;
                IProgress<double> progress = new Progress<double>(val =>
                {
                    lblCurrentTrackStatus.Text = "Downloading video...";
                    prgProgress.Value = Convert.ToInt32(val * 100);
                });
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
