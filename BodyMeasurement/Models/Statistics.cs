namespace BodyMeasurement.Models;

/// <summary>
/// Represents calculated statistics for weight measurements
/// </summary>
public class Statistics
{
    /// <summary>
    /// Current weight (most recent measurement) in kg
    /// </summary>
    public double? CurrentWeightKg { get; set; }

    /// <summary>
    /// Starting weight (earliest measurement) in kg
    /// </summary>
    public double? StartingWeightKg { get; set; }

    /// <summary>
    /// Weight change in kg (positive = gained, negative = lost)
    /// </summary>
    public double? WeightChangeKg { get; set; }

    /// <summary>
    /// Weight change as a percentage
    /// </summary>
    public double? WeightChangePercentage { get; set; }

    /// <summary>
    /// Total number of measurements
    /// </summary>
    public int TotalMeasurements { get; set; }

    /// <summary>
    /// Date of first measurement
    /// </summary>
    public DateTime? FirstMeasurementDate { get; set; }

    /// <summary>
    /// Date of most recent measurement
    /// </summary>
    public DateTime? LastMeasurementDate { get; set; }
}
