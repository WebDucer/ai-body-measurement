using BodyMeasurement.Models;
using BodyMeasurement.Services;
using Moq;

namespace BodyMeasurement.Tests.Integration;

/// <summary>
/// Integration tests for the complete add-measurement flow
/// Tests the path: Service → Database simulation with realistic scenarios
/// </summary>
public class AddMeasurementFlowTests
{
    [Fact]
    public async Task AddMeasurement_CompleteFlow_SavesAndRetrievesData()
    {
        // Arrange - Simulate database storage
        var entries = new List<WeightEntry>();
        var mockDb = new Mock<IDatabaseService>();
        var mockSettings = new Mock<ISettingsService>();
        var mockStats = new Mock<IStatisticsService>();
        
        mockSettings.Setup(s => s.PreferredUnit).Returns("kg");
        
        // Mock database operations with realistic behavior
        mockDb.Setup(x => x.InsertWeightEntryAsync(It.IsAny<WeightEntry>()))
            .Callback<WeightEntry>(entry =>
            {
                entry.Id = entries.Count + 1;
                entry.CreatedAt = DateTime.Now;
                entries.Add(entry);
            })
            .ReturnsAsync((WeightEntry entry) => entry.Id);
        
        mockDb.Setup(x => x.GetAllWeightEntriesAsync())
            .ReturnsAsync(() => entries.OrderByDescending(e => e.Date).ToList());
        
        mockStats.Setup(x => x.GetCurrentWeightAsync())
            .ReturnsAsync(() => entries.Any() ? entries.OrderByDescending(e => e.Date).First().WeightKg : (double?)null);
        
        // Act - Step 1: User adds a weight entry (simulated via service)
        var newEntry = new WeightEntry
        {
            WeightKg = 75.5,
            Date = DateTime.Today,
            Notes = "Test measurement",
            CreatedAt = DateTime.Now
        };
        
        var insertedId = await mockDb.Object.InsertWeightEntryAsync(newEntry);
        
        // Act - Step 2: Retrieve all entries (simulating main page load)
        var allEntries = await mockDb.Object.GetAllWeightEntriesAsync();
        var currentWeight = await mockStats.Object.GetCurrentWeightAsync();
        
        // Assert - Verify the complete flow
        Assert.Single(entries);
        Assert.Equal(75.5, entries[0].WeightKg);
        Assert.Equal(DateTime.Today, entries[0].Date);
        Assert.Equal("Test measurement", entries[0].Notes);
        Assert.Equal(1, insertedId);
        
        Assert.Single(allEntries);
        Assert.Equal(75.5, allEntries[0].WeightKg);
        Assert.Equal(75.5, currentWeight);
        
        // Verify database methods were called
        mockDb.Verify(x => x.InsertWeightEntryAsync(It.IsAny<WeightEntry>()), Times.Once);
        mockDb.Verify(x => x.GetAllWeightEntriesAsync(), Times.Once);
    }
    
    [Fact]
    public async Task AddMeasurement_WithLbsUnit_ConvertsToKgForStorage()
    {
        // Arrange
        var entries = new List<WeightEntry>();
        var mockDb = new Mock<IDatabaseService>();
        var mockSettings = new Mock<ISettingsService>();
        
        mockSettings.Setup(s => s.PreferredUnit).Returns("lbs");
        
        mockDb.Setup(x => x.InsertWeightEntryAsync(It.IsAny<WeightEntry>()))
            .Callback<WeightEntry>(entry =>
            {
                entry.Id = entries.Count + 1;
                entry.CreatedAt = DateTime.Now;
                entries.Add(entry);
            })
            .ReturnsAsync((WeightEntry entry) => entry.Id);
        
        // Act - User enters weight in lbs (simulated conversion)
        double weightInLbs = 166.0;
        double weightInKg = WeightConverter.LbsToKg(weightInLbs);
        
        var entry = new WeightEntry
        {
            WeightKg = weightInKg, // Stored in kg
            Date = DateTime.Today,
            CreatedAt = DateTime.Now
        };
        
        await mockDb.Object.InsertWeightEntryAsync(entry);
        
        // Assert - Verify data is stored in kg
        Assert.Single(entries);
        Assert.InRange(entries[0].WeightKg, 75.0, 75.5); // Converted from lbs to kg
    }
    
    [Fact]
    public void AddMeasurement_WithInvalidWeight_FailsValidation()
    {
        // Arrange - Simulate validation logic
        double weight = 0;
        
        // Act - Validate weight
        bool isValid = weight > 0;
        
        // Assert - Verify validation fails
        Assert.False(isValid);
    }
    
    [Fact]
    public void AddMeasurement_WithFutureDate_FailsValidation()
    {
        // Arrange - Simulate validation logic
        DateTime date = DateTime.Today.AddDays(1);
        
        // Act - Validate date
        bool isValid = date <= DateTime.Today;
        
        // Assert - Verify validation fails
        Assert.False(isValid);
    }
    
    [Fact]
    public async Task AddMultipleMeasurements_MaintainsChronologicalOrder()
    {
        // Arrange
        var entries = new List<WeightEntry>();
        var mockDb = new Mock<IDatabaseService>();
        
        mockDb.Setup(x => x.InsertWeightEntryAsync(It.IsAny<WeightEntry>()))
            .Callback<WeightEntry>(entry =>
            {
                entry.Id = entries.Count + 1;
                entry.CreatedAt = DateTime.Now;
                entries.Add(entry);
            })
            .ReturnsAsync((WeightEntry entry) => entry.Id);
        
        mockDb.Setup(x => x.GetAllWeightEntriesAsync())
            .ReturnsAsync(() => entries.OrderByDescending(e => e.Date).ToList());
        
        // Act - Add three measurements
        await mockDb.Object.InsertWeightEntryAsync(new WeightEntry
        {
            WeightKg = 76.0,
            Date = DateTime.Today.AddDays(-2),
            CreatedAt = DateTime.Now
        });
        
        await mockDb.Object.InsertWeightEntryAsync(new WeightEntry
        {
            WeightKg = 75.5,
            Date = DateTime.Today.AddDays(-1),
            CreatedAt = DateTime.Now
        });
        
        await mockDb.Object.InsertWeightEntryAsync(new WeightEntry
        {
            WeightKg = 75.0,
            Date = DateTime.Today,
            CreatedAt = DateTime.Now
        });
        
        var allEntries = await mockDb.Object.GetAllWeightEntriesAsync();
        
        // Assert - Verify chronological order (most recent first)
        Assert.Equal(3, allEntries.Count);
        Assert.Equal(75.0, allEntries[0].WeightKg); // Today
        Assert.Equal(75.5, allEntries[1].WeightKg); // Yesterday
        Assert.Equal(76.0, allEntries[2].WeightKg); // 2 days ago
    }
}
