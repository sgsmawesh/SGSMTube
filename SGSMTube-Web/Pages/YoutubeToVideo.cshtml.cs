using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SGSMTube_Lib.Models;
using System.Diagnostics;

namespace SGSMTube_Web.Pages
{
    public class YoutubeToVideoModel : PageModel
    {
        private readonly YTDownloader _downloader;
        public YoutubeToVideoModel(YTDownloader downloader)
        {
            _downloader = downloader;
        }


        public async Task<IActionResult> OnGetAsync(string videoUrl)
        {

            Debug.WriteLine("Download started");
            IProgress<double> progress = new Progress<double>((p) => { Debug.WriteLine(p); });
            try
            {
                var fileInfo = await _downloader.GetMuxedVideoStream(videoUrl, progress);

                if (fileInfo == null)
                {
                    Debug.WriteLine("No file was downloaded.");
                    return NotFound();
                }

                return File(fileInfo.Item1, "application/octet-stream", fileInfo.Item2);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"An error occurred: {ex.Message}");
                return StatusCode(500, ex.Message);
            }
        }
    }
}
