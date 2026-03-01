namespace BodyMeasurement.Services;

/// <summary>
/// Interface for managing app settings
/// </summary>
public interface ISettingsService
{
    /// <summary>
    /// Preferred unit for displaying weight ("kg" or "lbs")
    /// </summary>
    string PreferredUnit { get; set; }

    /// <summary>
    /// App language ("de" or "en")
    /// </summary>
    string Language { get; set; }

    /// <summary>
    /// Whether the onboarding flow has been completed
    /// </summary>
    bool OnboardingCompleted { get; set; }

    /// <summary>
    /// Display name for the user
    /// </summary>
    string UserName { get; set; }

    /// <summary>
    /// Goal weight in kg, or null if not set
    /// </summary>
    double? GoalWeightKg { get; set; }
}
