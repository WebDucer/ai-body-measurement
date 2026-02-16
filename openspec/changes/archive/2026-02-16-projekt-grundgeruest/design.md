# Design: Projekt Grundgerüst

## Context

Dies ist eine neue .NET MAUI App für plattformübergreifendes Körpergewichts-Tracking. Die App wird von Grund auf neu entwickelt ohne bestehende Legacy-Systeme. Der Fokus liegt auf:

- **Local-First Ansatz**: Alle Daten werden lokal gespeichert, keine Cloud-Abhängigkeit
- **Cross-Platform**: iOS, Android, Windows, macOS mit gemeinsamer Codebasis
- **Testbarkeit**: Unit Tests für Business-Logik als Qualitätssicherung
- **Erweiterbarkeit**: Architektur vorbereitet für zukünftige Features (weitere Körperwerte, Cloud-Sync, etc.)

**Constraints:**
- .NET 10 und .NET MAUI Framework
- Open-Source Bibliotheken bevorzugt (MIT/Apache Lizenzen)
- Keine externen Services oder APIs in V1
- App ID: `page.eugen.maui.ai.bodymeasurement`

**Stakeholders:**
- Endnutzer: Einzelpersonen, die ihr Körpergewicht tracken möchten
- Entwickler: Wartbarkeit und Erweiterbarkeit der Codebasis

## Goals / Non-Goals

**Goals:**
- MAUI App-Projekt mit klarer Ordnerstruktur
- Separates Test-Projekt (xUnit) für Unit Tests
- MVVM-Architektur mit testbarer Business-Logik
- Lokale SQLite Datenbank für Gewichtsmessungen
- MAUI Preferences API für App-Einstellungen
- Mehrsprachigkeit (Deutsch/Englisch) via RESX
- Syncfusion Open-Source Toolkit für Charts
- Export nach CSV und JSON
- Unterstützung von kg und lbs (Pfund) mit Umrechnung
- Kurzes Onboarding beim ersten App-Start

**Non-Goals:**
- Keine Cloud-Synchronisation in V1 (später möglich)
- Keine Benutzer-Authentifizierung (Single-User App)
- Keine Social Features oder Sharing
- Keine Integration mit Health-Apps (Apple Health, Google Fit) in V1
- Keine komplexen Berechnungen wie BMI-Prognosen oder KI-Empfehlungen
- Kein Web-Interface (nur native Mobile/Desktop Apps)

## Decisions

### 1. Architektur: MVVM Pattern

**Entscheidung:** MVVM (Model-View-ViewModel) mit .NET MAUI Data Binding

**Rationale:**
- Standard-Pattern für .NET MAUI mit exzellentem Framework-Support
- Klare Trennung: View (XAML) ↔ ViewModel (Logik) ↔ Model/Services (Daten)
- ViewModels sind testbar ohne UI-Abhängigkeiten
- Community Binding (INotifyPropertyChanged) via CommunityToolkit.Mvvm

**Alternativen:**
- ❌ **MVU (Model-View-Update)**: Funktionaler Ansatz, aber weniger MAUI-Community-Support
- ❌ **Clean Architecture (Multi-Project)**: Overhead für diese App-Größe, erschwert schnelle Iteration

**Solution-Struktur:**
```
BodyMeasurement.sln
├── BodyMeasurement/              (MAUI App Projekt)
│   ├── Models/                   - Datenmodelle (WeightEntry, Statistics)
│   ├── ViewModels/               - MVVM ViewModels mit Commands und Properties
│   ├── Views/                    - XAML Pages und Controls
│   ├── Services/                 - Business-Logik (DatabaseService, SettingsService, ExportService)
│   ├── Resources/                - RESX, Styles, Images
│   ├── Platforms/                - Platform-spezifischer Code
│   └── MauiProgram.cs            - DI Container Setup
│
└── BodyMeasurement.Tests/        (xUnit Test Projekt)
    ├── ViewModels/               - ViewModel Unit Tests
    ├── Services/                 - Service Unit Tests
    └── Models/                   - Model/Logic Tests
```

### 2. Datenbank: sqlite-net-pcl

**Entscheidung:** `sqlite-net-pcl` als ORM für SQLite

**Rationale:**
- Leichtgewichtig (< 500 KB), perfekt für Mobile
- Einfache Attribute-basierte API (`[Table]`, `[PrimaryKey]`, `[AutoIncrement]`)
- Async/Await Support
- Gut dokumentiert, große Community
- Keine Migrations-Komplexität (einfache Schema-Evolution)

**Alternativen:**
- ❌ **Entity Framework Core**: Zu groß (mehrere MB), Overkill für einfaches Schema
- ❌ **Microsoft.Data.Sqlite**: Low-level, viel Boilerplate-Code

**Schema (V1):**
```csharp
[Table("WeightEntries")]
public class WeightEntry
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    
    public DateTime Date { get; set; }
    public double WeightKg { get; set; }  // Immer in kg speichern
    public string Notes { get; set; }     // Optional
    public DateTime CreatedAt { get; set; }
}
```

**Benutzer-Einstellungen:** 
Nutzen `.NET MAUI Preferences` API statt Datenbank-Tabelle für App-Settings:

```csharp
// Preferences Service für Settings
public interface ISettingsService
{
    string PreferredUnit { get; set; }        // "kg" oder "lbs"
    string Language { get; set; }              // "de" oder "en"
    bool OnboardingCompleted { get; set; }
}

public class SettingsService : ISettingsService
{
    public string PreferredUnit
    {
        get => Preferences.Get(nameof(PreferredUnit), "kg");
        set => Preferences.Set(nameof(PreferredUnit), value);
    }
    
    public string Language
    {
        get => Preferences.Get(nameof(Language), "en");
        set => Preferences.Set(nameof(Language), value);
    }
    
    public bool OnboardingCompleted
    {
        get => Preferences.Get(nameof(OnboardingCompleted), false);
        set => Preferences.Set(nameof(OnboardingCompleted), value);
    }
}
```

**Rationale für Preferences statt Datenbank:**
- Leichtgewichtig für Key-Value Einstellungen
- Platform-native Storage (UserDefaults/SharedPreferences/Settings)
- Kein Datenbank-Overhead für simple Settings
- Automatische Serialisierung für primitive Typen

### 3. Charts: Syncfusion .NET MAUI Toolkit (Open Source)

**Entscheidung:** Syncfusion Toolkit for .NET MAUI (MIT License)

**Rationale:**
- **100% Open Source** unter MIT Lizenz (keine kommerzielle Lizenz nötig)
- Cartesian Charts (Line, Bar, Area) für Gewichtsverlauf
- Hervorragende Performance und MAUI-Integration
- Professionelles Aussehen out-of-the-box
- Aktive Entwicklung (2025 Release)
- NuGet Package: `Syncfusion.Maui.Toolkit`

**Alternativen:**
- ❌ **Microcharts**: Weniger Features, begrenzte Anpassung
- ❌ **LiveCharts2**: Gut, aber größere Library mit mehr Dependencies
- ❌ **Syncfusion Commercial**: Nicht nötig, da Open-Source Toolkit ausreichend

**Chart-Typen für V1:**
- Line Chart: Gewichtsverlauf über Zeit
- Optional: Bar Chart für wöchentliche/monatliche Durchschnitte

### 4. Lokalisierung: RESX Resource Files

**Entscheidung:** .NET RESX Files mit `IStringLocalizer`

**Rationale:**
- Standard .NET Mechanismus, in Visual Studio integriert
- Kompilierte Ressourcen (schnell, typsicher via Code-Generator)
- Plattformübergreifend ohne zusätzliche Dependencies
- Einfache Struktur: `Resources/Strings.de.resx`, `Resources/Strings.en.resx`

**Alternativen:**
- ❌ **JSON-basiert**: Mehr Flexibilität, aber Runtime-Parsing overhead und kein Tooling-Support

**Implementierung:**
```csharp
// In MauiProgram.cs
builder.Services.AddLocalization();

// Verwendung in ViewModels
public class MainViewModel
{
    private readonly IStringLocalizer<Strings> _localizer;
    
    public string Title => _localizer["AppTitle"];
}
```

### 5. Dependency Injection: Built-in .NET MAUI DI

**Entscheidung:** `MauiProgram.cs` mit `IServiceCollection`

**Rationale:**
- Integriert in .NET MAUI, kein zusätzliches Package
- Ausreichend für App-Größe (Services, ViewModels, Views registrieren)
- Standard Microsoft DI-Container

**Alternativen:**
- ❌ **Autofac/Ninject**: Overhead für diese App, keine erweiterten Features nötig

**Registrierung:**
```csharp
// Services
builder.Services.AddSingleton<IDatabaseService, DatabaseService>();
builder.Services.AddSingleton<ISettingsService, SettingsService>();
builder.Services.AddSingleton<IExportService, ExportService>();
builder.Services.AddSingleton<IStatisticsService, StatisticsService>();

// ViewModels
builder.Services.AddTransient<MainViewModel>();
builder.Services.AddTransient<AddWeightViewModel>();

// Views
builder.Services.AddTransient<MainPage>();
builder.Services.AddTransient<AddWeightPage>();
```

### 6. App-Einstellungen: MAUI Preferences API

**Entscheidung:** `.NET MAUI Preferences` für Benutzer-Einstellungen

**Rationale:**
- Leichtgewichtig für Key-Value Settings (keine Datenbank nötig)
- Platform-native Storage (iOS: UserDefaults, Android: SharedPreferences, Windows: ApplicationData)
- Thread-safe und async-kompatibel
- Automatische Serialisierung für primitive Typen (string, bool, int, double, DateTime)
- Kein Datenbank-Overhead für einfache Settings

**Alternativen:**
- ❌ **SQLite Settings-Tabelle**: Overhead für einfache Key-Value Pairs
- ❌ **JSON Files**: Manuelle Serialisierung, keine Platform-Integration

**Implementierung via SettingsService:**
```csharp
public interface ISettingsService
{
    string PreferredUnit { get; set; }        // "kg" oder "lbs"
    string Language { get; set; }              // "de" oder "en"
    bool OnboardingCompleted { get; set; }
}

public class SettingsService : ISettingsService
{
    private const string DefaultUnit = "kg";
    private const string DefaultLanguage = "en";
    
    public string PreferredUnit
    {
        get => Preferences.Get(nameof(PreferredUnit), DefaultUnit);
        set => Preferences.Set(nameof(PreferredUnit), value);
    }
    
    public string Language
    {
        get => Preferences.Get(nameof(Language), DefaultLanguage);
        set => Preferences.Set(nameof(Language), value);
    }
    
    public bool OnboardingCompleted
    {
        get => Preferences.Get(nameof(OnboardingCompleted), false);
        set => Preferences.Set(nameof(OnboardingCompleted), value);
    }
}
```

**Testing:** Mock `ISettingsService` in Unit Tests (keine Platform-Dependency)

### 7. Testing: xUnit + Moq (Separates Projekt)

**Entscheidung:** Separates xUnit Test-Projekt, Moq für Mocking

**Rationale:**
- **Separates Projekt**: Klare Trennung zwischen App und Tests, Standard Best Practice
- xUnit: Modern, parallelisierbar, große .NET Community
- Moq: De-facto Standard für Mocking in .NET
- ViewModels und Services sind testbar (keine UI-Abhängigkeiten)
- ISettingsService als Interface → einfach mockbar in Tests

**Alternativen:**
- ❌ **NUnit**: Ähnlich gut, aber xUnit ist moderner
- ❌ **MSTest**: Weniger Features als xUnit
- ❌ **Tests im MAUI-Projekt**: Vermischt App und Tests, nicht Best Practice

**Test-Projekt Struktur:**
```
BodyMeasurement.Tests/
  /ViewModels    - ViewModel Unit Tests
  /Services      - Service Unit Tests (DatabaseService, SettingsService, etc.)
  /Models        - Model/Logic Tests
```

**Beispiel:**
```csharp
public class MainViewModelTests
{
    [Fact]
    public async Task LoadWeightEntries_ShouldPopulateList()
    {
        // Arrange
        var mockDb = new Mock<IDatabaseService>();
        mockDb.Setup(x => x.GetAllWeightEntriesAsync())
              .ReturnsAsync(new List<WeightEntry> { /* ... */ });
        
        var vm = new MainViewModel(mockDb.Object);
        
        // Act
        await vm.LoadWeightEntriesAsync();
        
        // Assert
        Assert.NotEmpty(vm.WeightEntries);
    }
}
```

### 8. Gewichtseinheiten: kg + lbs mit Umrechnung

**Entscheidung:** Intern in kg speichern, UI zeigt bevorzugte Einheit

**Rationale:**
- Datenbank speichert immer `WeightKg` (normalisiert)
- UI konvertiert on-the-fly basierend auf `AppSettings.PreferredUnit`
- Einfache Umrechnung: `1 kg = 2.20462 lbs`
- Vermeidet Daten-Inkonsistenzen

**Implementierung:**
```csharp
public static class WeightConverter
{
    public static double KgToLbs(double kg) => kg * 2.20462;
    public static double LbsToKg(double lbs) => lbs / 2.20462;
    
    public static string Format(double weightKg, string unit)
        => unit == "lbs" 
            ? $"{KgToLbs(weightKg):F1} lbs"
            : $"{weightKg:F1} kg";
}
```

### 9. Export-Formate: CSV + JSON

**Entscheidung:** Beide Formate unterstützen

**Rationale:**
- **CSV**: Excel-kompatibel, einfach zu importieren, human-readable
- **JSON**: Strukturiert, maschinenlesbar, für Entwickler/APIs

**Export-Service Interface:**
```csharp
public interface IExportService
{
    Task<string> ExportToCsvAsync(List<WeightEntry> entries);
    Task<string> ExportToJsonAsync(List<WeightEntry> entries);
    Task<bool> ShareFileAsync(string filePath);  // Platform Share Sheet
}
```

**CSV Format:**
```
Date,Weight (kg),Weight (lbs),Notes
2026-02-15,75.5,166.4,Morgens vor Frühstück
2026-02-14,76.0,167.6,
```

**JSON Format:**
```json
{
  "exportDate": "2026-02-15T10:30:00Z",
  "entries": [
    {
      "date": "2026-02-15T06:00:00Z",
      "weightKg": 75.5,
      "weightLbs": 166.4,
      "notes": "Morgens vor Frühstück"
    }
  ]
}
```

### 10. Onboarding: Kurzes 3-Screen Intro

**Entscheidung:** Einfache Carousel-basierte Einführung beim ersten Start

**Rationale:**
- Erklärt Kernfunktionen (Erfassen, Verlauf, Statistiken)
- Einmalig beim ersten Start (Flag in `AppSettings.OnboardingCompleted`)
- Kann übersprungen werden

**Screens:**
1. **Willkommen**: "Verfolge dein Körpergewicht einfach und privat"
2. **Features**: Erfassung, Verlauf, Statistiken, Export
3. **Einheit auswählen**: kg oder lbs (speichert in Settings)

**Implementierung:** Einfache `CarouselView` mit Skip-Button

### 11. Dark Mode Support

**Entscheidung:** Dark Mode via `AppThemeBinding` unterstützen

**Rationale:**
- .NET MAUI bietet native Support für System-Theme
- Automatische Umschaltung basierend auf OS-Einstellung
- Modern und von Nutzern erwartet
- Minimaler Mehraufwand: Styles in `ResourceDictionary` definieren

**Implementierung:**
```xaml
<Label Text="Title" 
       TextColor="{AppThemeBinding Light=Black, Dark=White}" />
```

**Styling-Strategie:**
- Light Theme: Standard Colors
- Dark Theme: Invertierte Farben + reduzierte Kontraste
- Syncfusion Charts passen sich automatisch an (Theme-aware)

### 12. Statistiken: Fokus auf Gewichtsveränderung

**Entscheidung:** Gewichtsveränderung als primäre Statistik in V1

**Rationale:**
- Wichtigste Metrik für Nutzer (Fortschritt sichtbar)
- Zeigt absolute Veränderung (kg/lbs) und prozentual
- Vergleich über verschiedene Zeiträume (7/30/90 Tage, gesamt)

**V1 Statistiken:**
- ✅ Gewichtsveränderung (absolut + prozentual)
- ✅ Aktuelles Gewicht
- ✅ Startgewicht (erste Messung)
- ⏭️ Durchschnitt, Min/Max, Trend → **V2** (nice-to-have)

**Display:**
```
Gewichtsveränderung (letzte 30 Tage)
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
-2.3 kg (-3.0%)  ⬇️
```

### 13. Chart-Interaktivität: Zeitraum-Filter

**Entscheidung:** Zeitraum-Filter für Chart-Ansicht

**Rationale:**
- Nutzer können relevanten Zeitraum wählen
- Bessere Übersicht bei langen Verläufen
- Einfach mit Syncfusion umsetzbar

**Filter-Optionen:**
- 1 Woche
- 1 Monat
- 3 Monate
- 6 Monate
- Alle Daten

**V1 ausgeschlossen:**
- ❌ Datenpunkte anklickbar → nice-to-have für V2
- ❌ Zoom/Pan → Komplexität nicht gerechtfertigt

### 14. Erinnerungen: Nicht in V1

**Entscheidung:** Keine Notifications in V1

**Rationale:**
- Fokus auf Core-Features (Tracking, Visualisierung, Export)
- Notifications erfordern Platform-spezifische Permissions
- Kann als separate Capability in V2 hinzugefügt werden

**Future V2:**
- Tägliche Erinnerung zur gewählten Zeit
- Konfigurierbar (aktivieren/deaktivieren, Zeit wählen)

### 15. Test-Coverage: 80%+ für ViewModels/Services

**Entscheidung:** Hohe Test-Coverage für Business-Logik

**Rationale:**
- ViewModels und Services enthalten kritische Logik
- 80%+ Coverage sichert Qualität und vermeidet Regressionen
- UI-Tests manuell (MAUI UI-Testing noch limitiert)

**Coverage-Ziele:**
- ViewModels: **80%+** (Commands, Property Changes, Data Binding)
- Services: **80%+** (Database, Export, Statistics, Unit Conversion)
- Models: **60%+** (Validation, Business Rules)
- Views: **Manuell** (Exploratory Testing auf allen Plattformen)

**CI/CD Integration:**
- Tests laufen automatisch bei jedem Commit
- Pull Requests benötigen grüne Tests
- Coverage-Report als Build-Artefakt

## Risks / Trade-offs

### Risk: Syncfusion Open-Source Toolkit Einschränkungen
**Beschreibung:** Die Open-Source Version könnte weniger Features haben als die kommerzielle Version.

**Mitigation:** 
- Recherche zeigt, dass Cartesian/Line/Bar Charts in Open-Source enthalten sind (ausreichend für V1)
- Falls nötig: Wechsel zu LiveCharts2 ist einfach (gleiche Datenstruktur)

### Risk: Keine Cloud-Backup
**Beschreibung:** Nutzer könnten Daten verlieren bei Gerätewechsel.

**Mitigation:**
- Export-Funktion erlaubt manuelles Backup (CSV/JSON)
- Dokumentation empfiehlt regelmäßige Exports
- Future: Cloud-Sync als separate Capability (optional)

### Trade-off: Separate Test-Projekt
**Entscheidung:** Separates xUnit Test-Projekt (nicht im MAUI-Projekt)

**Trade-off:**
- ✅ Vorteil: Saubere Trennung, klare Boundaries, Standard .NET Testing
- ✅ Vorteil: Tests können unabhängig von der App ausgeführt werden
- ❌ Nachteil: Leichter Mehraufwand bei Projektsetup

**Akzeptabel weil:** Bessere Testbarkeit überwiegt minimalen Setup-Overhead, klare Best Practice

### Risk: Platform-spezifische Unterschiede
**Beschreibung:** SQLite-Pfade, File Picker, Share Sheets unterscheiden sich pro Platform.

**Mitigation:**
- .NET MAUI bietet Abstractions (`FileSystem`, `Share` API)
- Platform-spezifischer Code in `/Platforms` isolieren
- Test auf allen Plattformen vor Release

### Trade-off: RESX vs. JSON Localization
**Entscheidung:** RESX für Einfachheit

**Trade-off:**
- ✅ Vorteil: Tooling-Support, typsicher, performant
- ❌ Nachteil: Neue Sprachen erfordern Rebuild (kein Hot-Reload)

**Akzeptabel weil:** Nur 2 Sprachen in V1, keine dynamische Spracherweiterung geplant

## Migration Plan

**Nicht anwendbar** - Dies ist eine neue App ohne Migration.

**Rollout-Strategie:**
1. **Internal Testing:** Lokale Builds auf allen Plattformen testen
2. **TestFlight/Beta:** Kleine Nutzergruppe für Feedback
3. **Public Release:** App Stores (iOS App Store, Google Play, Microsoft Store)

**Rollback:** Bei kritischen Bugs → App Store Rollback auf vorherige Version (falls vorhanden)

## Resolved Questions

Alle offenen Fragen wurden geklärt und als Design-Entscheidungen dokumentiert:

1. ✅ **Dark Mode Support** → Ja, via `AppThemeBinding` (Decision #11)
2. ✅ **Statistik-Fokus** → Gewichtsveränderung als primäre Metrik (Decision #12)
3. ✅ **Chart-Interaktivität** → Zeitraum-Filter (1W, 1M, 3M, 6M, Alle) (Decision #13)
4. ✅ **Erinnerungen** → Nicht in V1, Future Capability für V2 (Decision #14)
5. ✅ **Test-Coverage** → 80%+ für ViewModels/Services (Decision #15)

**Zusätzliche Klärungen:**
- ✅ **App-Einstellungen** → MAUI Preferences API statt SQLite (Decision #6)
- ✅ **Test-Projekt** → Separates xUnit-Projekt (Decision #7)

## Open Questions

Keine offenen Fragen mehr - alle Entscheidungen getroffen. Bereit für specs-Phase.
