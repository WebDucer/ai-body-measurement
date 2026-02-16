using BodyMeasurement.Models;

namespace BodyMeasurement.Services;

/// <summary>
/// Interface for calculating weight statistics
/// </summary>
public interface IStatisticsService
{
    /// <summary>
    /// Gets the current weight (most recent measurement)
    /// </summary>
    Task<double?> GetCurrentWeightAsync();

    /// <summary>
    /// Gets the starting weight (earliest measurement)
    /// </summary>
    Task<double?> GetStartingWeightAsync();

    /// <summary>
    /// Calculates overall weight change (absolute and percentage)
    /// </summary>
    Task<(double? absolute, double? percentage)> CalculateWeightChangeAsync();

    /// <summary>
    /// Calculates weight change for a specific period
    /// </summary>
    /// <param name="days">Number of days to look back (7, 30, 90, etc.)</param>
    Task<(double? absolute, double? percentage)> CalculateWeightChangeForPeriodAsync(int days);

    /// <summary>
    /// Gets comprehensive statistics
    /// </summary>
    Task<Statistics> GetStatisticsAsync(int? periodDays = null);
}
