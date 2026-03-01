## 1. Color Palette (Brand Theme)

- [x] 1.1 Update `Colors.xaml`: change `Primary` to `#8CB43E`, `PrimaryDark` to `#6E9E2D`, `Secondary` to `#E8F5D0`, `Tertiary` to `#5A8020`
- [x] 1.2 Update `Colors.xaml`: change `Magenta` to `#8CB43E` (tab bar accent) and `MidnightBlue` to `#2D4A10` (Headline/SubHeadline text)
- [x] 1.3 Update Android `Platforms/Android/Resources/values/colors.xml` to match the new primary green

## 2. Settings — UserName and GoalWeightKg

- [x] 2.1 Add `string UserName { get; set; }` and `double? GoalWeightKg { get; set; }` to `ISettingsService`
- [x] 2.2 Implement `UserName` and `GoalWeightKg` in `SettingsService` using `Preferences.Get/Set` (use sentinel `-1.0` for null goal weight)
- [x] 2.3 Add `[ObservableProperty] string _userName` and `[ObservableProperty] string _goalWeightKgText` to `SettingsViewModel`; load from settings in constructor
- [x] 2.4 Add `partial void OnUserNameChanged` and `partial void OnGoalWeightKgTextChanged` in `SettingsViewModel` to save parsed values to `_settingsService`
- [x] 2.5 Add a "Profile" section to `SettingsPage.xaml` (before the Language section) with an `Entry` for Name and an `Entry` with `Keyboard=Numeric` for Goal Weight, bound to the new ViewModel properties

## 3. Localization Strings

- [x] 3.1 Add new keys to `Strings.resx` (EN): `ProfileSection`, `UserNameLabel`, `GoalWeightLabel`, `MotivationalKeepGoing`, `WeightLostFormat`, `StartLabel`, `GoalLabel`, `VisceralFatLabel`, `WaterInBodyLabel`, `MuscleLabel`, `BmiLabel`
- [x] 3.2 Add matching German translations for all new keys in `Strings.de.resx`

## 4. MainViewModel — New Computed Properties

- [x] 4.1 Add `[ObservableProperty]` for: `UserName`, `GoalWeightKg` (double?), `ProgressValue` (double), `WeightLostDisplay` (string), `MotivationalMessage` (string), `StartWeightDisplay` (string), `GoalWeightDisplay` (string)
- [x] 4.2 Add placeholder `[ObservableProperty]` strings: `VisceralFatDisplay`, `WaterDisplay`, `MuscleDisplay`, `BmiDisplay` (all default to `"--"`)
- [x] 4.3 Update `LoadStatisticsAsync()` in `MainViewModel` to populate `UserName` and `GoalWeightKg` from `_settingsService`
- [x] 4.4 Implement progress calculation in `LoadStatisticsAsync()`: `ProgressValue = clamp((start - current) / (start - goal), 0, 1)` with null-guards
- [x] 4.5 Set `WeightLostDisplay`, `MotivationalMessage`, `StartWeightDisplay`, `GoalWeightDisplay` from computed values and RESX strings

## 5. MainPage XAML Redesign

- [x] 5.1 Set `Shell.NavBarIsVisible="False"` on `MainPage` ContentPage element
- [x] 5.2 Add custom header `Border` at top of page: left side has logo glyph (MaterialIcons ⚖) + "LemoWeight" Label; right side has person icon `ImageButton` and gear icon `ImageButton`, both navigating to Settings tab
- [x] 5.3 Add green profile banner section: `Border` with `BackgroundColor="#8CB43E"`, circular `Border` containing the app logo `Image`, and a `Label` bound to `UserName` (with fallback placeholder)
- [x] 5.4 Add motivational text section: two `Label` elements bound to `MotivationalMessage` and `MotivationalSubtitle` (from `Strings.MotivationalKeepGoing`)
- [x] 5.5 Add progress bar section: `Grid` with "Start"/"Ziel" labels, `AbsoluteLayout` containing `BoxView` with `LinearGradientBrush` (red→yellow→green), a vertical marker `BoxView` positioned via `AbsoluteLayout.LayoutFlags="XProportional"` at `ProgressValue`, and start/goal weight labels below
- [x] 5.6 Add measurement cards `VerticalStackLayout` inside a `ScrollView`: 5 `Border` elements each with a `Grid` containing a label (bound to RESX string), a value label (bound to display property), and a green chevron `Label` (MaterialIcons &#xE5CF;) on the right
- [x] 5.7 Keep the FAB button; change its `BackgroundColor` to `#8CB43E`
- [x] 5.8 Wire the person icon and gear icon `Command` bindings to a `NavigateToSettingsCommand` in `MainViewModel`

## 6. MainViewModel — Settings Navigation

- [x] 6.1 Add `[RelayCommand] NavigateToSettingsAsync()` in `MainViewModel` that calls `await Shell.Current.GoToAsync("//settings")` (or equivalent)

## 7. Verification

- [x] 7.1 Build the project: `dotnet build BodyMeasurement/BodyMeasurement.csproj` with no errors
- [x] 7.2 Run on iOS Simulator: verify green theme on all tabs, profile banner, progress bar, and measurement cards
- [x] 7.3 Run on Android Emulator: verify the same visual elements and Android `colors.xml` update
- [x] 7.4 Set a name and goal weight in Settings; verify home screen reflects them after navigating back
- [x] 7.5 Verify both EN and DE languages show correct strings on the home screen and Settings Profile section
