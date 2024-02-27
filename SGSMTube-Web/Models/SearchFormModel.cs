using System.ComponentModel.DataAnnotations;

namespace SGSMTube_Web.Models
{
    public class SearchFormModel
    {
        [Required(ErrorMessage = "Please provide search text", AllowEmptyStrings = false)]
        public string Keyword { get; set; }
    }
}
