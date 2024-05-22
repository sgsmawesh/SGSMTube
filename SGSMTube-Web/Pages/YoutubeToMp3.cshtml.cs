using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SGSMTube_Lib.Models;
using System.Diagnostics;


namespace SGSMTube_Web.Pages
{
    public class YoutubeToMp3Model : PageModel
    {
        private readonly YTDownloader _downloader;
        public YoutubeToMp3Model(YTDownloader downloader)
        {
            _downloader = downloader;
        }


        public async Task<IActionResult> OnGetAsync(string videoUrl)
        {
            //if (download)
            //{
            Debug.WriteLine("Download started");
            IProgress<double> progress = new Progress<double>((p) => { Debug.WriteLine(p); });

            try
            {

                var fileInfo = await _downloader.GetAudioOnlyStream_v2(videoUrl, progress); // await _downloader.GetAudioOnlyStream(videoUrl, progress);

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
            //}
            //else
            //{
            //    return StatusCode(200);
            //}
            // Handle the case where download is false...
        }

    }
}
