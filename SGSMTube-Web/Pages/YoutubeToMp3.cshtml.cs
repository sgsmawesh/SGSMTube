using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SGSMTube_Lib.Models;
using System.Diagnostics;


namespace SGSMTube_Web.Pages
{
    public class YoutubeToMp3Model : PageModel
    {
        private readonly YTDownloader _downloader;
        public YoutubeToMp3Model()
        {
            _downloader = new YTDownloader();
        }
        

        public async Task<IActionResult> OnGetAsync(bool download = false)
        {
            if (download)
            {
                Debug.WriteLine("Download started");
                IProgress<double> progress = new Progress<double>((p) => { Debug.WriteLine(p); });

                try
                {
                    var file = await _downloader.GetVideoStream("https://youtu.be/7RMQksXpQSk", progress);

                    if (file == null)
                    {
                        Debug.WriteLine("No file was downloaded.");
                        return NotFound();
                    }

                    return File(file, "application/octet-stream", "my-test-video02.webm");

                    //return new FileStreamResult(file, "application/octet-stream")
                    //{
                    //    FileDownloadName = "my-test-video.webm"
                    //};
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"An error occurred: {ex.Message}");
                    return StatusCode(500);
                }
            }
            else
            {
                return StatusCode(200);
            }
            // Handle the case where download is false...
        }

    }
}
