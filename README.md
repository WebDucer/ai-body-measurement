# Body Measurement App

A cross-platform .NET MAUI application for tracking body weight measurements with local-first data storage.

## Features

- **Weight Tracking**: Record weight measurements with date, value, and optional notes
- **Multi-language Support**: German (de) and English (en) localization
- **Unit Conversion**: Support for both kg and lbs with automatic conversion
- **Data Visualization**: View weight history as a list and interactive charts
- **Statistics Dashboard**: Track progress with trends, averages, and weight changes
- **Data Export**: Export measurements to CSV or JSON formats
- **Offline-First**: All data stored locally using SQLite, no cloud dependency
- **Dark Mode**: Automatic dark mode support based on system preferences
- **Cross-Platform**: Runs on iOS, Android, Windows, and macOS

## Project Structure

```
BodyMeasurement/
├── Models/                    # Data models (WeightEntry, Statistics)
├── ViewModels/                # MVVM ViewModels with Commands
├── Views/                     # XAML Pages and Controls
├── Services/                  # Business logic services
│   ├── DatabaseService.cs    # SQLite database operations
│   ├── SettingsService.cs    # User preferences
│   ├── StatisticsService.cs  # Weight statistics calculations
│   └── ExportService.cs      # Data export functionality
├── Resources/                 # Localization, Styles, Images
├── Converters/                # Value converters (e.g., WeightConverter)
└── Platforms/                 # Platform-specific code

BodyMeasurement.Tests/
├── Models/                    # Model unit tests
├── Services/                  # Service unit tests
├── ViewModels/                # ViewModel unit tests
└── Integration/               # End-to-end integration tests
```

## Technology Stack

- **.NET 10**: Latest .NET framework
- **.NET MAUI**: Cross-platform UI framework
- **SQLite**: Local database (via sqlite-net-pcl)
- **CommunityToolkit.Mvvm**: MVVM helpers and commands
- **Syncfusion.Maui.Toolkit**: Charts and data visualization (MIT License)
- **xUnit**: Testing framework
- **Moq**: Mocking library for unit tests

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- Visual Studio 2022 (17.9+) or Visual Studio Code with C# extension
- Platform-specific tools:
  - **iOS**: macOS with Xcode 15+
  - **Android**: Android SDK 34+
  - **Windows**: Windows 11 SDK
  - **macOS**: macOS 13+ with Xcode

## Getting Started

### 1. Clone the Repository

```bash
git clone <repository-url>
cd ai-body-measurement
```

### 2. Restore Dependencies

```bash
dotnet restore
```

### 3. Build the Solution

```bash
dotnet build
```

### 4. Run Tests

```bash
dotnet test
```

### 5. Run the Application

#### Run on specific platform:

```bash
# Android
dotnet build -t:Run -f net10.0-android

# iOS (macOS only)
dotnet build -t:Run -f net10.0-ios

# Windows
dotnet build -t:Run -f net10.0-windows10.0.19041.0

# macOS
dotnet build -t:Run -f net10.0-maccatalyst
```

#### Using Visual Studio:
1. Open `BodyMeasurement.sln`
2. Select target platform from the toolbar
3. Press F5 or click Run

## Configuration

### App ID
- **App ID**: `page.eugen.maui.ai.bodymeasurement`

### Supported Languages
- **English (en)**: Default language
- **German (de)**: Secondary language

Language is automatically detected from system settings but can be changed in the app's Settings page.

### Supported Units
- **Kilograms (kg)**: Default unit
- **Pounds (lbs)**: Alternative unit

Unit preference can be set during onboarding or changed in Settings.

## Database

### Location

The SQLite database is stored in the app's local data directory:

- **iOS**: `~/Library/Application Support/<app-id>/bodymeasurement.db3`
- **Android**: `/data/data/<app-id>/files/bodymeasurement.db3`
- **Windows**: `%LOCALAPPDATA%\<app-id>\bodymeasurement.db3`
- **macOS**: `~/Library/Containers/<app-id>/Data/Library/Application Support/bodymeasurement.db3`

### Schema

See [docs/database-schema.md](docs/database-schema.md) for detailed database schema documentation.

## Localization

Localization uses standard .NET RESX resource files:

- **Resources/Strings.resx**: English (default)
- **Resources/Strings.de.resx**: German

See [docs/localization-guide.md](docs/localization-guide.md) for instructions on adding new languages.

## Testing

### Test Coverage

- **Unit Tests**: 80%+ coverage for Services and ViewModels
- **Integration Tests**: End-to-end flow tests for critical user journeys

### Run All Tests

```bash
dotnet test
```

### Run Specific Test Category

```bash
# Unit tests only
dotnet test --filter "FullyQualifiedName~Services"

# Integration tests only
dotnet test --filter "FullyQualifiedName~Integration"
```

### Test with Coverage Report

```bash
dotnet test --collect:"XPlat Code Coverage"
```

## Building for Production

### Android APK

```bash
dotnet publish -f net10.0-android -c Release
```

Output: `BodyMeasurement/bin/Release/net10.0-android/publish/`

### iOS IPA

```bash
dotnet publish -f net10.0-ios -c Release
```

Output: `BodyMeasurement/bin/Release/net10.0-ios/publish/`

### Windows MSIX

```bash
dotnet publish -f net10.0-windows10.0.19041.0 -c Release
```

Output: `BodyMeasurement/bin/Release/net10.0-windows10.0.19041.0/publish/`

### macOS APP

```bash
dotnet publish -f net10.0-maccatalyst -c Release
```

Output: `BodyMeasurement/bin/Release/net10.0-maccatalyst/publish/`

See [docs/build-deployment.md](docs/build-deployment.md) for detailed build and deployment instructions.

## CI/CD

The project uses automated CI/CD pipelines for:
- Automated builds on commit
- Test execution
- Code coverage reporting
- Platform-specific builds

See [docs/ci-cd-setup.md](docs/ci-cd-setup.md) for pipeline configuration details.

## Architecture

### MVVM Pattern

The app follows the Model-View-ViewModel (MVVM) pattern:

- **Models**: Data structures (e.g., `WeightEntry`, `Statistics`)
- **Views**: XAML UI pages (e.g., `MainPage`, `AddEditWeightPage`)
- **ViewModels**: Business logic and UI state (e.g., `MainViewModel`)
- **Services**: Data access and business operations

### Dependency Injection

Services are registered in `MauiProgram.cs`:

```csharp
// Services
builder.Services.AddSingleton<IDatabaseService, DatabaseService>();
builder.Services.AddSingleton<ISettingsService, SettingsService>();
builder.Services.AddSingleton<IExportService, ExportService>();
builder.Services.AddSingleton<IStatisticsService, StatisticsService>();

// ViewModels
builder.Services.AddTransient<MainViewModel>();
builder.Services.AddTransient<AddEditWeightViewModel>();
builder.Services.AddTransient<StatisticsViewModel>();
builder.Services.AddTransient<ChartViewModel>();

// Views
builder.Services.AddTransient<MainPage>();
builder.Services.AddTransient<AddEditWeightPage>();
builder.Services.AddTransient<StatisticsPage>();
builder.Services.AddTransient<ChartPage>();
```

## Data Privacy

- **Local-First**: All data is stored locally on the device
- **No Cloud**: No data is sent to external servers
- **No Analytics**: No telemetry or usage tracking
- **Data Portability**: Export your data anytime as CSV or JSON

## Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

### Code Style

- Follow C# coding conventions
- Use XML documentation for public APIs
- Maintain 80%+ test coverage for new code
- Run tests before committing

## License

[MIT License](LICENSE)

## Support

For questions or issues:
- Create an issue in the repository
- Check existing issues for solutions

## Roadmap

### V1 (Current)
- ✅ Weight tracking
- ✅ Multi-language support (de/en)
- ✅ Unit conversion (kg/lbs)
- ✅ Charts and statistics
- ✅ Data export (CSV/JSON)
- ✅ Dark mode

### V2 (Future)
- Cloud sync (optional)
- Additional body measurements (BMI, body fat, circumference)
- Goal setting and reminders
- Integration with health apps (Apple Health, Google Fit)
- Advanced statistics and trends
- More language support

## Acknowledgments

- [.NET MAUI](https://dotnet.microsoft.com/apps/maui) - Microsoft's cross-platform framework
- [Syncfusion .NET MAUI Toolkit](https://github.com/syncfusion/maui-toolkit) - Open-source charts and controls
- [CommunityToolkit.Mvvm](https://github.com/CommunityToolkit/dotnet) - MVVM helpers
- [sqlite-net-pcl](https://github.com/praeclarum/sqlite-net) - SQLite ORM
