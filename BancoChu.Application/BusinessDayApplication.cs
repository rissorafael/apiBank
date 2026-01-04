using BancoChu.Application.Dtos;
using BancoChu.Application.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace BancoChu.Application
{
    public class BusinessDayApplication : IBusinessDayApplication
    {
        private readonly IBrasilApiService _brasilApiService;
        private readonly IDistributedCache _cache;

        public BusinessDayApplication(IBrasilApiService brasilApiService, IDistributedCache cache)
        {
            _brasilApiService = brasilApiService;
            _cache = cache;
        }
        public async Task<bool> IsBusinessDayAsync(DateTime date)
        {
            if (date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
                return false;

            var holidays = await GetHolidaysByYearAsync(date.Year);

            var isHoliday = holidays.Any(h => h.Date.Date == date.Date);

            return !isHoliday;
        }

        private async Task<List<HolidayDto>> GetHolidaysByYearAsync(int year)
        {
            var cacheKey = $"holidays:{year}";

            var cached = await _cache.GetStringAsync(cacheKey);

            if (!string.IsNullOrEmpty(cached))
            {
                return JsonSerializer.Deserialize<List<HolidayDto>>(cached)!;
            }

            var holidays = await _brasilApiService.GetHolidayAsync(year);

            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24)
            };

            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(holidays), options);

            return holidays;
        }
    }
}
