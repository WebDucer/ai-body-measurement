## Why

Die App enthält eine Reihe akkumulierter technischer Schulden und kritischer Fehler: ein App-Absturz beim Bearbeiten von Einträgen, eine nicht funktionierende CI/CD-Pipeline, Architekturverstöße (ViewModels greifen direkt auf UI-APIs zu), nicht übersetzte Texte sowie veraltete Projekt- und Test-Konfigurationen. Diese Korrekturen sind notwendig, um die App stabil, wartbar und testbar zu machen.

## What Changes

### Fehlerbehebungen (kritisch)
- **BUGFIX**: App-Absturz beim Öffnen von "Edit" — `int`-Query-Parameter wird als `string` übergeben, was einen `InvalidCastException` in `ShellContent.ApplyQueryAttributes` auslöst → Navigation muss Parameter als `string` übertragen und im ViewModel konvertieren
- **BUGFIX**: CI/CD-Pipeline schlägt fehl auf Ubuntu (MAUI Workload nicht unterstützt für Tests), Windows (fehlendes `--runtime`-Argument bei Restore) und macOS (falsches Xcode für .NET 10 iOS SDK)

### Infrastruktur & Tooling
- Neues Solution-Format `.slnx` einführen (ersetzt `BodyMeasurement.sln`)
- Testprojekt auf echte `<ProjectReference>` umstellen (statt `<Compile Include="...">` File-Linking) gemäß Microsoft MAUI Unit Testing Dokumentation
- Testprojekt auf xUnit v3 mit Microsoft Testing Platform migrieren (ersetzt xUnit 2.x + `Microsoft.NET.Test.Sdk`)
- CI/CD-Pipeline anpassen: Tests + Android-Build auf allen Zweigen; alle Plattform-Builds (iOS, macOS, Windows) nur auf `main`

### Architektur & Code-Qualität
- ViewModels von UI-APIs entkoppeln: `INavigationService` einführen, das Navigation und Dialoge vereint, sodass ViewModels testbar werden
- `System.Diagnostics.Debug.WriteLine` durch `ILogger<T>` ersetzen (durchgehend in allen ViewModels und Services)
- Nicht genutzte `Microsoft.Extensions.Localization`-Bibliothek aus `BodyMeasurement.csproj` entfernen
- Daten-Schicht erhält domänenspezifische Methodennamen: `InsertWeightEntryAsync` → `RecordMeasurementAsync`, `GetAllWeightEntriesAsync` → `GetMeasurementHistoryAsync`, `GetWeightEntryByIdAsync` → `FindMeasurementAsync`, `UpdateWeightEntryAsync` → `UpdateMeasurementAsync`, `DeleteWeightEntryAsync` → `RemoveMeasurementAsync`, `GetWeightEntriesInDateRangeAsync` → `GetMeasurementsInPeriodAsync`
- View+ViewModel-Registrierung in eine lesbare Extension-Methode auslagern (`AddViewWithViewModel<TView, TViewModel>()`)

### UI & Lokalisierung
- Hard-codierte Texte in ViewModels und Views durch lokalisierte Strings ersetzen (insb. `DisplayAlert`-Texte, Fehlermeldungen)
- Icons für Chart- und Statistik-Tab in der Shell korrigieren (zeigen aktuell Rechtecke statt korrekter Icons)
- Floating Action Button auf Android vertikal verschoben (Android-Button-System-Padding schiebt `+`-Text nach unten) — iOS korrekt
- 26 Build-Warnungen analysieren und beheben

## Capabilities

### New Capabilities
- `navigation-service`: Kombinierte Abstraktion für Navigation und Dialoge (`INavigationService`) — wird in ViewModels injiziert statt direkter `Shell.Current`- und `Application.Current.MainPage`-Zugriffe

### Modified Capabilities
- `weight-tracking`: Domänenspezifische Methodennamen in `IDatabaseService` (Umbenennung bestehender CRUD-Methoden); Navigation-/Dialog-Entkopplung in `MainViewModel` und `AddEditWeightViewModel`
- `local-storage`: Methodennamen-Umbenennung in `IDatabaseService` und `DatabaseService`; kein Breaking Change im Verhalten
- `localization`: Verbleibende hard-codierte Strings lokalisieren; Entfernung der ungenutzten `Microsoft.Extensions.Localization`-Abhängigkeit

## Impact

- **`BodyMeasurement.csproj`**: Paket `Microsoft.Extensions.Localization` entfernen; View+ViewModel-Registrierung refactoren
- **`BodyMeasurement.Tests.csproj`**: Umstieg auf `<ProjectReference>`, xUnit v3, Microsoft Testing Platform; neue Service-Abstractions testen
- **`BodyMeasurement.sln`** → **`BodyMeasurement.slnx`**: Neue Solution-Datei; alte `.sln` kann entfernt werden
- **`IDatabaseService.cs` / `DatabaseService.cs`**: Alle 6 Methoden umbenennen (mit Anpassung aller Aufrufer)
- **`MainViewModel.cs` / `AddEditWeightViewModel.cs` / `SettingsViewModel.cs`**: `INavigationService` injizieren; `Debug.WriteLine` → `ILogger`; hard-codierte Strings ersetzen
- **`ChartViewModel.cs` / `StatisticsViewModel.cs`**: `Debug.WriteLine` → `ILogger`
- **`MauiProgram.cs`**: `AddViewWithViewModel`-Extension einführen; neue Services registrieren
- **`AppShell.xaml`**: Icons für Chart/Statistik-Tab korrigieren
- **`.github/workflows/build-and-test.yml`**: Branch-Strategie anpassen; Ubuntu-Testjob reparieren; Windows/macOS-Jobs korrigieren
- **Bestehende Tests**: Alle Aufrufe auf umbenannte DB-Methoden anpassen; Mock für `INavigationService` in ViewModel-Tests hinzufügen
