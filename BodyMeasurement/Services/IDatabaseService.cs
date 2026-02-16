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
    /// Inserts a new weight entry into the database
    /// </summary>
    Task<int> InsertWeightEntryAsync(WeightEntry entry);

    /// <summary>
    /// Gets all weight entries sorted by date descending
    /// </summary>
    Task<List<WeightEntry>> GetAllWeightEntriesAsync();

    /// <summary>
    /// Gets a weight entry by its ID
    /// </summary>
    Task<WeightEntry?> GetWeightEntryByIdAsync(int id);

    /// <summary>
    /// Updates an existing weight entry
    /// </summary>
    Task<int> UpdateWeightEntryAsync(WeightEntry entry);

    /// <summary>
    /// Deletes a weight entry
    /// </summary>
    Task<int> DeleteWeightEntryAsync(int id);

    /// <summary>
    /// Gets weight entries within a date range
    /// </summary>
    Task<List<WeightEntry>> GetWeightEntriesInDateRangeAsync(DateTime startDate, DateTime endDate);
}
