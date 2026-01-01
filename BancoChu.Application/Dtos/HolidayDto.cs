
namespace BancoChu.Application.Dtos
{
    public class HolidayDto
    {
        public DateTime Date { get; set; }
        public string Name { get; set; } = default!;
        public string Type { get; set; } = default!;
    }
}
