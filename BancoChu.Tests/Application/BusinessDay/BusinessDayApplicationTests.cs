using BancoChu.Application;
using BancoChu.Application.Dtos;
using BancoChu.Application.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using System.Text.Json;

namespace BancoChu.Tests.Application.BusinessDay
{
    public class BusinessDayApplicationTests
    {
        private readonly Mock<IBrasilApiService> _brasilApiMock;
        private readonly Mock<IDistributedCache> _cacheMock;
        private readonly BusinessDayApplication _application;

        public BusinessDayApplicationTests()
        {
            _brasilApiMock = new Mock<IBrasilApiService>();
            _cacheMock = new Mock<IDistributedCache>();

            _application = new BusinessDayApplication(
                _brasilApiMock.Object,
                _cacheMock.Object
            );
        }

        [Fact]
        public async Task IsBusinessDayAsync_WhenSaturday_ShouldReturnFalse()
        {
            // Arrange
            var saturday = new DateTime(2025, 1, 4); // sábado

            // Act
            var result = await _application.IsBusinessDayAsync(saturday);

            // Assert
            Assert.False(result);
        }


        [Fact]
        public async Task IsBusinessDayAsync_WhenHoliday_ShouldReturnFalse()
        {
            // Arrange
            var date = new DateTime(2025, 1, 1);

            _cacheMock
                .Setup(c => c.GetStringAsync("holidays:2025", It.IsAny<CancellationToken>()))
                .ReturnsAsync((string?)null);

            _brasilApiMock
                .Setup(b => b.GetHolidayAsync(2025))
                .ReturnsAsync(new List<HolidayDto>
                {
            new HolidayDto { Date = date }
                });

            // Act
            var result = await _application.IsBusinessDayAsync(date);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task IsBusinessDayAsync_WhenCacheExists_ShouldNotCallApi()
        {
            // Arrange
            var date = new DateTime(2025, 1, 2);

            var cachedHolidays = JsonSerializer.Serialize(new List<HolidayDto>());

            _cacheMock
                .Setup(c => c.GetStringAsync("holidays:2025", It.IsAny<CancellationToken>()))
                .ReturnsAsync(cachedHolidays);

            // Act
            var result = await _application.IsBusinessDayAsync(date);

            // Assert
            Assert.True(result);
            _brasilApiMock.Verify(b => b.GetHolidayAsync(It.IsAny<int>()), Times.Never);
        }


    }
}
