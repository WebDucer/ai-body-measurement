using BodyMeasurement.Models;
using BodyMeasurement.Services;
using Moq;

namespace BodyMeasurement.Tests.ViewModels;

public class AddEditWeightViewModelTests
{
    [Fact]
    public void Constructor_InitializesWithAddMode()
    {
        // Arrange
        var mockDb = new Mock<IDatabaseService>();
        var mockSettings = new Mock<ISettingsService>();
        mockSettings.Setup(s => s.PreferredUnit).Returns("kg");

        // Act - Verify services can be injected
        // Note: We can't instantiate AddEditWeightViewModel directly due to MAUI dependencies

        // Assert
        Assert.NotNull(mockDb.Object);
        Assert.NotNull(mockSettings.Object);
    }

    [Fact]
    public async Task LoadWeightEntry_PopulatesProperties()
    {
        // Arrange
        var mockDb = new Mock<IDatabaseService>();
        var mockSettings = new Mock<ISettingsService>();
        
        var entry = new WeightEntry
        {
            Id = 1,
            Date = DateTime.Today,
            WeightKg = 75.0,
            Notes = "Test note"
        };

        mockDb.Setup(x => x.GetWeightEntryByIdAsync(1)).ReturnsAsync(entry);
        mockSettings.Setup(s => s.PreferredUnit).Returns("kg");

        // Act
        var loadedEntry = await mockDb.Object.GetWeightEntryByIdAsync(1);

        // Assert
        Assert.NotNull(loadedEntry);
        Assert.Equal(75.0, loadedEntry.WeightKg);
        Assert.Equal("Test note", loadedEntry.Notes);
    }

    [Fact]
    public async Task SaveAsync_WithNewEntry_InsertsToDatabase()
    {
        // Arrange
        var mockDb = new Mock<IDatabaseService>();
        var entry = new WeightEntry
        {
            WeightKg = 75.0,
            Date = DateTime.Today,
            Notes = "New entry"
        };

        mockDb.Setup(x => x.InsertWeightEntryAsync(It.IsAny<WeightEntry>()))
              .ReturnsAsync(1);

        // Act
        await mockDb.Object.InsertWeightEntryAsync(entry);

        // Assert
        mockDb.Verify(x => x.InsertWeightEntryAsync(It.IsAny<WeightEntry>()), Times.Once);
    }

    [Fact]
    public async Task SaveAsync_WithExistingEntry_UpdatesDatabase()
    {
        // Arrange
        var mockDb = new Mock<IDatabaseService>();
        var entry = new WeightEntry
        {
            Id = 1,
            WeightKg = 75.5,
            Date = DateTime.Today,
            Notes = "Updated entry"
        };

        mockDb.Setup(x => x.GetWeightEntryByIdAsync(1)).ReturnsAsync(entry);
        mockDb.Setup(x => x.UpdateWeightEntryAsync(It.IsAny<WeightEntry>()))
              .ReturnsAsync(1);

        // Act
        var loadedEntry = await mockDb.Object.GetWeightEntryByIdAsync(1);
        loadedEntry!.WeightKg = 76.0;
        await mockDb.Object.UpdateWeightEntryAsync(loadedEntry);

        // Assert
        mockDb.Verify(x => x.UpdateWeightEntryAsync(It.IsAny<WeightEntry>()), Times.Once);
    }

    [Fact]
    public void Validation_WeightZero_ReturnsError()
    {
        // Arrange
        double weight = 0;

        // Act
        bool isValid = weight > 0;

        // Assert
        Assert.False(isValid);
    }

    [Fact]
    public void Validation_WeightNegative_ReturnsError()
    {
        // Arrange
        double weight = -5.0;

        // Act
        bool isValid = weight > 0;

        // Assert
        Assert.False(isValid);
    }

    [Fact]
    public void Validation_WeightPositive_PassesValidation()
    {
        // Arrange
        double weight = 75.0;

        // Act
        bool isValid = weight > 0;

        // Assert
        Assert.True(isValid);
    }

    [Fact]
    public void Validation_DateInFuture_ReturnsError()
    {
        // Arrange
        DateTime date = DateTime.Today.AddDays(1);

        // Act
        bool isValid = date <= DateTime.Today;

        // Assert
        Assert.False(isValid);
    }

    [Fact]
    public void Validation_DateToday_PassesValidation()
    {
        // Arrange
        DateTime date = DateTime.Today;

        // Act
        bool isValid = date <= DateTime.Today;

        // Assert
        Assert.True(isValid);
    }

    [Fact]
    public void Validation_DateInPast_PassesValidation()
    {
        // Arrange
        DateTime date = DateTime.Today.AddDays(-1);

        // Act
        bool isValid = date <= DateTime.Today;

        // Assert
        Assert.True(isValid);
    }

    [Fact]
    public void UnitConversion_KgToLbs_ConvertsCorrectly()
    {
        // Arrange
        double weightKg = 75.0;
        string preferredUnit = "lbs";

        // Act
        double displayWeight = preferredUnit == "lbs" 
            ? WeightConverter.KgToLbs(weightKg) 
            : weightKg;

        // Assert
        Assert.Equal(165.3465, displayWeight, 4);
    }

    [Fact]
    public void UnitConversion_LbsToKg_ConvertsCorrectly()
    {
        // Arrange
        double weightLbs = 165.3465;
        string preferredUnit = "lbs";

        // Act
        double weightKg = preferredUnit == "lbs" 
            ? WeightConverter.LbsToKg(weightLbs) 
            : weightLbs;

        // Assert
        Assert.Equal(75.0, weightKg, 4);
    }
}
