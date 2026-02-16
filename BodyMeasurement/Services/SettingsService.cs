namespace BodyMeasurement.Services;

/// <summary>
/// Implementation of settings service using MAUI Preferences API
/// </summary>
public class SettingsService : ISettingsService
{
    private const string PreferredUnitKey = "PreferredUnit";
    private const string LanguageKey = "Language";
    private const string OnboardingCompletedKey = "OnboardingCompleted";

    private const string DefaultUnit = "kg";
    private const string DefaultLanguage = "en";

    /// <summary>
    /// Preferred unit for displaying weight ("kg" or "lbs")
    /// </summary>
    public string PreferredUnit
    {
        get => Preferences.Get(PreferredUnitKey, DefaultUnit);
        set => Preferences.Set(PreferredUnitKey, value);
    }

    /// <summary>
    /// App language ("de" or "en")
    /// </summary>
    public string Language
    {
        get
        {
            // Try to get saved language, otherwise detect system language
            var savedLanguage = Preferences.Get(LanguageKey, string.Empty);
            if (!string.IsNullOrEmpty(savedLanguage))
            {
                return savedLanguage;
            }

            // Detect system language
            var culture = System.Globalization.CultureInfo.CurrentUICulture;
            var systemLanguage = culture.TwoLetterISOLanguageName.ToLowerInvariant();

            // Only support "de" and "en", default to "en"
            return systemLanguage == "de" ? "de" : DefaultLanguage;
        }
        set => Preferences.Set(LanguageKey, value);
    }

    /// <summary>
    /// Whether the onboarding flow has been completed
    /// </summary>
    public bool OnboardingCompleted
    {
        get => Preferences.Get(OnboardingCompletedKey, false);
        set => Preferences.Set(OnboardingCompletedKey, value);
    }
}
