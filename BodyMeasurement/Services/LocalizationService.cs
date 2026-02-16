using System.Globalization;
using System.Resources;

namespace BodyMeasurement.Services;

/// <summary>
/// Implementation of localization service using .resx resources
/// </summary>
public class LocalizationService : ILocalizationService
{
    private readonly ISettingsService _settingsService;
    private readonly ResourceManager _resourceManager;

    public LocalizationService(ISettingsService settingsService)
    {
        _settingsService = settingsService;
        
        // Get the resource manager for Strings.resx
        _resourceManager = new ResourceManager(
            "BodyMeasurement.Resources.Strings.Strings",
            typeof(LocalizationService).Assembly);

        // Set the initial culture based on saved language
        SetLanguage(_settingsService.Language);
    }

    /// <summary>
    /// Gets the current app language
    /// </summary>
    public string CurrentLanguage => _settingsService.Language;

    /// <summary>
    /// Sets the app language and updates the UI culture
    /// </summary>
    public void SetLanguage(string languageCode)
    {
        if (string.IsNullOrEmpty(languageCode))
            return;

        // Save to preferences
        _settingsService.Language = languageCode;

        // Set the UI culture
        var culture = new CultureInfo(languageCode);
        CultureInfo.CurrentUICulture = culture;
        CultureInfo.CurrentCulture = culture;

        // Also set the default thread culture
        CultureInfo.DefaultThreadCurrentUICulture = culture;
        CultureInfo.DefaultThreadCurrentCulture = culture;
    }

    /// <summary>
    /// Gets a localized string by key
    /// </summary>
    public string GetString(string key)
    {
        try
        {
            return _resourceManager.GetString(key, CultureInfo.CurrentUICulture) ?? key;
        }
        catch
        {
            return key;
        }
    }
}
