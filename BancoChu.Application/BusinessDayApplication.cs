using BancoChu.Application.Dtos;
using BancoChu.Application.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using System.Text;
using System.Text.Json;

namespace BancoChu.Application
{
    public class BusinessDayApplication : IBusinessDayApplication
    {
        private readonly IBrasilApiService _brasilApiService;
        private readonly IDistributedCache _cache;

        public BusinessDayApplication(
            IBrasilApiService brasilApiService,
            IDistributedCache cache)
        {
            _brasilApiService = brasilApiService;
            _cache = cache;
        }

        public async Task<bool> IsBusinessDayAsync(DateTime date)
        {
            if (date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
                return false;

            var holidays = await GetHolidaysByYearAsync(date.Year);

            var isholidays = holidays.Any(h => h.Date.Date == date.Date);

            return !isholidays;
        }

        private async Task<List<HolidayDto>> GetHolidaysByYearAsync(int year)
        {
            var cacheKey = $"holidays:{year}";

            var cachedBytes = await _cache.GetAsync(cacheKey);

            if (cachedBytes != null)
            {
                var json = Encoding.UTF8.GetString(cachedBytes);
                return JsonSerializer.Deserialize<List<HolidayDto>>(json)!;
            }

            var holidays = await _brasilApiService.GetHolidayAsync(year);

            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24)
            };

            await _cache.SetAsync(
                cacheKey,
                Encoding.UTF8.GetBytes(JsonSerializer.Serialize(holidays)),
                options
            );

            return holidays;
        }
    }
}
