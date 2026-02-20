using BodyMeasurement.Models;
using BodyMeasurement.Services;
using Moq;

namespace BodyMeasurement.Tests.Services;

public class StatisticsServiceTests
{
    [Fact]
    public async Task GetCurrentWeightAsync_WithEntries_ReturnsLatestWeight()
    {
        // Arrange
        var mockDb = new Mock<IDatabaseService>();
        var entries = new List<WeightEntry>
        {
            new WeightEntry { Date = DateTime.Today, WeightKg = 73.5 }, // Most recent first (sorted descending)
            new WeightEntry { Date = DateTime.Today.AddDays(-2), WeightKg = 75.0 }
        };
        mockDb.Setup(x => x.GetMeasurementHistoryAsync()).ReturnsAsync(entries);

        var service = new StatisticsService(mockDb.Object);

        // Act
        var result = await service.GetCurrentWeightAsync();

        // Assert
        Assert.Equal(73.5, result);
    }

    [Fact]
    public async Task GetCurrentWeightAsync_WithNoEntries_ReturnsNull()
    {
        // Arrange
        var mockDb = new Mock<IDatabaseService>();
        mockDb.Setup(x => x.GetMeasurementHistoryAsync()).ReturnsAsync(new List<WeightEntry>());

        var service = new StatisticsService(mockDb.Object);

        // Act
        var result = await service.GetCurrentWeightAsync();

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetStartingWeightAsync_WithEntries_ReturnsEarliestWeight()
    {
        // Arrange
        var mockDb = new Mock<IDatabaseService>();
        var entries = new List<WeightEntry>
        {
            new WeightEntry { Date = DateTime.Today, WeightKg = 73.5 },
            new WeightEntry { Date = DateTime.Today.AddDays(-2), WeightKg = 75.0 } // Oldest last
        };
        mockDb.Setup(x => x.GetMeasurementHistoryAsync()).ReturnsAsync(entries);

        var service = new StatisticsService(mockDb.Object);

        // Act
        var result = await service.GetStartingWeightAsync();

        // Assert
        Assert.Equal(75.0, result);
    }

    [Fact]
    public async Task GetStartingWeightAsync_WithNoEntries_ReturnsNull()
    {
        // Arrange
        var mockDb = new Mock<IDatabaseService>();
        mockDb.Setup(x => x.GetMeasurementHistoryAsync()).ReturnsAsync(new List<WeightEntry>());

        var service = new StatisticsService(mockDb.Object);

        // Act
        var result = await service.GetStartingWeightAsync();

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CalculateWeightChangeAsync_WithWeightLoss_ReturnsNegativeChange()
    {
        // Arrange
        var mockDb = new Mock<IDatabaseService>();
        var entries = new List<WeightEntry>
        {
            new WeightEntry { Date = DateTime.Today, WeightKg = 70.0 },      // Current (first in sorted list)
            new WeightEntry { Date = DateTime.Today.AddDays(-30), WeightKg = 75.0 }  // Starting (last)
        };
        mockDb.Setup(x => x.GetMeasurementHistoryAsync()).ReturnsAsync(entries);

        var service = new StatisticsService(mockDb.Object);

        // Act
        var (absolute, percentage) = await service.CalculateWeightChangeAsync();

        // Assert
        Assert.NotNull(absolute);
        Assert.NotNull(percentage);
        Assert.Equal(-5.0, absolute.Value, 2);
        Assert.Equal(-6.67, percentage.Value, 2); // (70-75)/75 * 100 = -6.67%
    }

    [Fact]
    public async Task CalculateWeightChangeAsync_WithWeightGain_ReturnsPositiveChange()
    {
        // Arrange
        var mockDb = new Mock<IDatabaseService>();
        var entries = new List<WeightEntry>
        {
            new WeightEntry { Date = DateTime.Today, WeightKg = 77.0 },
            new WeightEntry { Date = DateTime.Today.AddDays(-30), WeightKg = 75.0 }
        };
        mockDb.Setup(x => x.GetMeasurementHistoryAsync()).ReturnsAsync(entries);

        var service = new StatisticsService(mockDb.Object);

        // Act
        var (absolute, percentage) = await service.CalculateWeightChangeAsync();

        // Assert
        Assert.NotNull(absolute);
        Assert.NotNull(percentage);
        Assert.Equal(2.0, absolute.Value, 2);
        Assert.Equal(2.67, percentage.Value, 2); // (77-75)/75 * 100 = 2.67%
    }

    [Fact]
    public async Task CalculateWeightChangeForPeriodAsync_WithValidPeriod_ReturnsChange()
    {
        // Arrange
        var mockDb = new Mock<IDatabaseService>();
        var entries = new List<WeightEntry>
        {
            new WeightEntry { Date = DateTime.Today.AddDays(-5), WeightKg = 73.0 },
            new WeightEntry { Date = DateTime.Today.AddDays(-2), WeightKg = 72.0 }
        };
        mockDb.Setup(x => x.GetMeasurementsInPeriodAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
              .ReturnsAsync(entries);

        var service = new StatisticsService(mockDb.Object);

        // Act
        var (absolute, percentage) = await service.CalculateWeightChangeForPeriodAsync(7);

        // Assert
        Assert.NotNull(absolute);
        Assert.NotNull(percentage);
        Assert.Equal(-1.0, absolute.Value, 2); // 72 - 73 = -1
        Assert.Equal(-1.37, percentage.Value, 2); // -1/73 * 100 = -1.37%
    }

    [Fact]
    public async Task CalculateWeightChangeForPeriodAsync_WithNoDataInPeriod_ReturnsNull()
    {
        // Arrange
        var mockDb = new Mock<IDatabaseService>();
        mockDb.Setup(x => x.GetMeasurementsInPeriodAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
              .ReturnsAsync(new List<WeightEntry>());

        var service = new StatisticsService(mockDb.Object);

        // Act
        var (absolute, percentage) = await service.CalculateWeightChangeForPeriodAsync(7);

        // Assert
        Assert.Null(absolute);
        Assert.Null(percentage);
    }

    [Fact]
    public async Task GetStatisticsAsync_WithEntries_ReturnsCompleteStatistics()
    {
        // Arrange
        var mockDb = new Mock<IDatabaseService>();
        var entries = new List<WeightEntry>
        {
            new WeightEntry { Date = DateTime.Today.AddDays(-30), WeightKg = 80.0 },
            new WeightEntry { Date = DateTime.Today.AddDays(-20), WeightKg = 78.0 },
            new WeightEntry { Date = DateTime.Today.AddDays(-10), WeightKg = 76.0 },
            new WeightEntry { Date = DateTime.Today, WeightKg = 75.0 }
        };
        mockDb.Setup(x => x.GetMeasurementHistoryAsync()).ReturnsAsync(entries);

        var service = new StatisticsService(mockDb.Object);

        // Act
        var stats = await service.GetStatisticsAsync();

        // Assert
        Assert.Equal(4, stats.TotalMeasurements);
        Assert.Equal(75.0, stats.CurrentWeightKg);
        Assert.Equal(80.0, stats.StartingWeightKg);
        Assert.Equal(-5.0, stats.WeightChangeKg);
        Assert.Equal(-6.25, stats.WeightChangePercentage); // -5/80 * 100 = -6.25%
        Assert.NotNull(stats.FirstMeasurementDate);
        Assert.NotNull(stats.LastMeasurementDate);
    }

    [Fact]
    public async Task GetStatisticsAsync_WithNoEntries_ReturnsEmptyStatistics()
    {
        // Arrange
        var mockDb = new Mock<IDatabaseService>();
        mockDb.Setup(x => x.GetMeasurementHistoryAsync()).ReturnsAsync(new List<WeightEntry>());

        var service = new StatisticsService(mockDb.Object);

        // Act
        var stats = await service.GetStatisticsAsync();

        // Assert
        Assert.Equal(0, stats.TotalMeasurements);
        Assert.Null(stats.CurrentWeightKg);
        Assert.Null(stats.StartingWeightKg);
    }

    [Fact]
    public async Task GetStatisticsAsync_WithPeriod_ReturnsFilteredStatistics()
    {
        // Arrange
        var mockDb = new Mock<IDatabaseService>();
        var entries = new List<WeightEntry>
        {
            new WeightEntry { Date = DateTime.Today.AddDays(-5), WeightKg = 76.0 },
            new WeightEntry { Date = DateTime.Today, WeightKg = 75.0 }
        };
        mockDb.Setup(x => x.GetMeasurementsInPeriodAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
              .ReturnsAsync(entries);

        var service = new StatisticsService(mockDb.Object);

        // Act
        var stats = await service.GetStatisticsAsync(7);

        // Assert
        Assert.Equal(2, stats.TotalMeasurements);
        Assert.Equal(75.0, stats.CurrentWeightKg);
        Assert.Equal(76.0, stats.StartingWeightKg);
    }
}
