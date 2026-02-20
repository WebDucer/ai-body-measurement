## Context

Die App ist eine .NET MAUI Single-Project-App (BodyMeasurement) mit einer zugehörigen xUnit-Testbibliothek (BodyMeasurement.Tests). Die Architektur folgt MVVM mit CommunityToolkit.Mvvm. Aktuell sind ViewModels direkt an MAUI-UI-APIs gekoppelt (`Shell.Current`, `Application.Current.MainPage`), was sowohl Testbarkeit als auch plattformübergreifende Portierbarkeit verhindert. Das Testprojekt kompiliert Quelldateien über `<Compile Include="...">` File-Linking statt über eine echte Projektreferenz — was zu LSP-Ambiguity-Fehlern führt und die Testbarkeit von ViewModels komplett verhindert. Die CI/CD-Pipeline ist für alle drei Plattformbuild-Jobs broken.

## Goals / Non-Goals

**Goals:**
- Kritischen Edit-Absturz (`InvalidCastException` bei Shell-Navigation) beheben
- CI/CD-Pipeline vollständig reparieren und Branch-Strategie kostenoptimiert einführen
- Testprojekt auf echte `<ProjectReference>` mit xUnit v3 + Microsoft Testing Platform migrieren
- ViewModels durch `INavigationService` (vereint Navigation und Dialoge) von MAUI-UI-APIs entkoppeln
- `Debug.WriteLine` durchgehend durch `ILogger<T>` ersetzen
- Domänenspezifische Methodennamen in `IDatabaseService` einführen
- Ungenutzte `Microsoft.Extensions.Localization`-Abhängigkeit entfernen
- Hard-codierte UI-Strings lokalisieren
- Shell-Icons für Chart und Statistik reparieren
- FAB auf Android optisch korrigieren
- Solution auf `.slnx`-Format migrieren
- View+ViewModel-Registrierung in lesbare Extension auslagern

**Non-Goals:**
- Keine neuen fachlichen Features
- Kein Wechsel des MVVM-Frameworks (CommunityToolkit.Mvvm bleibt)
- Kein Wechsel der Datenbank-Bibliothek (sqlite-net-pcl bleibt)
- Keine neue Plattformunterstützung
- Kein komplettes Testprojekt-Rewrite (bestehende Tests bleiben, werden nur angepasst)

## Decisions

### D1: Edit-Absturz + `INavigationService` — domänenspezifisches Interface mit typsicherer Parameterübergabe

**Problem:** `MainViewModel.NavigateToEditWeightAsync(int entryId)` navigiert via `Shell.Current.GoToAsync($"editweight?id={entryId}")`. MAUI versucht, den Query-String-Wert `"123"` direkt als `Nullable<int>` zu casten — das schlägt mit `InvalidCastException` fehl. Zusätzlich ist `[QueryProperty]` laut Microsoft-Dokumentation nicht trim-safe. Weiterhin sind ViewModels durch direkte Aufrufe von `Shell.Current` und `Application.Current.MainPage.DisplayAlert` nicht standalone-testbar.

**Entscheidung:** Ein domänenspezifisches `INavigationService`-Interface, das sowohl Navigation als auch Dialoge abdeckt. Navigationsmethoden sind explizit benannt — keine generischen `GoToAsync(string)`-Methoden im Interface. Die Implementierung verwendet `ShellNavigationQueryParameters` für typsichere Parameterübergabe und `IQueryAttributable` auf der Empfängerseite.

Interface:
```csharp
// Services/INavigationService.cs
public interface INavigationService
{
    Task OpenAddMeasurementAsync();
    Task OpenEditMeasurementAsync(int measurementId);
    Task GoBackAsync();
    Task<bool> ShowConfirmationAsync(string title, string message, string accept, string cancel);
    Task ShowAlertAsync(string title, string message, string cancel);
}
```

MAUI-Implementierung — nutzt `ShellNavigationQueryParameters` (single-use, kein Memory-Leak):
```csharp
// Services/ShellNavigationService.cs
public class ShellNavigationService : INavigationService
{
    public Task OpenAddMeasurementAsync() =>
        Shell.Current.GoToAsync("addweight");

    public Task OpenEditMeasurementAsync(int measurementId) =>
        Shell.Current.GoToAsync("editweight", new ShellNavigationQueryParameters
        {
            { "MeasurementId", measurementId }
        });

    public Task GoBackAsync() => Shell.Current.GoToAsync("..");

    public Task<bool> ShowConfirmationAsync(string title, string message, string accept, string cancel) =>
        Application.Current!.MainPage!.DisplayAlert(title, message, accept, cancel);

    public Task ShowAlertAsync(string title, string message, string cancel) =>
        Application.Current!.MainPage!.DisplayAlert(title, message, cancel);
}
```

Empfänger (`AddEditWeightViewModel` implementiert `IQueryAttributable` statt `[QueryProperty]`):
```csharp
public partial class AddEditWeightViewModel : ObservableObject, IQueryAttributable
{
    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("MeasurementId", out var value) && value is int id)
        {
            IsEditMode = true;
            _ = LoadWeightEntryAsync(id);
        }
    }
}
```

ViewModels erhalten `INavigationService` per Constructor Injection. `SettingsViewModel.OnSelectedLanguageChanged` verwendet zusätzlich `Application.Current` direkt für den Window-Restart — dieser bleibt ungekapselt, da er App-Lifecycle-spezifisch ist und kein sinnvolles Interface-Szenario hat.

**Alternative verworfen:** Generisches `GoToAsync(string route, ...)` im Interface — zu nah an der MAUI-Implementierung, schwer zu lesen, erfordert im Test Kenntnis von Routen-Strings.  
**Alternative verworfen:** `[QueryProperty]` — nicht trim-safe, erfordert String-Parsing für primitive Typen.  
**Alternative verworfen:** Zwei separate Interfaces (`INavigationService` + `IAlertService`) — unnötige Aufteilung, da ein Mock im Test ausreicht.

---

### D3: Testprojekt — `<ProjectReference>` statt File-Linking

**Problem:** Das aktuelle File-Linking (`<Compile Include="...">`) verursacht:
- LSP-Ambiguity-Fehler (Typen doppelt definiert aus Sicht des Language Servers)
- ViewModels sind nicht testbar (MAUI-Dependencies können nicht aufgelöst werden)
- Wartungsaufwand bei jeder neuen Datei

**Entscheidung:** Umstieg auf `<ProjectReference>` gemäß [Microsoft MAUI Unit Testing Doku](https://learn.microsoft.com/en-us/dotnet/maui/deployment/unit-testing). Das Testprojekt bleibt auf `net10.0` (plain .NET), referenziert aber das MAUI-Projekt. MAUI-spezifische Typen werden per Moq-Interfaces abstrahiert.

```xml
<!-- BodyMeasurement.Tests.csproj -->
<ItemGroup>
  <ProjectReference Include="..\BodyMeasurement\BodyMeasurement.csproj" />
</ItemGroup>
```

Da das MAUI-Projekt `net10.0-android` etc. als TargetFrameworks hat, muss das Testprojekt entweder `net10.0-android` targeten oder das Haupt-Projekt bekommt `net10.0` als zusätzliches (non-MAUI) TargetFramework für reine Logik-Tests. 

**Entscheidung:** Das Testprojekt targetet weiterhin `net10.0`. Das Haupt-Projekt erhält `net10.0` als zusätzliches TargetFramework, wobei MAUI-spezifische Abhängigkeiten per `$(TargetFramework.Contains('-'))` konditioniert werden — dieser Ansatz ist gemäß Microsoft-Dokumentation der empfohlene Weg.

---

### D4: xUnit v3 + Microsoft Testing Platform

**Problem:** xUnit 2.9.x + `Microsoft.NET.Test.Sdk` ist die alte Kombination. xUnit v3 nutzt die neue Microsoft Testing Platform nativ.

**Entscheidung:** Migration zu xUnit v3:

```xml
<!-- Vorher -->
<PropertyGroup>
  <TargetFramework>net10.0</TargetFramework>
  <IsPackable>false</IsPackable>
</PropertyGroup>
<PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.14.1" />
<PackageReference Include="xunit" Version="2.9.3" />
<PackageReference Include="xunit.runner.visualstudio" Version="3.1.4" />

<!-- Nachher (laut offiziellem xUnit v3 Getting-Started-Template) -->
<PropertyGroup>
  <TargetFramework>net10.0</TargetFramework>
  <OutputType>Exe</OutputType>  <!-- Pflicht für MTP-Runner -->
  <!-- dotnet test Unterstützung über VSTest-Adapter: -->
  <TestingPlatformDotnetTestSupport>true</TestingPlatformDotnetTestSupport>
</PropertyGroup>
<PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.13.0" />  <!-- bleibt -->
<PackageReference Include="xunit.v3" Version="3.2.2" />
<PackageReference Include="xunit.runner.visualstudio" Version="3.1.5">
  <PrivateAssets>all</PrivateAssets>
  <IncludeAssets>runtime; build; native; contentfiles; analyzers</IncludeAssets>
</PackageReference>
```

**Breaking Change in xUnit v3** der angepasst werden muss: `IAsyncLifetime` — `Task` wird zu `ValueTask`:

```csharp
// Vorher (v2):
public class MyFixture : IAsyncLifetime
{
    public Task InitializeAsync() { ... }
    public Task DisposeAsync() { ... }
}

// Nachher (v3):
public class MyFixture : IAsyncLifetime
{
    public ValueTask InitializeAsync() { ... }
    public ValueTask DisposeAsync() { ... }
}
```

Alle anderen verwendeten APIs (`[Fact]`, `[Theory]`, `[InlineData]`, `Assert.*`, `IClassFixture<T>`, Moq-Integration) sind v2-kompatibel — die Migration ist ansonsten minimal-invasiv.

---

### D5: CI/CD-Pipeline — Reparatur und Branch-Strategie

**Fehler und Ursachen:**

| Job | Fehler | Ursache | Fix |
|---|---|---|---|
| `test` (Ubuntu) | `maui isn't supported on this platform` | Ubuntu unterstützt kein `maui`-Workload für Tests | MAUI-Workload für Testjob entfernen; Testprojekt braucht ihn nicht |
| `build-windows` | `NETSDK1112: runtime pack not downloaded` | `dotnet restore` ohne `--runtime` auf Windows für MAUI | `dotnet restore BodyMeasurement/BodyMeasurement.csproj -r win-x64` |
| `build-macos` | Xcode 26.2 required, 16.4 installed | .NET 10 GA braucht Xcode 26.2, GitHub runner hat 16.4 | `macos-15` runner + `xcode-version: '16.x'` oder auf .NET 10 stable warten |

**Branch-Strategie:**

```yaml
# Test + Android: auf allen Branches
test:
  runs-on: ubuntu-latest
  # kein branch-filter

build-android:
  runs-on: ubuntu-latest
  needs: test

# Windows + macOS/iOS: nur auf main
build-windows:
  runs-on: windows-latest
  needs: test
  if: github.ref == 'refs/heads/main'

build-macos:
  runs-on: macos-latest
  needs: test
  if: github.ref == 'refs/heads/main'
```

Die Solution-Referenz in `dotnet restore BodyMeasurement.sln` wird nach der `.slnx`-Migration auf `BodyMeasurement.slnx` umgestellt.

---

### D6: Domänenspezifische Methodennamen in `IDatabaseService`

**Entscheidung:** Umbenennung der 6 CRUD-Methoden zu domänenspezifischen Namen:

| Alt | Neu |
|---|---|
| `InsertWeightEntryAsync` | `RecordMeasurementAsync` |
| `GetAllWeightEntriesAsync` | `GetMeasurementHistoryAsync` |
| `GetWeightEntryByIdAsync` | `FindMeasurementAsync` |
| `UpdateWeightEntryAsync` | `UpdateMeasurementAsync` |
| `DeleteWeightEntryAsync` | `RemoveMeasurementAsync` |
| `GetWeightEntriesInDateRangeAsync` | `GetMeasurementsInPeriodAsync` |

`InitializeAsync` bleibt unverändert (kein CRUD-Name, technisch korrekt).

Alle Aufrufer werden angepasst: `MainViewModel`, `AddEditWeightViewModel`, `SettingsViewModel`, `ChartViewModel`, `StatisticsService`, sowie alle Test-Dateien.

---

### D7: Shell-Icons — Emoji-Problem auf Android

**Problem:** `{FontImage Glyph='📊', Size=24}` rendert auf Android als schwarzes Rechteck, weil Android Emoji-Glyphen im `FontImage`-Kontext nicht als farbige Glyphen darstellt.

**Entscheidung:** Umstieg auf Material Design Symbols oder Material Icons Font (bereits in MAUI verfügbar via `Microsoft.Maui.Controls`). Alternativ: SVG-Icons als `MauiImage`-Ressourcen. 

Da die App bereits `OpenSans` einbindet, ist der einfachste Weg: Material Icons Font registrieren und Unicode-Codepoints verwenden:

```xml
<!-- MauiProgram.cs -->
fonts.AddFont("MaterialIcons-Regular.ttf", "MaterialIcons");

<!-- AppShell.xaml -->
<ShellContent Icon="{FontImage FontFamily=MaterialIcons, Glyph='&#xE88A;', Size=24}" ... /> <!-- home        e88a -->
<ShellContent Icon="{FontImage FontFamily=MaterialIcons, Glyph='&#xE26B;', Size=24}" ... /> <!-- bar_chart   e26b -->
<ShellContent Icon="{FontImage FontFamily=MaterialIcons, Glyph='&#xE6E1;', Size=24}" ... /> <!-- show_chart  e6e1 -->
<ShellContent Icon="{FontImage FontFamily=MaterialIcons, Glyph='&#xE8B8;', Size=24}" ... /> <!-- settings    e8b8 -->
```

`bar_chart` (Balkendiagramm, &#xe26b;) für den Chart-Tab und `show_chart` (Liniendiagramm, &#xe6e1;) für den Statistics-Tab spiegeln den Inhalt der jeweiligen Seite am besten wider.

**Alternative verworfen:** Font Awesome — externe Abhängigkeit ohne MAUI-nativen Support.

---

### D8: FAB auf Android — Plattformspezifisches Padding

**Problem:** Android-`Button` hat ein systemseitiges internes Padding/Inset-Verhalten, das den `+`-Text nach unten aus der Mitte schiebt. iOS ist nicht betroffen.

**Entscheidung:** `Padding="0"` auf dem Button setzen und `LineHeight="1"` hinzufügen. FontSize wird von 32 auf 28 reduziert, um korrekt in 60px zu passen:

```xml
<Button Text="+"
        FontSize="28"
        Padding="0"
        LineHeight="1"
        WidthRequest="60" HeightRequest="60" CornerRadius="30" ... />
```

---

### D9: `AddViewWithViewModel`-Extension in `MauiProgram.cs`

**Problem:** View- und ViewModel-Registrierung sind aktuell manuell getrennt:
```csharp
builder.Services.AddTransient<MainPage>();
builder.Services.AddTransient<MainViewModel>();
```

**Entscheidung:** Eine Extension-Methode für `IServiceCollection`:

```csharp
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddViewWithViewModel<TView, TViewModel>(
        this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Transient)
        where TView : Page
        where TViewModel : ObservableObject
    {
        services.Add(new ServiceDescriptor(typeof(TView), typeof(TView), lifetime));
        services.Add(new ServiceDescriptor(typeof(TViewModel), typeof(TViewModel), lifetime));
        return services;
    }
}
```

Aufruf in `MauiProgram.cs`:
```csharp
builder.Services
    .AddViewWithViewModel<MainPage, MainViewModel>()
    .AddViewWithViewModel<AddEditWeightPage, AddEditWeightViewModel>()
    .AddViewWithViewModel<StatisticsPage, StatisticsViewModel>()
    .AddViewWithViewModel<ChartPage, ChartViewModel>()
    .AddViewWithViewModel<SettingsPage, SettingsViewModel>();
builder.Services.AddTransient<OnboardingPage>(); // kein ViewModel
```

---

### D10: `Microsoft.Extensions.Localization` entfernen

Das Paket ist in `BodyMeasurement.csproj` referenziert, wird aber nicht genutzt. Die Lokalisierung läuft vollständig über den selbst implementierten `LocalizationService` mit `System.Resources.ResourceManager`. Einfaches Entfernen des `<PackageReference>`-Eintrags genügt.

---

### D11: Hard-codierte Strings lokalisieren

Folgende Strings in ViewModels und XAML-Views sind noch nicht in `Strings.resx` / `Strings.de.resx`:

| Ort | Text | Neuer Key |
|---|---|---|
| `MainViewModel` | `"Confirm Delete"` | `ConfirmDeleteTitle` |
| `MainViewModel` | `"Are you sure you want to delete this measurement?"` | `ConfirmDeleteMessage` |
| `MainViewModel` | `"Yes"` / `"No"` | `Yes` / `No` |
| `MainViewModel` | `"Error"` | `ErrorTitle` |
| `MainViewModel` | `"Failed to delete measurement"` | `ErrorDeleteMeasurement` |
| `AddEditWeightViewModel` | `"Add Weight"` / `"Edit Weight"` | `AddWeightTitle` / `EditWeightTitle` |
| `AddEditWeightViewModel` | `"Weight must be greater than zero"` | `ValidationWeightPositive` |
| `AddEditWeightViewModel` | `"Date cannot be in the future"` | `ValidationDateNotFuture` |
| `AddEditWeightViewModel` | `"Error"` / `"Failed to save measurement"` | `ErrorTitle` / `ErrorSaveMeasurement` |
| `SettingsViewModel` | `"Language Changed"` + Restart-Text | `LanguageChangedTitle` / `LanguageChangedMessage` |
| `SettingsViewModel` | `"No Data"` / `"No measurements to export"` | `NoDataTitle` / `NoDataExportMessage` |
| `SettingsViewModel` | `"Success"` / `"Data exported successfully"` | `ExportSuccessTitle` / `ExportSuccessMessage` |
| `SettingsViewModel` | `"Error"` / `"Failed to export data"` | `ErrorTitle` / `ErrorExportData` |
| `MainPage.xaml` | `EmptyView="No measurements yet..."` | `EmptyMeasurements` |
| `MainPage.xaml` | SwipeItem `Text="Edit"` / `Text="Delete"` | `Edit` / `Delete` |

---

### D12: `.slnx`-Format

`BodyMeasurement.sln` wird durch `BodyMeasurement.slnx` ersetzt. Das neue Format ist XML-basiert und einfacher zu lesen. Die `BodyMeasurement.sln`-Datei wird nach der Migration gelöscht.

---

### D13: `ILogger<T>` statt `Debug.WriteLine`

Alle `System.Diagnostics.Debug.WriteLine(...)` in ViewModels und Services werden durch `_logger.LogError(ex, "...")` bzw. `_logger.LogWarning(...)` ersetzt. Jedes ViewModel erhält `ILogger<T>` per Constructor Injection. `builder.Logging.AddDebug()` ist bereits im `#if DEBUG`-Block vorhanden.

## Risks / Trade-offs

- **Testprojekt + `net10.0` + `<ProjectReference>` auf MAUI-Projekt** → Das MAUI-Projekt muss `net10.0` als zusätzliches TFM haben. Builds werden minimal länger. Mitigation: MAUI-spezifische APIs per MSBuild-Condition ausschließen.
- **xUnit v3 Breaking Changes** → Wenige, aber vorhanden (insb. `IAsyncLifetime`-Signatur). Mitigation: alle Test-Dateien vor Migration auf Kompatibilität prüfen.
- **Domain-Methodennamen-Umbenennung** → Große Anzahl an Aufrufer-Dateien muss angepasst werden (5 ViewModels + 1 Service + alle Testdateien). Mitigation: Compiler-Fehler leiten die Änderungen; kein Runtime-Risiko.
- **Material Icons Font hinzufügen** → Zusätzliche Font-Datei (~300KB). Mitigation: Akzeptabel für Desktop/Mobile-App; tree-shaking durch MAUI-Linker möglich.
- **CI/CD macOS-Job** → Xcode-Version auf GitHub Actions-Runner für .NET 10 kann sich ändern. Mitigation: explizite `xcode-version` pinnen.
- **`INavigationService` in `SettingsViewModel.OnSelectedLanguageChanged`** → Dieser Callback ist synchron (`partial void`), der Alert ist aber async. Der `Dispatcher.Dispatch`-Wrapper bleibt direkt auf `Application.Current`, da kein sinnvolles Interface für App-Lifecycle passt. Trade-off: diese eine Stelle bleibt MAUI-gekoppelt.

## Open Questions

Keine offenen Fragen.
