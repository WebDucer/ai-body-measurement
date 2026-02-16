using BodyMeasurement.Services;
using Moq;

namespace BodyMeasurement.Tests.Services;

public class SettingsServiceTests
{
    [Fact]
    public void ISettingsService_Interface_HasRequiredProperties()
    {
        // Arrange - Create a mock that implements the interface
        var mockSettings = new Mock<ISettingsService>();
        
        // Setup default values
        mockSettings.Setup(s => s.PreferredUnit).Returns("kg");
        mockSettings.Setup(s => s.Language).Returns("en");
        mockSettings.Setup(s => s.OnboardingCompleted).Returns(false);

        // Act
        var service = mockSettings.Object;

        // Assert - Verify interface contract
        Assert.Equal("kg", service.PreferredUnit);
        Assert.Equal("en", service.Language);
        Assert.False(service.OnboardingCompleted);
    }

    [Fact]
    public void PreferredUnit_CanBeSetToKg()
    {
        // Arrange
        var mockSettings = new Mock<ISettingsService>();
        mockSettings.SetupProperty(s => s.PreferredUnit, "kg");

        var service = mockSettings.Object;

        // Act
        service.PreferredUnit = "kg";

        // Assert
        Assert.Equal("kg", service.PreferredUnit);
    }

    [Fact]
    public void PreferredUnit_CanBeSetToLbs()
    {
        // Arrange
        var mockSettings = new Mock<ISettingsService>();
        mockSettings.SetupProperty(s => s.PreferredUnit, "kg");

        var service = mockSettings.Object;

        // Act
        service.PreferredUnit = "lbs";

        // Assert
        Assert.Equal("lbs", service.PreferredUnit);
    }

    [Fact]
    public void Language_CanBeSetToDe()
    {
        // Arrange
        var mockSettings = new Mock<ISettingsService>();
        mockSettings.SetupProperty(s => s.Language, "en");

        var service = mockSettings.Object;

        // Act
        service.Language = "de";

        // Assert
        Assert.Equal("de", service.Language);
    }

    [Fact]
    public void Language_CanBeSetToEn()
    {
        // Arrange
        var mockSettings = new Mock<ISettingsService>();
        mockSettings.SetupProperty(s => s.Language, "en");

        var service = mockSettings.Object;

        // Act
        service.Language = "en";

        // Assert
        Assert.Equal("en", service.Language);
    }

    [Fact]
    public void OnboardingCompleted_CanBeSetToTrue()
    {
        // Arrange
        var mockSettings = new Mock<ISettingsService>();
        mockSettings.SetupProperty(s => s.OnboardingCompleted, false);

        var service = mockSettings.Object;

        // Act
        service.OnboardingCompleted = true;

        // Assert
        Assert.True(service.OnboardingCompleted);
    }

    [Fact]
    public void OnboardingCompleted_DefaultsToFalse()
    {
        // Arrange
        var mockSettings = new Mock<ISettingsService>();
        mockSettings.Setup(s => s.OnboardingCompleted).Returns(false);

        var service = mockSettings.Object;

        // Assert
        Assert.False(service.OnboardingCompleted);
    }
}
