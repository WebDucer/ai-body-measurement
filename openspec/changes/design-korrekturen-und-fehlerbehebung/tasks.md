## 1. Infrastruktur & Tooling

- [ ] 1.1 `BodyMeasurement.slnx` erstellen und alle Projekte eintragen
- [ ] 1.2 `BodyMeasurement.sln` löschen
- [ ] 1.3 In `BodyMeasurement.csproj`: `net10.0` als zusätzliches TargetFramework hinzufügen; MAUI-spezifische Abhängigkeiten per `$(TargetFramework.Contains('-'))` konditionieren
- [ ] 1.4 In `BodyMeasurement.csproj`: `<PackageReference>` für `Microsoft.Extensions.Localization` entfernen
- [ ] 1.5 In `BodyMeasurement.Tests.csproj`: alle `<Compile Include="...">` File-Links entfernen und durch `<ProjectReference Include="..\BodyMeasurement\BodyMeasurement.csproj" />` ersetzen
- [ ] 1.6 In `BodyMeasurement.Tests.csproj`: xUnit 2.x-Pakete ersetzen durch `xunit.v3` 3.2.2 und `xunit.runner.visualstudio` 3.1.5; `<OutputType>Exe</OutputType>` und `<TestingPlatformDotnetTestSupport>true</TestingPlatformDotnetTestSupport>` hinzufügen

## 2. CI/CD-Pipeline

- [ ] 2.1 Im `test`-Job: MAUI-Workload-Installation entfernen; Testprojekt direkt mit `dotnet test` aufrufen
- [ ] 2.2 Im `build-windows`-Job: `dotnet restore` mit `-r win-x64` ergänzen
- [ ] 2.3 Im `build-macos`-Job: Xcode-Version explizit pinnen (`xcode-version: '16.x'`)
- [ ] 2.4 Branch-Strategie einführen: `build-windows` und `build-macos` Jobs mit `if: github.ref == 'refs/heads/main'` einschränken
- [ ] 2.5 Solution-Referenz in der Pipeline von `BodyMeasurement.sln` auf `BodyMeasurement.slnx` umstellen

## 3. INavigationService

- [ ] 3.1 `Services/INavigationService.cs` anlegen mit Methoden: `OpenAddMeasurementAsync()`, `OpenEditMeasurementAsync(int measurementId)`, `GoBackAsync()`, `ShowConfirmationAsync(...)`, `ShowAlertAsync(...)`
- [ ] 3.2 `Services/ShellNavigationService.cs` anlegen; `OpenEditMeasurementAsync` nutzt `ShellNavigationQueryParameters` mit Key `"MeasurementId"` und `int`-Wert
- [ ] 3.3 `INavigationService` und `ShellNavigationService` in `MauiProgram.cs` als Singleton registrieren

## 4. ViewModel-Entkopplung

- [ ] 4.1 In `AddEditWeightViewModel.cs`: `[QueryProperty]`-Attribut entfernen; `IQueryAttributable` implementieren; `ApplyQueryAttributes` liest `"MeasurementId"` als `int` via Pattern Matching
- [ ] 4.2 In `MainViewModel.cs`: `Shell.Current.GoToAsync(...)` durch `_navigationService.OpenAddMeasurementAsync()` und `_navigationService.OpenEditMeasurementAsync(id)` ersetzen
- [ ] 4.3 In `MainViewModel.cs`: `Application.Current!.MainPage!.DisplayAlert(...)` durch `_navigationService.ShowConfirmationAsync(...)` und `_navigationService.ShowAlertAsync(...)` ersetzen
- [ ] 4.4 In `AddEditWeightViewModel.cs`: `Shell.Current.GoToAsync("..")` durch `_navigationService.GoBackAsync()` ersetzen; `DisplayAlert`-Aufrufe durch `_navigationService.ShowAlertAsync(...)` ersetzen
- [ ] 4.5 In `SettingsViewModel.cs`: `DisplayAlert`-Aufrufe durch `_navigationService.ShowAlertAsync(...)` / `_navigationService.ShowConfirmationAsync(...)` ersetzen (außer `Application.Current`-Aufruf für Window-Restart, der bleibt direkt)
- [ ] 4.6 `INavigationService` per Constructor Injection in `MainViewModel`, `AddEditWeightViewModel` und `SettingsViewModel` einbinden

## 5. ILogger statt Debug.WriteLine

- [ ] 5.1 In `MainViewModel.cs`: alle `System.Diagnostics.Debug.WriteLine(...)` durch `_logger.LogError(ex, "...")` bzw. `_logger.LogWarning(...)` ersetzen; `ILogger<MainViewModel>` per Constructor Injection einbinden
- [ ] 5.2 In `AddEditWeightViewModel.cs`: analog zu 5.1
- [ ] 5.3 In `ChartViewModel.cs`: analog zu 5.1
- [ ] 5.4 In `StatisticsViewModel.cs`: analog zu 5.1
- [ ] 5.5 In `SettingsViewModel.cs`: analog zu 5.1
- [ ] 5.6 In `DatabaseService.cs` und weiteren Services: analog zu 5.1 (falls vorhanden)

## 6. Domänenspezifische Methodennamen in IDatabaseService

- [ ] 6.1 In `IDatabaseService.cs`: 6 Methoden umbenennen (`InsertWeightEntryAsync` → `RecordMeasurementAsync`, `GetAllWeightEntriesAsync` → `GetMeasurementHistoryAsync`, `GetWeightEntryByIdAsync` → `FindMeasurementAsync`, `UpdateWeightEntryAsync` → `UpdateMeasurementAsync`, `DeleteWeightEntryAsync` → `RemoveMeasurementAsync`, `GetWeightEntriesInDateRangeAsync` → `GetMeasurementsInPeriodAsync`)
- [ ] 6.2 In `DatabaseService.cs`: Implementierung entsprechend umbenennen
- [ ] 6.3 Alle Aufrufer anpassen: `MainViewModel`, `AddEditWeightViewModel`, `SettingsViewModel`, `ChartViewModel`, `StatisticsService`
- [ ] 6.4 Alle Test-Dateien auf neue Methodennamen aktualisieren

## 7. AddViewWithViewModel-Extension

- [ ] 7.1 `Extensions/ServiceCollectionExtensions.cs` anlegen mit `AddViewWithViewModel<TView, TViewModel>` (Constraints: `where TView : Page`, `where TViewModel : ObservableObject`)
- [ ] 7.2 In `MauiProgram.cs`: View+ViewModel-Registrierungen auf `AddViewWithViewModel<TView, TViewModel>()` umstellen

## 8. Shell-Icons & Material Icons

- [ ] 8.1 `MaterialIcons-Regular.ttf` als `MauiFont`-Ressource in `BodyMeasurement/Resources/Fonts/` ablegen
- [ ] 8.2 In `MauiProgram.cs`: `fonts.AddFont("MaterialIcons-Regular.ttf", "MaterialIcons")` registrieren
- [ ] 8.3 In `AppShell.xaml`: Icons für alle vier Tabs auf `FontImage`-Glyphen umstellen (`home` &#xE88A;, `bar_chart` &#xE26B;, `show_chart` &#xE6E1;, `settings` &#xE8B8;)

## 9. FAB-Fix auf Android

- [ ] 9.1 In `MainPage.xaml`: Am FAB-Button `Padding="0"`, `LineHeight="1"` setzen und `FontSize` von 32 auf 28 reduzieren

## 10. Lokalisierung — fehlende Strings

- [ ] 10.1 In `Strings.resx` und `Strings.de.resx`: neue Keys für alle hard-codierten Strings ergänzen (`ConfirmDeleteTitle`, `ConfirmDeleteMessage`, `Yes`, `No`, `ErrorTitle`, `ErrorDeleteMeasurement`, `AddWeightTitle`, `EditWeightTitle`, `ValidationWeightPositive`, `ValidationDateNotFuture`, `ErrorSaveMeasurement`, `LanguageChangedTitle`, `LanguageChangedMessage`, `NoDataTitle`, `NoDataExportMessage`, `ExportSuccessTitle`, `ExportSuccessMessage`, `ErrorExportData`, `EmptyMeasurements`, `Edit`, `Delete`)
- [ ] 10.2 In ViewModels: hard-codierte Strings durch Aufrufe auf den Lokalisierungs-Service ersetzen (nach Abschluss von Task 4.3–4.5, da Strings in Dialog-Aufrufen stecken)
- [ ] 10.3 In `MainPage.xaml`: `EmptyView`-Text und SwipeItem-Labels auf lokalisierte Bindings umstellen

## 11. Tests aktualisieren & ergänzen

- [ ] 11.1 Bestehende Tests: `IAsyncLifetime`-Implementierungen von `Task` auf `ValueTask` migrieren (xUnit v3 Breaking Change)
- [ ] 11.2 Bestehende Tests: alle veralteten `IDatabaseService`-Methodennamen auf neue domänenspezifische Namen aktualisieren
- [ ] 11.3 Mock für `INavigationService` in bestehende ViewModel-Tests einbinden (ersetzen vorherige Direktzugriffe)
- [ ] 11.4 Neue Tests für `ShellNavigationService` — prüfen, dass `OpenEditMeasurementAsync` die korrekten `ShellNavigationQueryParameters` übergibt
- [ ] 11.5 Neue Tests für `AddEditWeightViewModel.ApplyQueryAttributes` — prüfen, dass Edit-Modus korrekt gesetzt wird und `LoadWeightEntryAsync` aufgerufen wird
- [ ] 11.6 Neue Tests für `ServiceCollectionExtensions.AddViewWithViewModel` — prüfen, dass beide Typen im DI-Container registriert werden

## 12. Build-Warnungen bereinigen

- [ ] 12.1 `dotnet build` ausführen und alle 26 Warnungen analysieren
- [ ] 12.2 Warnungen beheben (nullable reference warnings, obsolete API-Aufrufe, nicht verwendete Variablen etc.)
- [ ] 12.3 Verbleibende nicht-behebbare Warnungen mit `#pragma warning disable` und Begründung unterdrücken
