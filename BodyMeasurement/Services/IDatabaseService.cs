using BodyMeasurement.Models;

namespace BodyMeasurement.Services;

/// <summary>
/// Interface for database operations on weight entries
/// </summary>
public interface IDatabaseService
{
    /// <summary>
    /// Initializes the database and creates tables if they don't exist
    /// </summary>
    Task InitializeAsync();

    /// <summary>
    /// Records a new measurement in the database
    /// </summary>
    Task<int> RecordMeasurementAsync(WeightEntry entry);

    /// <summary>
    /// Gets the full measurement history sorted by date descending
    /// </summary>
    Task<List<WeightEntry>> GetMeasurementHistoryAsync();

    /// <summary>
    /// Finds a measurement by its ID
    /// </summary>
    Task<WeightEntry?> FindMeasurementAsync(int id);

    /// <summary>
    /// Updates an existing measurement
    /// </summary>
    Task<int> UpdateMeasurementAsync(WeightEntry entry);

    /// <summary>
    /// Removes a measurement
    /// </summary>
    Task<int> RemoveMeasurementAsync(int id);

    /// <summary>
    /// Gets measurements within a date range
    /// </summary>
    Task<List<WeightEntry>> GetMeasurementsInPeriodAsync(DateTime startDate, DateTime endDate);
}
