using BancoChu.Application.Dtos;

namespace BancoChu.Application.Interfaces
{
    public interface IBrasilApiService
    {
        Task<List<HolidayDto>> GetHolidayAsync();
    }
}
