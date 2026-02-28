## Why

The app currently uses a generic purple MAUI template theme that doesn't reflect the LemoWeight brand identity. A provided design mockup establishes a cohesive green brand (olive-lime #8CB43E), a richer home screen with user profile, goal tracking, and body composition overview — all of which are needed to make the app feel complete and branded before any public launch.

## What Changes

- Replace the entire color palette (purple → LemoWeight green #8CB43E) across all pages and components
- Redesign `MainPage` to match the mockup: custom header bar, green profile banner, gradient progress bar (start → goal weight), and 5 body measurement cards
- Add `UserName` and `GoalWeightKg` to `ISettingsService` / `SettingsService` (stored via MAUI Preferences)
- Add a "Profile" section to `SettingsPage` for entering name and goal weight
- Update `MainViewModel` to compute progress value and motivational message
- Add localization strings for all new UI text in both English and German RESX files
- Extra measurement cards (Viszeralfett, Wasser in Körper, Muskel, BMI) show `--` as placeholders — real data tracking is a future feature

## Capabilities

### New Capabilities

- `brand-theme`: App-wide green color scheme replacing the default purple; affects Colors.xaml, Styles.xaml, and all themed elements
- `home-screen-redesign`: New MainPage layout with custom header, profile banner, gradient progress bar, and body measurement card list
- `goal-tracking`: User-configurable goal weight stored in settings; progress toward goal displayed on the home screen
- `user-profile`: User name stored in settings and shown in the profile banner on the home screen

### Modified Capabilities

- `localization`: New string keys added for profile section, goal weight, measurement card labels, and motivational messages (EN + DE)

## Impact

- `BodyMeasurement/Resources/Styles/Colors.xaml` — complete palette swap
- `BodyMeasurement/Resources/Styles/Styles.xaml` — Shell tab bar color references cascade automatically
- `BodyMeasurement/Services/ISettingsService.cs` + `SettingsService.cs` — two new properties
- `BodyMeasurement/Views/MainPage.xaml` — full XAML rewrite
- `BodyMeasurement/ViewModels/MainViewModel.cs` — new computed properties
- `BodyMeasurement/Views/SettingsPage.xaml` — new Profile section
- `BodyMeasurement/ViewModels/SettingsViewModel.cs` — two new observable properties
- `BodyMeasurement/Resources/Strings/Strings.resx` + `Strings.de.resx` — new string keys
- No breaking changes to the database schema or existing APIs
