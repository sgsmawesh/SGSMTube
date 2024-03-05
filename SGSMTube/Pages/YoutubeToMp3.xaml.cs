using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using SGSMTube.ViewModels;
using SGSMTube_Lib.Models;
using SGSMTube_Lib.Views;

namespace SGSMTube.Pages
{
    /// <summary>
    /// Interaction logic for YoutubeToMp3.xaml
    /// </summary>
    public partial class YoutubeToMp3 : Page
    {
        private readonly YTDownloader _downloader;
        private List<YoutubeVideoVM> _selectedVideos;
        public YoutubeToMp3()
        {
            InitializeComponent();
            _downloader = new YTDownloader();
            _selectedVideos = new List<YoutubeVideoVM>();
            UpdateSelectedVideoStatus();
        }

        private async void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            string keyword = txtSearchTerm.Text;
            if (string.IsNullOrEmpty(keyword))
                return;
            string strHowMany = txtHowManyResults.Text;
            int howMany = 0;
            int.TryParse(strHowMany, out howMany);
            if (howMany == 0) howMany = 10;
            // Check if keyword is a search term or URL
            Uri uriResult;
            bool isUrl = Uri.TryCreate(keyword, UriKind.Absolute, out uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

            dgVideos.ItemsSource = null;
            progressPanel.Visibility = Visibility.Visible;
            var items = new List<YoutubeVideoVM>();
            IEnumerable<VideoDetailModel>? videoSearchResults = null;
            if (isUrl)
            {
                // Keyword is a URL
                videoSearchResults = await _downloader.SearchVideoByUrl(keyword);
            }
            else
            {
                // Keyword is not a URL
                videoSearchResults = await _downloader.SearchVideo(keyword, howMany);

            }
            if (videoSearchResults == null)
            {
                progressPanel.Visibility = Visibility.Collapsed;
                return;
            }
            foreach (var item in videoSearchResults)
            {
                var duration = item.Duration.HasValue
                                        ? item.Duration.Value.ToString("hh\\:mm\\:ss")
                                        : "00:00:00";
                var video = new YoutubeVideoVM() { IsChecked = false, Author = item.Author, Duration = duration, Title = item.Title, VideoId = item.VideoId, VideoUrl = item.Url, ThumbnailImage = item.ThumbnailImage };
                video.PropertyChanged += Video_PropertyChanged;
                items.Add(video);
            }

            dgVideos.ItemsSource = items;
            progressPanel.Visibility = Visibility.Collapsed;
        }

        private void Video_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(YoutubeVideoVM.IsChecked))
            {
                var video = sender as YoutubeVideoVM;
                if (video != null)
                {
                    Debug.WriteLine($"{video.Title}");
                    if (video.IsChecked)
                    {
                        AddVideoIfNotExists(video);
                    }
                    else
                    {
                        RemoveVideo(video);
                    }
                    UpdateSelectedVideoStatus();
                }
            }
        }

        private void AddVideoIfNotExists(YoutubeVideoVM video)
        {
            if (video != null && !_selectedVideos.Any(x => x.VideoId == video.VideoId))
            {
                // Video not in the _selectedVideos, so add it
                _selectedVideos.Add(video);
            }
        }
        private void RemoveVideo(YoutubeVideoVM video)
        {
            if (video != null && _selectedVideos.Any(x => x.VideoId == video.VideoId))
            {
                _selectedVideos.Remove(video);
            }
        }

        private void UpdateSelectedVideoStatus()
        {
            btnSelectedSongs.Content = $"Count of videos selected to download : {_selectedVideos.Count}";
        }

        private void btnDownloadNow_Click(object sender, RoutedEventArgs e)
        {
            bool isAudioOnlyDownload = rdbAudioOnly.IsChecked == true;
            if (_selectedVideos.Any())
                this.NavigationService.Navigate(new DownloadProgressPage(_selectedVideos, isAudioOnlyDownload));
        }

        private void youtubeToMp3Page_Loaded(object sender, RoutedEventArgs e)
        {
            while (NavigationService.CanGoBack)
            {
                NavigationService.RemoveBackEntry();
            }
        }

        private void btnSelectedSongs_Click(object sender, RoutedEventArgs e)
        {
            //SelectionViewerDialog dialog = new SelectionViewerDialog(Window.GetWindow(this));
            //dialog.ShowDialog();
        }
    }
}