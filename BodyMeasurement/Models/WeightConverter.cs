namespace BodyMeasurement.Models;

/// <summary>
/// Utility class for converting weight between kg and lbs
/// </summary>
public static class WeightConverter
{
    private const double KgToLbsMultiplier = 2.20462;

    /// <summary>
    /// Converts weight from kilograms to pounds
    /// </summary>
    /// <param name="kg">Weight in kilograms</param>
    /// <returns>Weight in pounds</returns>
    public static double KgToLbs(double kg)
    {
        return kg * KgToLbsMultiplier;
    }

    /// <summary>
    /// Converts weight from pounds to kilograms
    /// </summary>
    /// <param name="lbs">Weight in pounds</param>
    /// <returns>Weight in kilograms</returns>
    public static double LbsToKg(double lbs)
    {
        return lbs / KgToLbsMultiplier;
    }

    /// <summary>
    /// Formats weight in the specified unit
    /// </summary>
    /// <param name="weightKg">Weight in kilograms (stored value)</param>
    /// <param name="unit">Unit to display ("kg" or "lbs")</param>
    /// <returns>Formatted weight string</returns>
    public static string Format(double weightKg, string unit)
    {
        if (unit.Equals("lbs", StringComparison.OrdinalIgnoreCase))
        {
            return $"{KgToLbs(weightKg):F1} lbs";
        }
        return $"{weightKg:F1} kg";
    }
}
