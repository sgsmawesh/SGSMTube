using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SGSMTube_Web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly Dictionary<string, string> _titles;
        public Dictionary<string, string> Titles { get { return _titles; } }


        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
            _titles = new Dictionary<string, string>
            {
                {"Effortless Video Downloads", "SGSMTube simplifies the process of downloading videos from the web. Just paste the video URL into our user-friendly interface, choose your preferred quality, and download with a single click." },
                {"Tailored Download Options","Customize your download experience with SGSMTube. Select the resolution and format that best suit your needs, ensuring that your videos are exactly how you want them." },
                {"Fast and Reliable Performance","Enjoy lightning-fast download speeds and reliable performance with SGSMTube. Our advanced technology ensures that your videos are downloaded quickly and securely, so you can start watching in no time." },
                {"No Ads, No Registrations","Say goodbye to annoying ads and intrusive registrations. SGSMTube is completely ad-free and does not require any registrations. Your privacy is important to us, and we believe in keeping your experience simple and hassle-free." },
                {"Privacy Guaranteed", "Rest assured that your privacy is protected with SGSMTube. We never collect or share your personal information with third parties, ensuring that your data remains safe and secure." }
            };
        }

        public void OnGet()
        {

        }
    }
}
