# Localization Guide

This guide explains how to add new languages and maintain translations in the Body Measurement App.

## Overview

The app uses **.NET RESX resource files** for localization, which is the standard mechanism for .NET applications. Currently supported languages:

- **English (en)**: Default language
- **German (de)**: Secondary language

## Adding a New Language

### Step 1: Create Resource File

1. Navigate to `BodyMeasurement/Resources/`
2. Copy `Strings.resx` to `Strings.<languagecode>.resx`

Example for French:
```
Strings.resx       → Default (English)
Strings.de.resx    → German
Strings.fr.resx    → French (new)
```

### Step 2: Translate Strings

Open the new `.resx` file in Visual Studio or a text editor and translate all `<value>` elements.

Example `Strings.fr.resx`:
```xml
<?xml version="1.0" encoding="utf-8"?>
<root>
  <data name="AppTitle" xml:space="preserve">
    <value>Suivi du poids</value>
  </data>
  <data name="AddWeight" xml:space="preserve">
    <value>Ajouter un poids</value>
  </data>
  <!-- ... more translations -->
</root>
```

### Step 3: Update Settings Service

Add the new language code to the settings validation (optional but recommended):

```csharp
// BodyMeasurement/Services/SettingsService.cs
public string Language
{
    get => Preferences.Get(nameof(Language), GetDefaultLanguage());
    set
    {
        // Validate language code
        var supportedLanguages = new[] { "en", "de", "fr" };
        if (supportedLanguages.Contains(value))
        {
            Preferences.Set(nameof(Language), value);
        }
    }
}
```

### Step 4: Update Language Picker UI

Add the new language option to the SettingsPage picker:

```xaml
<!-- BodyMeasurement/Views/SettingsPage.xaml -->
<Picker Title="Language"
        SelectedItem="{Binding SelectedLanguage}">
    <Picker.ItemsSource>
        <x:Array Type="{x:Type x:String}">
            <x:String>English</x:String>
            <x:String>Deutsch</x:String>
            <x:String>Français</x:String> <!-- New -->
        </x:Array>
    </Picker.ItemsSource>
</Picker>
```

### Step 5: Test the Translation

1. Build the project
2. Run the app
3. Go to Settings → Change language to the new language
4. Navigate through all pages to verify translations

## Translation Keys Reference

### Common UI Elements

| Key | English | German | Usage |
|-----|---------|--------|-------|
| `AppTitle` | Body Measurement | Körpermessung | App title |
| `AddWeight` | Add Weight | Gewicht hinzufügen | Button/Page title |
| `EditWeight` | Edit Weight | Gewicht bearbeiten | Page title |
| `Delete` | Delete | Löschen | Button |
| `Cancel` | Cancel | Abbrechen | Button |
| `Save` | Save | Speichern | Button |

### Navigation

| Key | English | German | Usage |
|-----|---------|--------|-------|
| `TabHome` | Home | Start | Tab label |
| `TabChart` | Chart | Diagramm | Tab label |
| `TabStatistics` | Statistics | Statistik | Tab label |
| `TabSettings` | Settings | Einstellungen | Tab label |

### Weight Entry

| Key | English | German | Usage |
|-----|---------|--------|-------|
| `WeightLabel` | Weight | Gewicht | Form label |
| `DateLabel` | Date | Datum | Form label |
| `NotesLabel` | Notes | Notizen | Form label |
| `NotesPlaceholder` | Optional notes... | Optionale Notizen... | Placeholder |

### Validation

| Key | English | German | Usage |
|-----|---------|--------|-------|
| `WeightRequired` | Weight is required | Gewicht ist erforderlich | Validation error |
| `WeightMustBePositive` | Weight must be greater than 0 | Gewicht muss größer als 0 sein | Validation error |
| `DateCannotBeFuture` | Date cannot be in the future | Datum darf nicht in der Zukunft liegen | Validation error |

### Statistics

| Key | English | German | Usage |
|-----|---------|--------|-------|
| `CurrentWeight` | Current Weight | Aktuelles Gewicht | Label |
| `StartingWeight` | Starting Weight | Startgewicht | Label |
| `WeightChange` | Weight Change | Gewichtsveränderung | Label |
| `Period7Days` | Last 7 Days | Letzte 7 Tage | Filter option |
| `Period30Days` | Last 30 Days | Letzte 30 Tage | Filter option |
| `PeriodAll` | All Time | Gesamt | Filter option |

### Export

| Key | English | German | Usage |
|-----|---------|--------|-------|
| `Export` | Export | Exportieren | Menu/Button |
| `ExportFormat` | Export Format | Exportformat | Dialog title |
| `CSV` | CSV | CSV | Format option |
| `JSON` | JSON | JSON | Format option |
| `ExportSuccess` | Data exported successfully | Daten erfolgreich exportiert | Success message |
| `ExportError` | Failed to export data | Fehler beim Exportieren | Error message |

### Empty States

| Key | English | German | Usage |
|-----|---------|--------|-------|
| `NoMeasurements` | No measurements yet | Noch keine Messungen | Empty list |
| `NoMeasurementsDetail` | Tap + to add your first measurement | Tippe auf +, um deine erste Messung hinzuzufügen | Empty list detail |
| `NoDataForPeriod` | No data for selected period | Keine Daten für den gewählten Zeitraum | Empty chart |

### Settings

| Key | English | German | Usage |
|-----|---------|--------|-------|
| `Language` | Language | Sprache | Setting label |
| `PreferredUnit` | Preferred Unit | Bevorzugte Einheit | Setting label |
| `Kilograms` | Kilograms (kg) | Kilogramm (kg) | Unit option |
| `Pounds` | Pounds (lbs) | Pfund (lbs) | Unit option |

### Onboarding

| Key | English | German | Usage |
|-----|---------|--------|-------|
| `OnboardingWelcomeTitle` | Welcome | Willkommen | Screen title |
| `OnboardingWelcomeText` | Track your body weight easily and privately | Verfolge dein Körpergewicht einfach und privat | Description |
| `OnboardingFeaturesTitle` | Features | Funktionen | Screen title |
| `OnboardingUnitTitle` | Choose Your Unit | Wähle deine Einheit | Screen title |
| `Skip` | Skip | Überspringen | Button |
| `Next` | Next | Weiter | Button |
| `Done` | Done | Fertig | Button |

## File Format

RESX files are XML-based with the following structure:

```xml
<?xml version="1.0" encoding="utf-8"?>
<root>
  <resheader name="resmimetype">
    <value>text/microsoft-resx</value>
  </resheader>
  <!-- ... other headers -->
  
  <data name="KeyName" xml:space="preserve">
    <value>Translated Text</value>
    <comment>Optional context for translators</comment>
  </data>
</root>
```

### Best Practices

1. **Use xml:space="preserve"**: Preserves whitespace in translations
2. **Add comments**: Provide context for translators
3. **Keep keys in English**: Makes code more readable
4. **Use descriptive keys**: e.g., `WeightLabel` not `Label1`

## Using Translations in Code

### In ViewModels

```csharp
using Microsoft.Extensions.Localization;

public class MainViewModel
{
    private readonly IStringLocalizer<Strings> _localizer;
    
    public MainViewModel(IStringLocalizer<Strings> localizer)
    {
        _localizer = localizer;
    }
    
    public string Title => _localizer["AppTitle"];
    public string AddButtonText => _localizer["AddWeight"];
}
```

### In XAML

With StaticResource (compile-time):

```xaml
<Label Text="{x:Static resources:Strings.AppTitle}" />
```

With Binding (runtime):

```xaml
<Label Text="{Binding TitleText}" />
```

### In Services

```csharp
public class ExportService
{
    private readonly IStringLocalizer<Strings> _localizer;
    
    public async Task<string> ExportToCsvAsync(List<WeightEntry> entries, string language)
    {
        var header = language == "de" 
            ? "Datum,Gewicht (kg),Gewicht (lbs),Notizen"
            : "Date,Weight (kg),Weight (lbs),Notes";
        
        // Or use localizer:
        var header = $"{_localizer["DateLabel"]},{_localizer["WeightLabel"]} (kg),...";
        
        // ... rest of export logic
    }
}
```

## Language Detection

The app automatically detects the system language on first launch:

```csharp
private string GetDefaultLanguage()
{
    var culture = CultureInfo.CurrentCulture;
    var languageCode = culture.TwoLetterISOLanguageName; // e.g., "en", "de"
    
    // Check if supported
    var supportedLanguages = new[] { "en", "de" };
    return supportedLanguages.Contains(languageCode) ? languageCode : "en";
}
```

## Date and Number Formatting

### Date Formatting

Use culture-specific formatting:

```csharp
var date = DateTime.Today;
var formattedDate = date.ToString("d", CultureInfo.CurrentCulture);
// en: 2/16/2026
// de: 16.02.2026
```

### Number Formatting

Use culture-specific decimal separators:

```csharp
var weight = 75.5;
var formattedWeight = weight.ToString("F1", CultureInfo.CurrentCulture);
// en: 75.5
// de: 75,5 (comma instead of period)
```

## Testing Translations

### Manual Testing

1. Change device language in system settings
2. Restart app
3. Verify all UI elements display in the correct language

### Automated Testing

Test language-specific functionality:

```csharp
[Fact]
public void LanguageSwitch_UpdatesUIText()
{
    var mockSettings = new Mock<ISettingsService>();
    mockSettings.Setup(s => s.Language).Returns("de");
    
    // Verify German is used
    Assert.Equal("de", mockSettings.Object.Language);
}
```

## Translation Checklist

When adding a new language:

- [ ] Create `Strings.<lang>.resx` file
- [ ] Translate all string keys
- [ ] Update language picker in SettingsPage
- [ ] Test all pages and flows
- [ ] Verify date/number formatting
- [ ] Test validation messages
- [ ] Test empty states
- [ ] Test error messages
- [ ] Test export headers (CSV)
- [ ] Update README.md with new language

## Common Issues

### Missing Translation

If a key is missing in a language file, the app falls back to the default (English) value.

**Solution**: Always add new keys to all language files.

### Incorrect Encoding

RESX files must be UTF-8 encoded to support special characters.

**Solution**: Save files with UTF-8 encoding in your text editor.

### Hardcoded Strings

Avoid hardcoded strings in XAML or C# code.

**Solution**: Always use localized string keys from RESX files.

## Resources

- [.NET Localization Documentation](https://learn.microsoft.com/dotnet/core/extensions/localization)
- [RESX File Format](https://learn.microsoft.com/dotnet/framework/resources/working-with-resx-files-programmatically)
- [MAUI Localization](https://learn.microsoft.com/dotnet/maui/fundamentals/localization)
- [ISO 639-1 Language Codes](https://en.wikipedia.org/wiki/List_of_ISO_639-1_codes)
