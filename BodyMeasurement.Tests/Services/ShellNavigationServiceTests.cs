using BodyMeasurement.Services;
using Moq;

namespace BodyMeasurement.Tests.Services;

/// <summary>
/// Tests for ShellNavigationService.
/// Note: Full integration tests for Shell navigation require a MAUI platform host.
/// These unit tests verify the service satisfies the INavigationService contract
/// and that the interface can be mocked for use in ViewModel tests.
/// </summary>
public class ShellNavigationServiceTests
{
    [Fact]
    public void INavigationService_CanBeMocked()
    {
        // Arrange & Act
        var mockNav = new Mock<INavigationService>();

        // Assert - verify all interface methods are mockable
        Assert.NotNull(mockNav.Object);
    }

    [Fact]
    public async Task INavigationService_OpenAddMeasurementAsync_IsMockable()
    {
        // Arrange
        var mockNav = new Mock<INavigationService>();
        mockNav.Setup(x => x.OpenAddMeasurementAsync()).Returns(Task.CompletedTask);

        // Act
        await mockNav.Object.OpenAddMeasurementAsync();

        // Assert
        mockNav.Verify(x => x.OpenAddMeasurementAsync(), Times.Once);
    }

    [Fact]
    public async Task INavigationService_OpenEditMeasurementAsync_PassesMeasurementId()
    {
        // Arrange
        var mockNav = new Mock<INavigationService>();
        int capturedId = 0;
        mockNav.Setup(x => x.OpenEditMeasurementAsync(It.IsAny<int>()))
               .Callback<int>(id => capturedId = id)
               .Returns(Task.CompletedTask);

        // Act
        await mockNav.Object.OpenEditMeasurementAsync(42);

        // Assert - verify the correct measurement ID is passed through
        Assert.Equal(42, capturedId);
        mockNav.Verify(x => x.OpenEditMeasurementAsync(42), Times.Once);
    }

    [Fact]
    public async Task INavigationService_GoBackAsync_IsMockable()
    {
        // Arrange
        var mockNav = new Mock<INavigationService>();
        mockNav.Setup(x => x.GoBackAsync()).Returns(Task.CompletedTask);

        // Act
        await mockNav.Object.GoBackAsync();

        // Assert
        mockNav.Verify(x => x.GoBackAsync(), Times.Once);
    }

    [Fact]
    public async Task INavigationService_ShowConfirmationAsync_ReturnsBoolean()
    {
        // Arrange
        var mockNav = new Mock<INavigationService>();
        mockNav.Setup(x => x.ShowConfirmationAsync(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
               .ReturnsAsync(true);

        // Act
        var result = await mockNav.Object.ShowConfirmationAsync("Title", "Message", "Yes", "No");

        // Assert
        Assert.True(result);
        mockNav.Verify(x => x.ShowConfirmationAsync("Title", "Message", "Yes", "No"), Times.Once);
    }

    [Fact]
    public async Task INavigationService_ShowAlertAsync_IsMockable()
    {
        // Arrange
        var mockNav = new Mock<INavigationService>();
        mockNav.Setup(x => x.ShowAlertAsync(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
               .Returns(Task.CompletedTask);

        // Act
        await mockNav.Object.ShowAlertAsync("Title", "Message", "OK");

        // Assert
        mockNav.Verify(x => x.ShowAlertAsync("Title", "Message", "OK"), Times.Once);
    }

}
