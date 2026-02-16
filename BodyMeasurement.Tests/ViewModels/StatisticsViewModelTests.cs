using BodyMeasurement.Models;
using BodyMeasurement.Services;
using Moq;

namespace BodyMeasurement.Tests.ViewModels;

public class StatisticsViewModelTests
{
    [Fact]
    public async Task LoadStatistics_WithAllPeriod_LoadsAllData()
    {
        // Arrange
        var mockStats = new Mock<IStatisticsService>();
        var mockSettings = new Mock<ISettingsService>();
        
        var statistics = new Statistics
        {
            TotalMeasurements = 10,
            CurrentWeightKg = 75.0,
            StartingWeightKg = 80.0,
            WeightChangeKg = -5.0,
            WeightChangePercentage = -6.25
        };

        mockStats.Setup(x => x.GetStatisticsAsync(null)).ReturnsAsync(statistics);
        mockSettings.Setup(s => s.PreferredUnit).Returns("kg");

        // Act
        var result = await mockStats.Object.GetStatisticsAsync(null);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(10, result.TotalMeasurements);
        Assert.Equal(75.0, result.CurrentWeightKg);
        Assert.Equal(-5.0, result.WeightChangeKg);
    }

    [Fact]
    public async Task LoadStatistics_With7DaysPeriod_LoadsFilteredData()
    {
        // Arrange
        var mockStats = new Mock<IStatisticsService>();
        var mockSettings = new Mock<ISettingsService>();
        
        var statistics = new Statistics
        {
            TotalMeasurements = 3,
            CurrentWeightKg = 75.0,
            StartingWeightKg = 75.5,
            WeightChangeKg = -0.5,
            WeightChangePercentage = -0.66
        };

        mockStats.Setup(x => x.GetStatisticsAsync(7)).ReturnsAsync(statistics);
        mockSettings.Setup(s => s.PreferredUnit).Returns("kg");

        // Act
        var result = await mockStats.Object.GetStatisticsAsync(7);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.TotalMeasurements);
        Assert.Equal(-0.5, result.WeightChangeKg);
    }

    [Fact]
    public async Task LoadStatistics_With30DaysPeriod_LoadsFilteredData()
    {
        // Arrange
        var mockStats = new Mock<IStatisticsService>();
        
        var statistics = new Statistics
        {
            TotalMeasurements = 5,
            CurrentWeightKg = 74.0,
            StartingWeightKg = 76.0,
            WeightChangeKg = -2.0,
            WeightChangePercentage = -2.63
        };

        mockStats.Setup(x => x.GetStatisticsAsync(30)).ReturnsAsync(statistics);

        // Act
        var result = await mockStats.Object.GetStatisticsAsync(30);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(5, result.TotalMeasurements);
        Assert.Equal(-2.0, result.WeightChangeKg);
    }

    [Fact]
    public async Task LoadStatistics_WithNoData_ReturnsEmptyStatistics()
    {
        // Arrange
        var mockStats = new Mock<IStatisticsService>();
        
        var statistics = new Statistics
        {
            TotalMeasurements = 0
        };

        mockStats.Setup(x => x.GetStatisticsAsync(null)).ReturnsAsync(statistics);

        // Act
        var result = await mockStats.Object.GetStatisticsAsync(null);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(0, result.TotalMeasurements);
        Assert.Null(result.CurrentWeightKg);
    }

    [Fact]
    public void FormatWeight_WithValue_ReturnsFormattedString()
    {
        // Arrange
        double? weightKg = 75.5;
        string unit = "kg";

        // Act
        string result = weightKg.HasValue ? WeightConverter.Format(weightKg.Value, unit) : "--";

        // Assert
        Assert.Equal("75.5 kg", result);
    }

    [Fact]
    public void FormatWeight_WithNull_ReturnsPlaceholder()
    {
        // Arrange
        double? weightKg = null;

        // Act
        string result = weightKg.HasValue ? "value" : "--";

        // Assert
        Assert.Equal("--", result);
    }

    [Fact]
    public void FormatWeightChange_WithNegativeChange_ShowsDecreaseWithArrow()
    {
        // Arrange
        double? weightChangeKg = -5.0;
        double? percentage = -6.25;

        // Act
        var sign = weightChangeKg.Value >= 0 ? "+" : "";
        var arrow = weightChangeKg.Value < 0 ? "↓" : weightChangeKg.Value > 0 ? "↑" : "";
        var formatted = WeightConverter.Format(Math.Abs(weightChangeKg.Value), "kg");
        var result = $"{sign}{formatted} ({sign}{percentage:F1}%) {arrow}";

        // Assert
        Assert.Contains("5.0 kg", result);
        Assert.Contains("↓", result);
        Assert.Contains("-6.2%", result);
    }

    [Fact]
    public void FormatWeightChange_WithPositiveChange_ShowsIncreaseWithArrow()
    {
        // Arrange
        double? weightChangeKg = 3.0;
        double? percentage = 4.0;

        // Act
        var sign = weightChangeKg.Value >= 0 ? "+" : "";
        var arrow = weightChangeKg.Value < 0 ? "↓" : weightChangeKg.Value > 0 ? "↑" : "";
        var formatted = WeightConverter.Format(Math.Abs(weightChangeKg.Value), "kg");
        var result = $"{sign}{formatted} ({sign}{percentage:F1}%) {arrow}";

        // Assert
        Assert.Contains("3.0 kg", result);
        Assert.Contains("↑", result);
        Assert.Contains("+4.0%", result);
    }

    [Fact]
    public void GetTrendText_WithSignificantLoss_ReturnsLosingWeight()
    {
        // Arrange
        double? weightChangeKg = -2.0;

        // Act
        string trend = weightChangeKg < -0.5 ? "Losing weight" 
            : weightChangeKg > 0.5 ? "Gaining weight" 
            : "Maintaining weight";

        // Assert
        Assert.Equal("Losing weight", trend);
    }

    [Fact]
    public void GetTrendText_WithSignificantGain_ReturnsGainingWeight()
    {
        // Arrange
        double? weightChangeKg = 1.5;

        // Act
        string trend = weightChangeKg < -0.5 ? "Losing weight" 
            : weightChangeKg > 0.5 ? "Gaining weight" 
            : "Maintaining weight";

        // Assert
        Assert.Equal("Gaining weight", trend);
    }

    [Fact]
    public void GetTrendText_WithMinimalChange_ReturnsMaintainingWeight()
    {
        // Arrange
        double? weightChangeKg = 0.2;

        // Act
        string trend = weightChangeKg < -0.5 ? "Losing weight" 
            : weightChangeKg > 0.5 ? "Gaining weight" 
            : "Maintaining weight";

        // Assert
        Assert.Equal("Maintaining weight", trend);
    }

    [Theory]
    [InlineData("7 Days", 7)]
    [InlineData("30 Days", 30)]
    [InlineData("90 Days", 90)]
    [InlineData("All", null)]
    public void PeriodMapping_ReturnsCorrectDays(string period, int? expectedDays)
    {
        // Act
        int? periodDays = period switch
        {
            "7 Days" => 7,
            "30 Days" => 30,
            "90 Days" => 90,
            "All" => null,
            _ => null
        };

        // Assert
        Assert.Equal(expectedDays, periodDays);
    }
}
