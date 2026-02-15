# Implementation Tasks: Projekt Grundgerüst

## 1. Project Setup

- [ ] 1.1 Create .NET MAUI solution with BodyMeasurement project (App ID: page.eugen.maui.ai.bodymeasurement)
- [ ] 1.2 Create BodyMeasurement.Tests xUnit test project
- [ ] 1.3 Add NuGet packages: sqlite-net-pcl, CommunityToolkit.Mvvm, Syncfusion.Maui.Toolkit
- [ ] 1.4 Configure project structure (Models/, ViewModels/, Views/, Services/, Resources/ folders)
- [ ] 1.5 Set up MauiProgram.cs with dependency injection container
- [ ] 1.6 Configure platform-specific settings (iOS, Android, Windows, macOS)

## 2. Data Models

- [ ] 2.1 Create WeightEntry model class with SQLite attributes (Id, Date, WeightKg, Notes, CreatedAt)
- [ ] 2.2 Create Statistics model class for calculated metrics
- [ ] 2.3 Create WeightConverter utility class with kg/lbs conversion methods
- [ ] 2.4 Add unit tests for WeightConverter conversion accuracy

## 3. Local Storage Implementation

- [ ] 3.1 Create IDatabaseService interface with CRUD operations
- [ ] 3.2 Implement DatabaseService using sqlite-net-pcl
- [ ] 3.3 Implement async database initialization and table creation
- [ ] 3.4 Implement InsertWeightEntryAsync method
- [ ] 3.5 Implement GetAllWeightEntriesAsync method (sorted by date descending)
- [ ] 3.6 Implement GetWeightEntryByIdAsync method
- [ ] 3.7 Implement UpdateWeightEntryAsync method
- [ ] 3.8 Implement DeleteWeightEntryAsync method
- [ ] 3.9 Implement GetWeightEntriesInDateRangeAsync method for filtering
- [ ] 3.10 Add database error handling and transaction support
- [ ] 3.11 Add unit tests for DatabaseService operations

## 4. Settings Service

- [ ] 4.1 Create ISettingsService interface
- [ ] 4.2 Implement SettingsService using MAUI Preferences API
- [ ] 4.3 Implement PreferredUnit property (kg/lbs) with default "kg"
- [ ] 4.4 Implement Language property (de/en) with system language detection
- [ ] 4.5 Implement OnboardingCompleted property with default false
- [ ] 4.6 Add unit tests for SettingsService with mocked Preferences

## 5. Localization

- [ ] 5.1 Create Resources/Strings.resx with English strings
- [ ] 5.2 Create Resources/Strings.de.resx with German translations
- [ ] 5.3 Add localization for all UI labels, buttons, and messages
- [ ] 5.4 Add localization for error messages and validation text
- [ ] 5.5 Add localization for empty state messages
- [ ] 5.6 Add localization for export file headers (CSV)
- [ ] 5.7 Configure MauiProgram.cs to use IStringLocalizer
- [ ] 5.8 Implement locale-specific date formatting
- [ ] 5.9 Implement locale-specific number formatting (comma vs period)

## 6. Statistics Service

- [ ] 6.1 Create IStatisticsService interface
- [ ] 6.2 Implement StatisticsService with calculation methods
- [ ] 6.3 Implement GetCurrentWeight method (most recent measurement)
- [ ] 6.4 Implement GetStartingWeight method (earliest measurement)
- [ ] 6.5 Implement CalculateWeightChange method (absolute and percentage)
- [ ] 6.6 Implement CalculateWeightChangeForPeriod method (7/30/90 days)
- [ ] 6.7 Handle edge cases (no data, single measurement, missing period data)
- [ ] 6.8 Add unit tests for all statistics calculations with various scenarios

## 7. Export Service

- [ ] 7.1 Create IExportService interface
- [ ] 7.2 Implement ExportService with CSV and JSON export methods
- [ ] 7.3 Implement ExportToCsvAsync method with proper escaping and headers
- [ ] 7.4 Implement ExportToJsonAsync method with structured format
- [ ] 7.5 Implement ShareFileAsync method using MAUI Share API
- [ ] 7.6 Add localized CSV headers based on selected language
- [ ] 7.7 Handle special characters in notes (quotes, newlines, commas)
- [ ] 7.8 Add filename generation with date (weight-data-YYYY-MM-DD)
- [ ] 7.9 Add unit tests for CSV formatting and JSON structure
- [ ] 7.10 Add unit tests for edge cases (empty data, special characters)

## 8. Main ViewModel

- [ ] 8.1 Create MainViewModel with CommunityToolkit.Mvvm base class
- [ ] 8.2 Add ObservableCollection<WeightEntry> for measurements list
- [ ] 8.3 Implement LoadWeightEntriesCommand
- [ ] 8.4 Implement DeleteWeightEntryCommand with confirmation
- [ ] 8.5 Implement NavigateToAddWeightCommand
- [ ] 8.6 Implement NavigateToEditWeightCommand
- [ ] 8.7 Add CurrentWeight and StartingWeight properties
- [ ] 8.8 Add WeightChange properties (absolute and percentage)
- [ ] 8.9 Implement unit conversion based on preferred unit
- [ ] 8.10 Add error handling and user feedback
- [ ] 8.11 Add unit tests for MainViewModel commands and properties

## 9. Add/Edit Weight ViewModel

- [ ] 9.1 Create AddEditWeightViewModel
- [ ] 9.2 Add properties: WeightValue, SelectedDate, Notes, IsEditMode
- [ ] 9.3 Implement SaveWeightCommand with validation
- [ ] 9.4 Implement CancelCommand
- [ ] 9.5 Add validation: weight > 0, date not in future
- [ ] 9.6 Handle unit conversion when saving (lbs to kg if needed)
- [ ] 9.7 Add validation error messages
- [ ] 9.8 Add unit tests for save/cancel scenarios and validation

## 10. Statistics ViewModel

- [ ] 10.1 Create StatisticsViewModel
- [ ] 10.2 Add properties for current weight, starting weight, weight changes
- [ ] 10.3 Add SelectedPeriod property (7/30/90 days, All)
- [ ] 10.4 Implement LoadStatisticsCommand
- [ ] 10.5 Implement ChangePeriodCommand
- [ ] 10.6 Calculate and display weight change with trend indicators (↑↓)
- [ ] 10.7 Handle empty data state gracefully
- [ ] 10.8 Add unit tests for period calculations

## 11. Chart ViewModel

- [ ] 11.1 Create ChartViewModel
- [ ] 11.2 Add ObservableCollection for chart data points
- [ ] 11.3 Add SelectedTimeFilter property (1W, 1M, 3M, 6M, All)
- [ ] 11.4 Implement LoadChartDataCommand with date filtering
- [ ] 11.5 Implement ChangeTimeFilterCommand
- [ ] 11.6 Handle empty chart state
- [ ] 11.7 Configure chart data in preferred unit
- [ ] 11.8 Add unit tests for filtering logic

## 12. Main Page UI

- [ ] 12.1 Create MainPage.xaml with navigation tabs/shell
- [ ] 12.2 Add FloatingActionButton for adding new measurement
- [ ] 12.3 Create weight measurements list view
- [ ] 12.4 Add swipe-to-delete gesture for list items
- [ ] 12.5 Add tap gesture to navigate to edit screen
- [ ] 12.6 Display weight in preferred unit with proper formatting
- [ ] 12.7 Add empty state view with message
- [ ] 12.8 Apply localized strings to all UI elements
- [ ] 12.9 Implement dark mode support with AppThemeBinding

## 13. Add/Edit Weight Page UI

- [ ] 13.1 Create AddEditWeightPage.xaml
- [ ] 13.2 Add weight input field with numeric keyboard
- [ ] 13.3 Add date picker with default to today
- [ ] 13.4 Add notes text input (optional, multiline)
- [ ] 13.5 Add Save and Cancel buttons
- [ ] 13.6 Display validation errors inline
- [ ] 13.7 Show current unit (kg/lbs) label
- [ ] 13.8 Apply localization to all labels and placeholders
- [ ] 13.9 Implement dark mode support

## 14. Statistics Page UI

- [ ] 14.1 Create StatisticsPage.xaml
- [ ] 14.2 Display current weight card
- [ ] 14.3 Display starting weight card
- [ ] 14.4 Display weight change card with absolute and percentage
- [ ] 14.5 Add trend indicator icons (↑↓ arrows)
- [ ] 14.6 Add period filter buttons (7/30/90 days, All)
- [ ] 14.7 Apply localization to all text
- [ ] 14.8 Implement dark mode support
- [ ] 14.9 Handle empty state when no measurements exist

## 15. Chart Page UI

- [ ] 15.1 Create ChartPage.xaml
- [ ] 15.2 Add Syncfusion Line Chart control
- [ ] 15.3 Configure X-axis (date) and Y-axis (weight)
- [ ] 15.4 Add time filter buttons (1W, 1M, 3M, 6M, All)
- [ ] 15.5 Bind chart data to ViewModel
- [ ] 15.6 Configure chart theme (light/dark mode support)
- [ ] 15.7 Add empty state when no data
- [ ] 15.8 Apply localization to axis labels
- [ ] 15.9 Set appropriate axis scale based on data range

## 16. Settings Page UI

- [ ] 16.1 Create SettingsPage.xaml
- [ ] 16.2 Add language selector (German/English)
- [ ] 16.3 Add unit preference selector (kg/lbs)
- [ ] 16.4 Implement immediate UI update when settings change
- [ ] 16.5 Apply localization to all settings labels
- [ ] 16.6 Implement dark mode support

## 17. Export Functionality UI

- [ ] 17.1 Add Export menu option or button in MainPage
- [ ] 17.2 Create export format selection dialog (CSV/JSON)
- [ ] 17.3 Show loading indicator during export
- [ ] 17.4 Show success message after export
- [ ] 17.5 Handle and display export errors
- [ ] 17.6 Trigger platform share sheet with generated file

## 18. Onboarding Flow

- [ ] 18.1 Create OnboardingPage.xaml with CarouselView
- [ ] 18.2 Add welcome screen with app introduction
- [ ] 18.3 Add features screen explaining key capabilities
- [ ] 18.4 Add unit selection screen (kg or lbs)
- [ ] 18.5 Add Skip button to all onboarding screens
- [ ] 18.6 Save OnboardingCompleted flag after completion
- [ ] 18.7 Show onboarding only on first launch
- [ ] 18.8 Apply localization to onboarding content
- [ ] 18.9 Implement dark mode support

## 19. Navigation and Shell

- [ ] 19.1 Configure AppShell.xaml with tab navigation
- [ ] 19.2 Add tabs: Home (List), Chart, Statistics, Settings
- [ ] 19.3 Register routes for modal pages (Add/Edit Weight)
- [ ] 19.4 Implement navigation service if needed
- [ ] 19.5 Apply localized tab labels

## 20. Styling and Theming

- [ ] 20.1 Create Resources/Styles/Colors.xaml with color definitions
- [ ] 20.2 Define light mode color palette
- [ ] 20.3 Define dark mode color palette using AppThemeBinding
- [ ] 20.4 Create Resources/Styles/Styles.xaml with reusable styles
- [ ] 20.5 Apply consistent spacing and padding throughout app
- [ ] 20.6 Ensure all text is readable in both themes
- [ ] 20.7 Test theme switching on all pages

## 21. Platform-Specific Implementation

- [ ] 21.1 Configure iOS Info.plist with app permissions and settings
- [ ] 21.2 Configure Android AndroidManifest.xml
- [ ] 21.3 Set app icons for all platforms
- [ ] 21.4 Set splash screens for all platforms
- [ ] 21.5 Test SQLite database path on all platforms
- [ ] 21.6 Test file sharing on all platforms
- [ ] 21.7 Verify Preferences storage on all platforms

## 22. Unit Tests - Services

- [ ] 22.1 Write tests for DatabaseService CRUD operations
- [ ] 22.2 Write tests for DatabaseService date range filtering
- [ ] 22.3 Write tests for SettingsService preference storage
- [ ] 22.4 Write tests for StatisticsService calculations
- [ ] 22.5 Write tests for ExportService CSV generation
- [ ] 22.6 Write tests for ExportService JSON generation
- [ ] 22.7 Write tests for WeightConverter accuracy
- [ ] 22.8 Achieve 80%+ code coverage for Services

## 23. Unit Tests - ViewModels

- [ ] 23.1 Write tests for MainViewModel load and delete commands
- [ ] 23.2 Write tests for AddEditWeightViewModel validation logic
- [ ] 23.3 Write tests for AddEditWeightViewModel save/cancel scenarios
- [ ] 23.4 Write tests for StatisticsViewModel period calculations
- [ ] 23.5 Write tests for ChartViewModel filtering logic
- [ ] 23.6 Mock all service dependencies in ViewModel tests
- [ ] 23.7 Achieve 80%+ code coverage for ViewModels

## 24. Integration Testing

- [ ] 24.1 Test complete add-measurement flow (UI → ViewModel → Service → Database)
- [ ] 24.2 Test complete edit-measurement flow
- [ ] 24.3 Test complete delete-measurement flow
- [ ] 24.4 Test export flow end-to-end
- [ ] 24.5 Test language switching updates UI correctly
- [ ] 24.6 Test unit switching converts all displayed weights
- [ ] 24.7 Test onboarding flow completes and doesn't show again

## 25. Manual Testing - All Platforms

- [ ] 25.1 Test on iOS device/simulator (iPhone, iPad)
- [ ] 25.2 Test on Android device/emulator (phone, tablet)
- [ ] 25.3 Test on Windows desktop
- [ ] 25.4 Test on macOS desktop
- [ ] 25.5 Verify database persistence across app restarts on all platforms
- [ ] 25.6 Verify theme switching on all platforms
- [ ] 25.7 Verify localization displays correctly on all platforms
- [ ] 25.8 Test edge cases: empty states, large datasets (1000+ entries)
- [ ] 25.9 Test performance: app responsiveness with 365+ measurements

## 26. Bug Fixes and Polish

- [ ] 26.1 Fix any crashes or exceptions discovered during testing
- [ ] 26.2 Fix UI layout issues on different screen sizes
- [ ] 26.3 Improve error messages for better user experience
- [ ] 26.4 Add loading indicators where needed
- [ ] 26.5 Optimize performance bottlenecks if any
- [ ] 26.6 Ensure all validation messages are clear and helpful

## 27. Documentation

- [ ] 27.1 Add XML documentation comments to public APIs
- [ ] 27.2 Create README.md with project overview and setup instructions
- [ ] 27.3 Document database schema and migration strategy
- [ ] 27.4 Document localization process for adding new languages
- [ ] 27.5 Document build and deployment process for each platform

## 28. CI/CD Setup

- [ ] 28.1 Configure build pipeline for automated builds
- [ ] 28.2 Configure test execution in CI pipeline
- [ ] 28.3 Configure code coverage reporting
- [ ] 28.4 Set up platform-specific build steps (iOS, Android, Windows, macOS)
- [ ] 28.5 Configure artifact generation for releases

## 29. Final Verification

- [ ] 29.1 Verify all unit tests pass (80%+ coverage achieved)
- [ ] 29.2 Verify all integration tests pass
- [ ] 29.3 Verify app works offline on all platforms
- [ ] 29.4 Verify data privacy: no network calls, data stays local
- [ ] 29.5 Verify app uninstall removes all data
- [ ] 29.6 Run final manual test pass on all platforms
- [ ] 29.7 Review and close any remaining GitHub issues/TODOs
