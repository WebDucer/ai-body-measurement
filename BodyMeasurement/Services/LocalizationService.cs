using System.Globalization;
using BodyMeasurement.Resources.Strings;

namespace BodyMeasurement.Services;

/// <summary>
/// Implementation of localization service using .resx resources.
/// Localized strings are accessed directly via the generated <see cref="Strings"/> class.
/// </summary>
public class LocalizationService : ILocalizationService
{
    private readonly ISettingsService _settingsService;

    public LocalizationService(ISettingsService settingsService)
    {
        _settingsService = settingsService;
        SetLanguage(_settingsService.Language);
    }

    /// <summary>
    /// Gets the current app language
    /// </summary>
    public string CurrentLanguage => _settingsService.Language;

    /// <summary>
    /// Sets the app language and updates the UI culture and the generated Strings class culture.
    /// </summary>
    public void SetLanguage(string languageCode)
    {
        if (string.IsNullOrEmpty(languageCode))
            return;

        _settingsService.Language = languageCode;

        var culture = new CultureInfo(languageCode);
        CultureInfo.CurrentUICulture = culture;
        CultureInfo.CurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;
        CultureInfo.DefaultThreadCurrentCulture = culture;

        // Keep the generated Strings class in sync so that Strings.Foo
        // returns the correct translation regardless of thread context.
        Strings.Culture = culture;
    }
}
