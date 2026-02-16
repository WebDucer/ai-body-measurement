using SQLite;

namespace BodyMeasurement.Models;

/// <summary>
/// Represents a weight measurement entry
/// </summary>
[Table("WeightEntries")]
public class WeightEntry
{
    /// <summary>
    /// Unique identifier for the weight entry
    /// </summary>
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    /// <summary>
    /// Date of the weight measurement
    /// </summary>
    [Indexed]
    public DateTime Date { get; set; }

    /// <summary>
    /// Weight in kilograms (always stored in kg for consistency)
    /// </summary>
    public double WeightKg { get; set; }

    /// <summary>
    /// Optional notes about the measurement
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Timestamp when this entry was created
    /// </summary>
    public DateTime CreatedAt { get; set; }
}
