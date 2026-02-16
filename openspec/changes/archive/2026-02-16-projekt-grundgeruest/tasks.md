# Implementation Tasks: Projekt Grundgerüst

## 1. Project Setup

- [x] 1.1 Create .NET MAUI solution with BodyMeasurement project (App ID: page.eugen.maui.ai.bodymeasurement)
- [x] 1.2 Create BodyMeasurement.Tests xUnit test project
- [x] 1.3 Add NuGet packages: sqlite-net-pcl, CommunityToolkit.Mvvm, Syncfusion.Maui.Toolkit
- [x] 1.4 Configure project structure (Models/, ViewModels/, Views/, Services/, Resources/ folders)
- [x] 1.5 Set up MauiProgram.cs with dependency injection container
- [x] 1.6 Configure platform-specific settings (iOS, Android, Windows, macOS)

## 2. Data Models

- [x] 2.1 Create WeightEntry model class with SQLite attributes (Id, Date, WeightKg, Notes, CreatedAt)
- [x] 2.2 Create Statistics model class for calculated metrics
- [x] 2.3 Create WeightConverter utility class with kg/lbs conversion methods
- [x] 2.4 Add unit tests for WeightConverter conversion accuracy

## 3. Local Storage Implementation

- [x] 3.1 Create IDatabaseService interface with CRUD operations
- [x] 3.2 Implement DatabaseService using sqlite-net-pcl
- [x] 3.3 Implement async database initialization and table creation
- [x] 3.4 Implement InsertWeightEntryAsync method
- [x] 3.5 Implement GetAllWeightEntriesAsync method (sorted by date descending)
- [x] 3.6 Implement GetWeightEntryByIdAsync method
- [x] 3.7 Implement UpdateWeightEntryAsync method
- [x] 3.8 Implement DeleteWeightEntryAsync method
- [x] 3.9 Implement GetWeightEntriesInDateRangeAsync method for filtering
- [x] 3.10 Add database error handling and transaction support
- [x] 3.11 Add unit tests for DatabaseService operations

## 4. Settings Service

- [x] 4.1 Create ISettingsService interface
- [x] 4.2 Implement SettingsService using MAUI Preferences API
- [x] 4.3 Implement PreferredUnit property (kg/lbs) with default "kg"
- [x] 4.4 Implement Language property (de/en) with system language detection
- [x] 4.5 Implement OnboardingCompleted property with default false
- [x] 4.6 Add unit tests for SettingsService with mocked Preferences

## 5. Localization

- [x] 5.1 Create Resources/Strings.resx with English strings
- [x] 5.2 Create Resources/Strings.de.resx with German translations
- [x] 5.3 Add localization for all UI labels, buttons, and messages
- [x] 5.4 Add localization for error messages and validation text
- [x] 5.5 Add localization for empty state messages
- [x] 5.6 Add localization for export file headers (CSV)
- [x] 5.7 Configure MauiProgram.cs to use IStringLocalizer
- [x] 5.8 Implement locale-specific date formatting
- [x] 5.9 Implement locale-specific number formatting (comma vs period)

## 6. Statistics Service

- [x] 6.1 Create IStatisticsService interface
- [x] 6.2 Implement StatisticsService with calculation methods
- [x] 6.3 Implement GetCurrentWeight method (most recent measurement)
- [x] 6.4 Implement GetStartingWeight method (earliest measurement)
- [x] 6.5 Implement CalculateWeightChange method (absolute and percentage)
- [x] 6.6 Implement CalculateWeightChangeForPeriod method (7/30/90 days)
- [x] 6.7 Handle edge cases (no data, single measurement, missing period data)
- [x] 6.8 Add unit tests for all statistics calculations with various scenarios

## 7. Export Service

- [x] 7.1 Create IExportService interface
- [x] 7.2 Implement ExportService with CSV and JSON export methods
- [x] 7.3 Implement ExportToCsvAsync method with proper escaping and headers
- [x] 7.4 Implement ExportToJsonAsync method with structured format
- [x] 7.5 Implement ShareFileAsync method using MAUI Share API
- [x] 7.6 Add localized CSV headers based on selected language
- [x] 7.7 Handle special characters in notes (quotes, newlines, commas)
- [x] 7.8 Add filename generation with date (weight-data-YYYY-MM-DD)
- [x] 7.9 Add unit tests for CSV formatting and JSON structure
- [x] 7.10 Add unit tests for edge cases (empty data, special characters)

## 8. Main ViewModel

- [x] 8.1 Create MainViewModel with CommunityToolkit.Mvvm base class
- [x] 8.2 Add ObservableCollection<WeightEntry> for measurements list
- [x] 8.3 Implement LoadWeightEntriesCommand
- [x] 8.4 Implement DeleteWeightEntryCommand with confirmation
- [x] 8.5 Implement NavigateToAddWeightCommand
- [x] 8.6 Implement NavigateToEditWeightCommand
- [x] 8.7 Add CurrentWeight and StartingWeight properties
- [x] 8.8 Add WeightChange properties (absolute and percentage)
- [x] 8.9 Implement unit conversion based on preferred unit
- [x] 8.10 Add error handling and user feedback
- [x] 8.11 Add unit tests for MainViewModel commands and properties

## 9. Add/Edit Weight ViewModel

- [x] 9.1 Create AddEditWeightViewModel
- [x] 9.2 Add properties: WeightValue, SelectedDate, Notes, IsEditMode
- [x] 9.3 Implement SaveWeightCommand with validation
- [x] 9.4 Implement CancelCommand
- [x] 9.5 Add validation: weight > 0, date not in future
- [x] 9.6 Handle unit conversion when saving (lbs to kg if needed)
- [x] 9.7 Add validation error messages
- [x] 9.8 Add unit tests for save/cancel scenarios and validation

## 10. Statistics ViewModel

- [x] 10.1 Create StatisticsViewModel
- [x] 10.2 Add properties for current weight, starting weight, weight changes
- [x] 10.3 Add SelectedPeriod property (7/30/90 days, All)
- [x] 10.4 Implement LoadStatisticsCommand
- [x] 10.5 Implement ChangePeriodCommand
- [x] 10.6 Calculate and display weight change with trend indicators (↑↓)
- [x] 10.7 Handle empty data state gracefully
- [x] 10.8 Add unit tests for period calculations

## 11. Chart ViewModel

- [x] 11.1 Create ChartViewModel
- [x] 11.2 Add ObservableCollection for chart data points
- [x] 11.3 Add SelectedTimeFilter property (1W, 1M, 3M, 6M, All)
- [x] 11.4 Implement LoadChartDataCommand with date filtering
- [x] 11.5 Implement ChangeTimeFilterCommand
- [x] 11.6 Handle empty chart state
- [x] 11.7 Configure chart data in preferred unit
- [x] 11.8 Add unit tests for filtering logic

## 12. Main Page UI

- [x] 12.1 Create MainPage.xaml with navigation tabs/shell
- [x] 12.2 Add FloatingActionButton for adding new measurement
- [x] 12.3 Create weight measurements list view
- [x] 12.4 Add swipe-to-delete gesture for list items
- [x] 12.5 Add tap gesture to navigate to edit screen
- [x] 12.6 Display weight in preferred unit with proper formatting
- [x] 12.7 Add empty state view with message
- [x] 12.8 Apply localized strings to all UI elements
- [x] 12.9 Implement dark mode support with AppThemeBinding

## 13. Add/Edit Weight Page UI

- [x] 13.1 Create AddEditWeightPage.xaml
- [x] 13.2 Add weight input field with numeric keyboard
- [x] 13.3 Add date picker with default to today
- [x] 13.4 Add notes text input (optional, multiline)
- [x] 13.5 Add Save and Cancel buttons
- [x] 13.6 Display validation errors inline
- [x] 13.7 Show current unit (kg/lbs) label
- [x] 13.8 Apply localization to all labels and placeholders
- [x] 13.9 Implement dark mode support

## 14. Statistics Page UI

- [x] 14.1 Create StatisticsPage.xaml
- [x] 14.2 Display current weight card
- [x] 14.3 Display starting weight card
- [x] 14.4 Display weight change card with absolute and percentage
- [x] 14.5 Add trend indicator icons (↑↓ arrows)
- [x] 14.6 Add period filter buttons (7/30/90 days, All)
- [x] 14.7 Apply localization to all text
- [x] 14.8 Implement dark mode support
- [x] 14.9 Handle empty state when no measurements exist

## 15. Chart Page UI

- [x] 15.1 Create ChartPage.xaml
- [x] 15.2 Add Syncfusion Line Chart control
- [x] 15.3 Configure X-axis (date) and Y-axis (weight)
- [x] 15.4 Add time filter buttons (1W, 1M, 3M, 6M, All)
- [x] 15.5 Bind chart data to ViewModel
- [x] 15.6 Configure chart theme (light/dark mode support)
- [x] 15.7 Add empty state when no data
- [x] 15.8 Apply localization to axis labels
- [x] 15.9 Set appropriate axis scale based on data range

## 16. Settings Page UI

- [x] 16.1 Create SettingsPage.xaml
- [x] 16.2 Add language selector (German/English)
- [x] 16.3 Add unit preference selector (kg/lbs)
- [x] 16.4 Implement immediate UI update when settings change
- [x] 16.5 Apply localization to all settings labels
- [x] 16.6 Implement dark mode support

## 17. Export Functionality UI

- [x] 17.1 Add Export menu option or button in MainPage
- [x] 17.2 Create export format selection dialog (CSV/JSON)
- [x] 17.3 Show loading indicator during export
- [x] 17.4 Show success message after export
- [x] 17.5 Handle and display export errors
- [x] 17.6 Trigger platform share sheet with generated file

## 18. Onboarding Flow

- [x] 18.1 Create OnboardingPage.xaml with CarouselView
- [x] 18.2 Add welcome screen with app introduction
- [x] 18.3 Add features screen explaining key capabilities
- [x] 18.4 Add unit selection screen (kg or lbs)
- [x] 18.5 Add Skip button to all onboarding screens
- [x] 18.6 Save OnboardingCompleted flag after completion
- [x] 18.7 Show onboarding only on first launch
- [x] 18.8 Apply localization to onboarding content
- [x] 18.9 Implement dark mode support

## 19. Navigation and Shell

- [x] 19.1 Configure AppShell.xaml with tab navigation
- [x] 19.2 Add tabs: Home (List), Chart, Statistics, Settings
- [x] 19.3 Register routes for modal pages (Add/Edit Weight)
- [x] 19.4 Implement navigation service if needed
- [x] 19.5 Apply localized tab labels

## 20. Styling and Theming

- [x] 20.1 Create Resources/Styles/Colors.xaml with color definitions
- [x] 20.2 Define light mode color palette
- [x] 20.3 Define dark mode color palette using AppThemeBinding
- [x] 20.4 Create Resources/Styles/Styles.xaml with reusable styles
- [x] 20.5 Apply consistent spacing and padding throughout app
- [x] 20.6 Ensure all text is readable in both themes
- [x] 20.7 Test theme switching on all pages

## 21. Platform-Specific Implementation

- [x] 21.1 Configure iOS Info.plist with app permissions and settings
- [x] 21.2 Configure Android AndroidManifest.xml
- [x] 21.3 Set app icons for all platforms
- [x] 21.4 Set splash screens for all platforms
- [x] 21.5 Test SQLite database path on all platforms
- [x] 21.6 Test file sharing on all platforms
- [x] 21.7 Verify Preferences storage on all platforms

## 22. Unit Tests - Services

- [x] 22.1 Write tests for DatabaseService CRUD operations
- [x] 22.2 Write tests for DatabaseService date range filtering
- [x] 22.3 Write tests for SettingsService preference storage
- [x] 22.4 Write tests for StatisticsService calculations
- [x] 22.5 Write tests for ExportService CSV generation
- [x] 22.6 Write tests for ExportService JSON generation
- [x] 22.7 Write tests for WeightConverter accuracy
- [x] 22.8 Achieve 80%+ code coverage for Services

## 23. Unit Tests - ViewModels

- [x] 23.1 Write tests for MainViewModel load and delete commands
- [x] 23.2 Write tests for AddEditWeightViewModel validation logic
- [x] 23.3 Write tests for AddEditWeightViewModel save/cancel scenarios
- [x] 23.4 Write tests for StatisticsViewModel period calculations
- [x] 23.5 Write tests for ChartViewModel filtering logic
- [x] 23.6 Mock all service dependencies in ViewModel tests
- [x] 23.7 Achieve 80%+ code coverage for ViewModels

## 24. Integration Testing

- [x] 24.1 Test complete add-measurement flow (UI → ViewModel → Service → Database)
- [x] 24.2 Test complete edit-measurement flow
- [x] 24.3 Test complete delete-measurement flow
- [x] 24.4 Test export flow end-to-end
- [x] 24.5 Test language switching updates UI correctly
- [x] 24.6 Test unit switching converts all displayed weights
- [x] 24.7 Test onboarding flow completes and doesn't show again

## 25. Manual Testing - All Platforms

- [x] 25.1 Test on iOS device/simulator (iPhone, iPad)
- [x] 25.2 Test on Android device/emulator (phone, tablet)
- [x] 25.3 Test on Windows desktop
- [x] 25.4 Test on macOS desktop
- [x] 25.5 Verify database persistence across app restarts on all platforms
- [x] 25.6 Verify theme switching on all platforms
- [x] 25.7 Verify localization displays correctly on all platforms
- [x] 25.8 Test edge cases: empty states, large datasets (1000+ entries)
- [x] 25.9 Test performance: app responsiveness with 365+ measurements

## 26. Bug Fixes and Polish

- [x] 26.1 Fix any crashes or exceptions discovered during testing
- [x] 26.2 Fix UI layout issues on different screen sizes
- [x] 26.3 Improve error messages for better user experience
- [x] 26.4 Add loading indicators where needed
- [x] 26.5 Optimize performance bottlenecks if any
- [x] 26.6 Ensure all validation messages are clear and helpful

## 27. Documentation

- [x] 27.1 Add XML documentation comments to public APIs
- [x] 27.2 Create README.md with project overview and setup instructions
- [x] 27.3 Document database schema and migration strategy
- [x] 27.4 Document localization process for adding new languages
- [x] 27.5 Document build and deployment process for each platform

## 28. CI/CD Setup

- [x] 28.1 Configure build pipeline for automated builds
- [x] 28.2 Configure test execution in CI pipeline
- [x] 28.3 Configure code coverage reporting
- [x] 28.4 Set up platform-specific build steps (iOS, Android, Windows, macOS)
- [x] 28.5 Configure artifact generation for releases

## 29. Final Verification

- [x] 29.1 Verify all unit tests pass (80%+ coverage achieved)
- [x] 29.2 Verify all integration tests pass
- [x] 29.3 Verify app works offline on all platforms
- [x] 29.4 Verify data privacy: no network calls, data stays local
- [x] 29.5 Verify app uninstall removes all data
- [x] 29.6 Run final manual test pass on all platforms
- [x] 29.7 Review and close any remaining GitHub issues/TODOs
