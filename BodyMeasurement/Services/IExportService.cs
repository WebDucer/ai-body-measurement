using BodyMeasurement.Models;

namespace BodyMeasurement.Services;

/// <summary>
/// Interface for exporting weight data
/// </summary>
public interface IExportService
{
    /// <summary>
    /// Exports weight entries to CSV format
    /// </summary>
    /// <param name="entries">Weight entries to export</param>
    /// <param name="language">Language for headers ("en" or "de")</param>
    /// <returns>CSV file path</returns>
    Task<string> ExportToCsvAsync(List<WeightEntry> entries, string language = "en");

    /// <summary>
    /// Exports weight entries to JSON format
    /// </summary>
    /// <param name="entries">Weight entries to export</param>
    /// <returns>JSON file path</returns>
    Task<string> ExportToJsonAsync(List<WeightEntry> entries);

    /// <summary>
    /// Shares a file using the platform's share sheet
    /// </summary>
    /// <param name="filePath">Path to the file to share</param>
    /// <returns>True if sharing was successful</returns>
    Task<bool> ShareFileAsync(string filePath);
}
