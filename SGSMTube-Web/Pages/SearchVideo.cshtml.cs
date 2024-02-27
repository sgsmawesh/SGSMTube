using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using SGSMTube_Lib.Models;
using SGSMTube_Lib.Views;
using SGSMTube_Web.Models;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SGSMTube_Web.Pages
{
    public class SearchVideoModel(YTDownloader downloader) : PageModel
    {
        [BindProperty]
        [Required(ErrorMessage = "What would you like to search")]
        public string Keyword { get; set; } = string.Empty;

        [BindProperty]
        public IEnumerable<VideoDetailModel>? Results { get; set; }

        private readonly YTDownloader _downloader = downloader;

        public void OnGet()
        {
            var searchRes = HttpContext.Session.GetString("searchResult");
            if (!string.IsNullOrEmpty(searchRes))
            {
                Results = JsonConvert.DeserializeObject<IEnumerable<VideoDetailModel>>(searchRes);
            }
            else
            {
                Results = [];
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            var searchResult = await _downloader.SearchVideo(Keyword);

            HttpContext.Session.SetString("searchResult", JsonConvert.SerializeObject(searchResult));
            return RedirectToPage();
        }
    }
}
