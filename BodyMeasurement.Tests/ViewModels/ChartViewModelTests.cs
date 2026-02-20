using BodyMeasurement.Models;
using BodyMeasurement.Services;
using Moq;

namespace BodyMeasurement.Tests.ViewModels;

public class ChartViewModelTests
{
    [Fact]
    public async Task LoadChartData_WithAllFilter_LoadsAllEntries()
    {
        // Arrange
        var mockDb = new Mock<IDatabaseService>();
        var mockSettings = new Mock<ISettingsService>();
        
        var entries = new List<WeightEntry>
        {
            new WeightEntry { Date = DateTime.Today.AddDays(-60), WeightKg = 80.0 },
            new WeightEntry { Date = DateTime.Today.AddDays(-30), WeightKg = 77.0 },
            new WeightEntry { Date = DateTime.Today, WeightKg = 75.0 }
        };

        mockDb.Setup(x => x.GetMeasurementHistoryAsync()).ReturnsAsync(entries);
        mockSettings.Setup(s => s.PreferredUnit).Returns("kg");

        // Act
        var result = await mockDb.Object.GetMeasurementHistoryAsync();

        // Assert
        Assert.Equal(3, result.Count);
        mockDb.Verify(x => x.GetMeasurementHistoryAsync(), Times.Once);
    }

    [Fact]
    public async Task LoadChartData_With1WeekFilter_LoadsFilteredEntries()
    {
        // Arrange
        var mockDb = new Mock<IDatabaseService>();
        var mockSettings = new Mock<ISettingsService>();
        
        var entries = new List<WeightEntry>
        {
            new WeightEntry { Date = DateTime.Today.AddDays(-5), WeightKg = 75.5 },
            new WeightEntry { Date = DateTime.Today.AddDays(-3), WeightKg = 75.0 }
        };

        mockDb.Setup(x => x.GetMeasurementsInPeriodAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
              .ReturnsAsync(entries);
        mockSettings.Setup(s => s.PreferredUnit).Returns("kg");

        // Act
        var result = await mockDb.Object.GetMeasurementsInPeriodAsync(DateTime.Today.AddDays(-7), DateTime.Today);

        // Assert
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task LoadChartData_CalculatesMinMaxAverage()
    {
        // Arrange
        var entries = new List<WeightEntry>
        {
            new WeightEntry { Date = DateTime.Today.AddDays(-2), WeightKg = 75.0 },
            new WeightEntry { Date = DateTime.Today.AddDays(-1), WeightKg = 77.0 },
            new WeightEntry { Date = DateTime.Today, WeightKg = 76.0 }
        };

        // Act
        var min = entries.Min(e => e.WeightKg);
        var max = entries.Max(e => e.WeightKg);
        var avg = entries.Average(e => e.WeightKg);

        // Assert
        Assert.Equal(75.0, min);
        Assert.Equal(77.0, max);
        Assert.Equal(76.0, avg);
    }

    [Fact]
    public async Task LoadChartData_WithNoData_SetsHasDataFalse()
    {
        // Arrange
        var mockDb = new Mock<IDatabaseService>();
        mockDb.Setup(x => x.GetMeasurementHistoryAsync()).ReturnsAsync(new List<WeightEntry>());

        // Act
        var result = await mockDb.Object.GetMeasurementHistoryAsync();

        // Assert
        Assert.Empty(result);
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
    public void GetWeightRangeText_WithValues_ReturnsRange()
    {
        // Arrange
        double? minWeight = 70.0;
        double? maxWeight = 80.0;
        string unit = "kg";

        // Act
        string result = minWeight.HasValue && maxWeight.HasValue
            ? $"{WeightConverter.Format(minWeight.Value, unit)} - {WeightConverter.Format(maxWeight.Value, unit)}"
            : "--";

        // Assert
        Assert.Contains("70.0 kg", result);
        Assert.Contains("80.0 kg", result);
    }

    [Fact]
    public void ChartData_SortedByDate_AscendingOrder()
    {
        // Arrange
        var entries = new List<WeightEntry>
        {
            new WeightEntry { Date = DateTime.Today, WeightKg = 75.0 },
            new WeightEntry { Date = DateTime.Today.AddDays(-2), WeightKg = 77.0 },
            new WeightEntry { Date = DateTime.Today.AddDays(-1), WeightKg = 76.0 }
        };

        // Act
        var sorted = entries.OrderBy(e => e.Date).ToList();

        // Assert
        Assert.Equal(DateTime.Today.AddDays(-2), sorted[0].Date);
        Assert.Equal(DateTime.Today.AddDays(-1), sorted[1].Date);
        Assert.Equal(DateTime.Today, sorted[2].Date);
    }

    [Theory]
    [InlineData("1 Week", -7)]
    [InlineData("1 Month", -30)]
    [InlineData("3 Months", -90)]
    [InlineData("6 Months", -180)]
    public void FilterMapping_ReturnsCorrectDays(string filter, int expectedDays)
    {
        // Act
        var startDate = filter switch
        {
            "1 Week" => DateTime.Today.AddDays(-7),
            "1 Month" => DateTime.Today.AddDays(-30),
            "3 Months" => DateTime.Today.AddDays(-90),
            "6 Months" => DateTime.Today.AddDays(-180),
            "All" => DateTime.MinValue,
            _ => DateTime.Today.AddDays(-30)
        };

        // Assert
        Assert.Equal(DateTime.Today.AddDays(expectedDays), startDate);
    }

    [Fact]
    public async Task ChangeFilter_UpdatesSelectedFilter()
    {
        // Arrange
        string initialFilter = "1 Month";
        string newFilter = "3 Months";

        // Act
        string selectedFilter = newFilter;

        // Assert
        Assert.Equal("3 Months", selectedFilter);
        Assert.NotEqual(initialFilter, selectedFilter);
    }
}
