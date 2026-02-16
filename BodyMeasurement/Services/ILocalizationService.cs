namespace BodyMeasurement.Services;

/// <summary>
/// Service for managing app localization
/// </summary>
public interface ILocalizationService
{
    /// <summary>
    /// Gets the current app language
    /// </summary>
    string CurrentLanguage { get; }

    /// <summary>
    /// Sets the app language and updates the UI culture
    /// </summary>
    void SetLanguage(string languageCode);

    /// <summary>
    /// Gets a localized string by key
    /// </summary>
    string GetString(string key);
}
