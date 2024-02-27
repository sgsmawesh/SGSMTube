using YoutubeExplode.Videos;

namespace SGSMTube_Lib.Views
{
    public class VideoDetailModel
    {
        public string Url { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
        public TimeSpan? Duration { get; set; }
        public VideoId VideoId { get; set; }
        public string ThumbnailImage { get; set; }
    }
}
