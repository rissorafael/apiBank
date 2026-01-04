using BancoChu.Application;
using BancoChu.Application.Dtos;
using BancoChu.Application.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace BancoChu.Tests.Application.BusinessDay
{
    public class BusinessDayApplicationTests
    {
        private readonly Mock<IBrasilApiService> _brasilApiServiceMock;
        private readonly Mock<IDistributedCache> _cacheMock;
        private readonly BusinessDayApplication _sut; // System Under Test

        public BusinessDayApplicationTests()
        {
            _brasilApiServiceMock = new Mock<IBrasilApiService>();
            _cacheMock = new Mock<IDistributedCache>();
            _sut = new BusinessDayApplication(_brasilApiServiceMock.Object, _cacheMock.Object);
        }

        [Fact]
        public async Task IsBusinessDayAsync_ShouldReturnFalse_WhenSaturday()
        {
            var saturday = new DateTime(2026, 1, 3); // sábado
            var result = await _sut.IsBusinessDayAsync(saturday);

            Assert.False(result);
        }

        [Fact]
        public async Task IsBusinessDayAsync_ShouldReturnFalse_WhenSunday()
        {
            var sunday = new DateTime(2026, 1, 4); // domingo
            var result = await _sut.IsBusinessDayAsync(sunday);

            Assert.False(result);
        }

        [Fact]
        public async Task IsBusinessDayAsync_ShouldReturnFalse_WhenHoliday()
        {
            var holiday = new DateTime(2026, 1, 1); // Confraternização mundial

            // Cache deve retornar null para cair na chamada da API
            _cacheMock
                .Setup(c => c.GetAsync(It.IsAny<string>(), default))
                .ReturnsAsync((byte[]?)null);

            _brasilApiServiceMock
                .Setup(s => s.GetHolidayAsync(2026))
                .ReturnsAsync(new List<HolidayDto>
                {
            new HolidayDto { Date = holiday, Name = "Confraternização mundial", Type = "national" }
                });

            var result = await _sut.IsBusinessDayAsync(holiday);

            Assert.False(result);
        }


        [Fact]
        public async Task IsBusinessDayAsync_ShouldReturnTrue_WhenNormalBusinessDay()
        {
            var normalDay = new DateTime(2026, 1, 2); // sexta-feira

            // Cache deve retornar null para cair na chamada da API
            _cacheMock
                .Setup(c => c.GetAsync(It.IsAny<string>(), default))
                .ReturnsAsync((byte[]?)null);

            _brasilApiServiceMock
                .Setup(s => s.GetHolidayAsync(2026))
                .ReturnsAsync(new List<HolidayDto>()); // sem feriados

            var result = await _sut.IsBusinessDayAsync(normalDay);

            Assert.True(result);
        }


        [Fact]
        public async Task GetHolidaysByYearAsync_ShouldUseCache_WhenAvailable()
        {
            var year = 2026;
            var cacheKey = $"holidays:{year}";
            var holidays = new List<HolidayDto>
            {
                new HolidayDto { Date = new DateTime(2026, 1, 1), Name = "Confraternização mundial", Type = "national" }
            };

            var cachedJson = JsonSerializer.Serialize(holidays);
            var cachedBytes = Encoding.UTF8.GetBytes(cachedJson);

            _cacheMock
                .Setup(c => c.GetAsync(cacheKey, default))
                .ReturnsAsync(cachedBytes);

            var result = await _sut.IsBusinessDayAsync(new DateTime(2026, 1, 1));

            Assert.False(result); // porque é feriado
            _brasilApiServiceMock.Verify(s => s.GetHolidayAsync(It.IsAny<int>()), Times.Never);
        }
    }
}
