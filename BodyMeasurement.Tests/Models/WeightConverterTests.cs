using BodyMeasurement.Models;

namespace BodyMeasurement.Tests.Models;

public class WeightConverterTests
{
    [Fact]
    public void KgToLbs_ConvertsCorrectly()
    {
        // Arrange
        double kg = 75.0;

        // Act
        double lbs = WeightConverter.KgToLbs(kg);

        // Assert
        Assert.Equal(165.3465, lbs, 4); // 4 decimal places precision
    }

    [Fact]
    public void LbsToKg_ConvertsCorrectly()
    {
        // Arrange
        double lbs = 165.3465;

        // Act
        double kg = WeightConverter.LbsToKg(lbs);

        // Assert
        Assert.Equal(75.0, kg, 4); // 4 decimal places precision
    }

    [Fact]
    public void KgToLbs_RoundTrip_MaintainsAccuracy()
    {
        // Arrange
        double original = 70.5;

        // Act
        double converted = WeightConverter.LbsToKg(WeightConverter.KgToLbs(original));

        // Assert
        Assert.Equal(original, converted, 6); // 6 decimal places for round trip
    }

    [Fact]
    public void Format_WithKg_ReturnsFormattedString()
    {
        // Arrange
        double weightKg = 75.5;

        // Act
        string result = WeightConverter.Format(weightKg, "kg");

        // Assert
        Assert.Equal("75.5 kg", result);
    }

    [Fact]
    public void Format_WithLbs_ReturnsFormattedString()
    {
        // Arrange
        double weightKg = 75.0;

        // Act
        string result = WeightConverter.Format(weightKg, "lbs");

        // Assert
        Assert.Equal("165.3 lbs", result);
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(1, 2.20462)]
    [InlineData(100, 220.462)]
    public void KgToLbs_WithVariousValues_ConvertsCorrectly(double kg, double expectedLbs)
    {
        // Act
        double result = WeightConverter.KgToLbs(kg);

        // Assert
        Assert.Equal(expectedLbs, result, 3);
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(2.20462, 1)]
    [InlineData(220.462, 100)]
    public void LbsToKg_WithVariousValues_ConvertsCorrectly(double lbs, double expectedKg)
    {
        // Act
        double result = WeightConverter.LbsToKg(lbs);

        // Assert
        Assert.Equal(expectedKg, result, 3);
    }
}
