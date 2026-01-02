
using System.Text.Json.Serialization;

namespace BancoChu.Application.Dtos
{
    public class HolidayDto
    {
        [JsonPropertyName("date")]
        public DateTime Date { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = default!;

        [JsonPropertyName("type")]
        public string Type { get; set; } = default!;
    }
}
