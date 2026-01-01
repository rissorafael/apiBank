using ApiBancoChu.Infrastructure.EndPoints;
using BancoChu.Application.Dtos;
using BancoChu.Application.Interfaces;
using BancoChu.Application.Options;
using Microsoft.Extensions.Options;
using System.Text.Json;


namespace ApiBancoChu.Infrastructure.ExternalServices
{
    public class BrasilApiService : IBrasilApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IOptions<BrasilApiOptions> _settings;

        public BrasilApiService(HttpClient httpClient, IOptions<BrasilApiOptions> settings)
        {
            _httpClient = httpClient;
            _settings = settings;
            _httpClient.BaseAddress = new Uri(_settings.Value.BaseUrl);
        }

        public async Task<List<HolidayDto>> GetHolidayAsync()
        {
            var url = string.Format(BrasilApiEndPoints.Getferiados2025);

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var resp = await response.Content.ReadAsStringAsync();

            var holidays = JsonSerializer.Deserialize<List<HolidayDto>>(resp);

            return holidays;
        }
    }
}

