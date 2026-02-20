using BodyMeasurement.Models;
using BodyMeasurement.Services;

namespace BodyMeasurement.Tests.Services;

public class DatabaseServiceTests : IDisposable
{
    private readonly DatabaseService _databaseService;
    private readonly string _testDbPath;

    public DatabaseServiceTests()
    {
        // Create a unique test database for each test run
        var folder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        _testDbPath = Path.Combine(folder, $"test_bodymeasurement_{Guid.NewGuid()}.db");
        _databaseService = new DatabaseService(_testDbPath);
    }

    public void Dispose()
    {
        // Clean up test database
        if (File.Exists(_testDbPath))
        {
            File.Delete(_testDbPath);
        }
    }

    [Fact]
    public async Task InitializeAsync_CreatesDatabase()
    {
        // Act
        await _databaseService.InitializeAsync();

        // Assert - calling again should not throw
        await _databaseService.InitializeAsync();
    }

    [Fact]
    public async Task RecordMeasurementAsync_AddsEntry()
    {
        // Arrange
        await _databaseService.InitializeAsync();
        var entry = new WeightEntry
        {
            Date = DateTime.Today,
            WeightKg = 75.5,
            Notes = "Test entry"
        };

        // Act
        var result = await _databaseService.RecordMeasurementAsync(entry);

        // Assert
        Assert.True(result > 0);
        Assert.True(entry.Id > 0);
        Assert.NotEqual(default, entry.CreatedAt);
    }

    [Fact]
    public async Task GetMeasurementHistoryAsync_ReturnsEntriesSortedByDateDescending()
    {
        // Arrange
        await _databaseService.InitializeAsync();
        var entry1 = new WeightEntry { Date = DateTime.Today.AddDays(-2), WeightKg = 75.0 };
        var entry2 = new WeightEntry { Date = DateTime.Today.AddDays(-1), WeightKg = 74.5 };
        var entry3 = new WeightEntry { Date = DateTime.Today, WeightKg = 74.0 };

        await _databaseService.RecordMeasurementAsync(entry1);
        await _databaseService.RecordMeasurementAsync(entry2);
        await _databaseService.RecordMeasurementAsync(entry3);

        // Act
        var entries = await _databaseService.GetMeasurementHistoryAsync();

        // Assert
        Assert.Equal(3, entries.Count);
        Assert.Equal(DateTime.Today, entries[0].Date);
        Assert.Equal(DateTime.Today.AddDays(-1), entries[1].Date);
        Assert.Equal(DateTime.Today.AddDays(-2), entries[2].Date);
    }

    [Fact]
    public async Task FindMeasurementAsync_ReturnsCorrectEntry()
    {
        // Arrange
        await _databaseService.InitializeAsync();
        var entry = new WeightEntry
        {
            Date = DateTime.Today,
            WeightKg = 75.5,
            Notes = "Test entry"
        };
        await _databaseService.RecordMeasurementAsync(entry);

        // Act
        var retrieved = await _databaseService.FindMeasurementAsync(entry.Id);

        // Assert
        Assert.NotNull(retrieved);
        Assert.Equal(entry.Id, retrieved.Id);
        Assert.Equal(entry.WeightKg, retrieved.WeightKg);
        Assert.Equal(entry.Notes, retrieved.Notes);
    }

    [Fact]
    public async Task FindMeasurementAsync_WithInvalidId_ReturnsNull()
    {
        // Arrange
        await _databaseService.InitializeAsync();

        // Act
        var retrieved = await _databaseService.FindMeasurementAsync(999);

        // Assert
        Assert.Null(retrieved);
    }

    [Fact]
    public async Task UpdateMeasurementAsync_UpdatesEntry()
    {
        // Arrange
        await _databaseService.InitializeAsync();
        var entry = new WeightEntry
        {
            Date = DateTime.Today,
            WeightKg = 75.5,
            Notes = "Original notes"
        };
        await _databaseService.RecordMeasurementAsync(entry);

        // Act
        entry.WeightKg = 76.0;
        entry.Notes = "Updated notes";
        await _databaseService.UpdateMeasurementAsync(entry);

        // Assert
        var updated = await _databaseService.FindMeasurementAsync(entry.Id);
        Assert.NotNull(updated);
        Assert.Equal(76.0, updated.WeightKg);
        Assert.Equal("Updated notes", updated.Notes);
    }

    [Fact]
    public async Task RemoveMeasurementAsync_RemovesEntry()
    {
        // Arrange
        await _databaseService.InitializeAsync();
        var entry = new WeightEntry
        {
            Date = DateTime.Today,
            WeightKg = 75.5
        };
        await _databaseService.RecordMeasurementAsync(entry);
        var id = entry.Id;

        // Act
        var result = await _databaseService.RemoveMeasurementAsync(id);

        // Assert
        Assert.True(result > 0);
        var deleted = await _databaseService.FindMeasurementAsync(id);
        Assert.Null(deleted);
    }

    [Fact]
    public async Task GetMeasurementsInPeriodAsync_ReturnsFilteredEntries()
    {
        // Arrange
        await _databaseService.InitializeAsync();
        var entry1 = new WeightEntry { Date = DateTime.Today.AddDays(-10), WeightKg = 75.0 };
        var entry2 = new WeightEntry { Date = DateTime.Today.AddDays(-5), WeightKg = 74.5 };
        var entry3 = new WeightEntry { Date = DateTime.Today.AddDays(-2), WeightKg = 74.0 };
        var entry4 = new WeightEntry { Date = DateTime.Today, WeightKg = 73.5 };

        await _databaseService.RecordMeasurementAsync(entry1);
        await _databaseService.RecordMeasurementAsync(entry2);
        await _databaseService.RecordMeasurementAsync(entry3);
        await _databaseService.RecordMeasurementAsync(entry4);

        // Act
        var startDate = DateTime.Today.AddDays(-7);
        var endDate = DateTime.Today.AddDays(-1);
        var entries = await _databaseService.GetMeasurementsInPeriodAsync(startDate, endDate);

        // Assert
        Assert.Equal(2, entries.Count);
        Assert.Contains(entries, e => e.Date == DateTime.Today.AddDays(-5));
        Assert.Contains(entries, e => e.Date == DateTime.Today.AddDays(-2));
    }

    [Fact]
    public async Task GetMeasurementsInPeriodAsync_WithNoMatches_ReturnsEmpty()
    {
        // Arrange
        await _databaseService.InitializeAsync();
        var entry = new WeightEntry { Date = DateTime.Today, WeightKg = 75.0 };
        await _databaseService.RecordMeasurementAsync(entry);

        // Act
        var startDate = DateTime.Today.AddDays(-30);
        var endDate = DateTime.Today.AddDays(-10);
        var entries = await _databaseService.GetMeasurementsInPeriodAsync(startDate, endDate);

        // Assert
        Assert.Empty(entries);
    }
}
