﻿using NReco.VideoConverter;
using SGSMTube_Lib.Views;
using System.Drawing;
using YoutubeExplode;
using YoutubeExplode.Common;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;

namespace SGSMTube_Lib.Models
{
    public class YTDownloader
    {
        private readonly YoutubeClient _youtubeClient;
        public YTDownloader()
        {
            _youtubeClient = new YoutubeClient();
        }

        public async Task<IEnumerable<VideoDetailModel>?> SearchVideo(string keyword, int howMany = 10)
        {
            try
            {
                var searchResults = await _youtubeClient.Search.GetVideosAsync(keyword).CollectAsync(howMany);
                if (searchResults == null)
                    return null;
                var result = new List<VideoDetailModel>();
                result = searchResults.Select(video => new VideoDetailModel { Url = video.Url, Author = video.Author.ChannelTitle, Title = video.Title, Duration = video.Duration, VideoId = video.Id.Value, ThumbnailImage = $"https://img.youtube.com/vi/{video.Id.Value}/maxresdefault.jpg" }).ToList();
                return result;
            }
            catch (System.Net.Http.HttpRequestException ex)
            {
                return null;
            }
        }

        public async Task<IEnumerable<VideoDetailModel>?> SearchVideoByUrl(string videoUrl)
        {
            var videoInfo = await _youtubeClient.Videos.GetAsync(videoUrl);
            if (videoInfo == null) return null;
            return new List<VideoDetailModel>() {
                new VideoDetailModel
                {
                    Author = videoInfo.Author.ChannelTitle,
                    Duration = videoInfo.Duration,
                    ThumbnailImage = $"https://img.youtube.com/vi/{videoInfo.Id.Value}/maxresdefault.jpg", Title=videoInfo.Title,
                    Url=videoInfo.Url,
                    VideoId=videoInfo.Id, Description =videoInfo.Description
                }
            };
        }
        public async Task<string> DownloadAudio(string videoUrl, IProgress<double>? progress)
        {
            var videoInfo = await _youtubeClient.Videos.GetAsync(videoUrl);
            var streamManifest = await _youtubeClient.Videos.Streams.GetManifestAsync(videoUrl);
            var streamInfo = streamManifest.GetAudioOnlyStreams().GetWithHighestBitrate();
            string extn = streamInfo.Container.Name.ToLower();
            string videoFileNameWithoutExtn = Utils.CleanFileName($"{videoInfo.Title}-{videoInfo.Author.ChannelTitle}");
            string videoFileName = $"{videoFileNameWithoutExtn}.{extn}";
            string vidFilePath = Path.Combine(Utils.GetSaveFolderPath(), videoFileName);
            await _youtubeClient.Videos.Streams.DownloadAsync(streamInfo, vidFilePath, progress);
            return vidFilePath;
        }

        public async Task<string> DownloadMuxedVideo(string videoUrl, IProgress<double>? progress)
        {
            var videoInfo = await _youtubeClient.Videos.GetAsync(videoUrl);
            var streamManifest = await _youtubeClient.Videos.Streams.GetManifestAsync(videoUrl);
            var streamInfo = streamManifest.GetMuxedStreams().GetWithHighestBitrate();
            string extn = streamInfo.Container.Name.ToLower();
            string videoFileNameWithoutExtn = Utils.CleanFileName($"{videoInfo.Title}-{videoInfo.Author.ChannelTitle}");
            string videoFileName = $"{videoFileNameWithoutExtn}.{extn}";
            string vidFilePath = Path.Combine(Utils.GetSaveFolderPath(), videoFileName);
            await _youtubeClient.Videos.Streams.DownloadAsync(streamInfo, vidFilePath, progress);
            return vidFilePath;
        }
        public async Task<Tuple<Stream, string>> GetMuxedVideoStream(string videoUrl, IProgress<double>? progress)
        {
            var videoInfo = await _youtubeClient.Videos.GetAsync(videoUrl);
            var streamManifest = await _youtubeClient.Videos.Streams.GetManifestAsync(videoUrl);
            var streamInfo = streamManifest.GetMuxedStreams().GetWithHighestBitrate();
            string extn = streamInfo.Container.Name.ToLower();
            string videoFileNameWithoutExtn = Utils.CleanFileName($"{videoInfo.Title}-{videoInfo.Author.ChannelTitle}");
            string videoFileName = $"{videoFileNameWithoutExtn}.{extn}";
            var stream = await _youtubeClient.Videos.Streams.GetAsync(streamInfo);
            stream.Seek(0, SeekOrigin.Begin);

            return new Tuple<Stream, string>(stream, videoFileName);
        }
        public async Task<Tuple<Stream, string>> GetAudioOnlyStream(string videoUrl, IProgress<double>? progress)
        {
            var videoInfo = await _youtubeClient.Videos.GetAsync(videoUrl);
            var streamManifest = await _youtubeClient.Videos.Streams.GetManifestAsync(videoUrl);
            var streamInfo = streamManifest.GetAudioOnlyStreams().GetWithHighestBitrate();
            string extn = streamInfo.Container.Name.ToLower();
            string videoFileNameWithoutExtn = Utils.CleanFileName($"{videoInfo.Title}-{videoInfo.Author.ChannelTitle}");
            string videoFileName = $"{videoFileNameWithoutExtn}.{extn}";
            var stream = await _youtubeClient.Videos.Streams.GetAsync(streamInfo);
            stream.Seek(0, SeekOrigin.Begin);

            return new Tuple<Stream, string>(stream, videoFileName);
        }

        public async Task<string> ConvertVideoToMp3(string vidFilePath)
        {
            return await Task.Run(async () =>
            {
                var extn = Path.GetExtension(vidFilePath);
                string mp3FilePath = vidFilePath.Replace(extn, ".mp3");
                var converter = new FFMpegConverter();
                converter.ConvertMedia(vidFilePath, mp3FilePath, "mp3");
                await Task.Delay(500);
                File.Delete(vidFilePath);
                return mp3FilePath;
            });
        }
        public async Task<byte[]> GetThumbnailImage(VideoId videoId)
        {
            using (var httpClient = new HttpClient())
            {
                var imageContent = await httpClient.GetByteArrayAsync($"https://img.youtube.com/vi/{videoId}/maxresdefault.jpg").ConfigureAwait(false);
                return imageContent;
            }
        }
        public string GetThumbnailImageUrl(VideoId videoId)
        {
            return $"https://img.youtube.com/vi/{videoId}/maxresdefault.jpg";
        }
    }
}
