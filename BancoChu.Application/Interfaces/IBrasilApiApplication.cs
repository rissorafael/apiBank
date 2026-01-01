
using BancoChu.Application.Dtos;

namespace BancoChu.Application.Interfaces
{
    public interface IBrasilApiApplication
    {
        Task<List<HolidayDto>> GetHolidayAsync();
    }
}
