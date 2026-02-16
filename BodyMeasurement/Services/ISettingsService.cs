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
}
