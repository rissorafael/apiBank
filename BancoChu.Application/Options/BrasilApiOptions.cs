using System.ComponentModel.DataAnnotations;

namespace BancoChu.Application.Options
{
    public class BrasilApiOptions
    {
        public const string Section = "BrasilApi";

        [Required, Url]
        public string BaseUrl { get; set; }
    }
}
