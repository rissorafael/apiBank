
using BancoChu.Application.Dtos;
using BancoChu.Application.Interfaces;


namespace BancoChu.Application
{
    public class BrasilApiApplication : IBrasilApiApplication
    {
        private readonly IBrasilApiService _brasilApiService;
        public BrasilApiApplication(IBrasilApiService brasilApiService)
        {
            _brasilApiService = brasilApiService;
        }

        public async Task<List<HolidayDto>> GetHolidayAsync()
        {
            return await _brasilApiService.GetHolidayAsync();
        }
    }
}
