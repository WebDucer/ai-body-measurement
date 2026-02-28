## 1. Color Palette (Brand Theme)

- [ ] 1.1 Update `Colors.xaml`: change `Primary` to `#8CB43E`, `PrimaryDark` to `#6E9E2D`, `Secondary` to `#E8F5D0`, `Tertiary` to `#5A8020`
- [ ] 1.2 Update `Colors.xaml`: change `Magenta` to `#8CB43E` (tab bar accent) and `MidnightBlue` to `#2D4A10` (Headline/SubHeadline text)
- [ ] 1.3 Update Android `Platforms/Android/Resources/values/colors.xml` to match the new primary green

## 2. Settings — UserName and GoalWeightKg

- [ ] 2.1 Add `string UserName { get; set; }` and `double? GoalWeightKg { get; set; }` to `ISettingsService`
- [ ] 2.2 Implement `UserName` and `GoalWeightKg` in `SettingsService` using `Preferences.Get/Set` (use sentinel `-1.0` for null goal weight)
- [ ] 2.3 Add `[ObservableProperty] string _userName` and `[ObservableProperty] string _goalWeightKgText` to `SettingsViewModel`; load from settings in constructor
- [ ] 2.4 Add `partial void OnUserNameChanged` and `partial void OnGoalWeightKgTextChanged` in `SettingsViewModel` to save parsed values to `_settingsService`
- [ ] 2.5 Add a "Profile" section to `SettingsPage.xaml` (before the Language section) with an `Entry` for Name and an `Entry` with `Keyboard=Numeric` for Goal Weight, bound to the new ViewModel properties

## 3. Localization Strings

- [ ] 3.1 Add new keys to `Strings.resx` (EN): `ProfileSection`, `UserNameLabel`, `GoalWeightLabel`, `MotivationalKeepGoing`, `WeightLostFormat`, `StartLabel`, `GoalLabel`, `VisceralFatLabel`, `WaterInBodyLabel`, `MuscleLabel`, `BmiLabel`
- [ ] 3.2 Add matching German translations for all new keys in `Strings.de.resx`

## 4. MainViewModel — New Computed Properties

- [ ] 4.1 Add `[ObservableProperty]` for: `UserName`, `GoalWeightKg` (double?), `ProgressValue` (double), `WeightLostDisplay` (string), `MotivationalMessage` (string), `StartWeightDisplay` (string), `GoalWeightDisplay` (string)
- [ ] 4.2 Add placeholder `[ObservableProperty]` strings: `VisceralFatDisplay`, `WaterDisplay`, `MuscleDisplay`, `BmiDisplay` (all default to `"--"`)
- [ ] 4.3 Update `LoadStatisticsAsync()` in `MainViewModel` to populate `UserName` and `GoalWeightKg` from `_settingsService`
- [ ] 4.4 Implement progress calculation in `LoadStatisticsAsync()`: `ProgressValue = clamp((start - current) / (start - goal), 0, 1)` with null-guards
- [ ] 4.5 Set `WeightLostDisplay`, `MotivationalMessage`, `StartWeightDisplay`, `GoalWeightDisplay` from computed values and RESX strings

## 5. MainPage XAML Redesign

- [ ] 5.1 Set `Shell.NavBarIsVisible="False"` on `MainPage` ContentPage element
- [ ] 5.2 Add custom header `Border` at top of page: left side has logo glyph (MaterialIcons ⚖) + "LemoWeight" Label; right side has person icon `ImageButton` and gear icon `ImageButton`, both navigating to Settings tab
- [ ] 5.3 Add green profile banner section: `Border` with `BackgroundColor="#8CB43E"`, circular `Border` containing the app logo `Image`, and a `Label` bound to `UserName` (with fallback placeholder)
- [ ] 5.4 Add motivational text section: two `Label` elements bound to `MotivationalMessage` and `MotivationalSubtitle` (from `Strings.MotivationalKeepGoing`)
- [ ] 5.5 Add progress bar section: `Grid` with "Start"/"Ziel" labels, `AbsoluteLayout` containing `BoxView` with `LinearGradientBrush` (red→yellow→green), a vertical marker `BoxView` positioned via `AbsoluteLayout.LayoutFlags="XProportional"` at `ProgressValue`, and start/goal weight labels below
- [ ] 5.6 Add measurement cards `VerticalStackLayout` inside a `ScrollView`: 5 `Border` elements each with a `Grid` containing a label (bound to RESX string), a value label (bound to display property), and a green chevron `Label` (MaterialIcons &#xE5CF;) on the right
- [ ] 5.7 Keep the FAB button; change its `BackgroundColor` to `#8CB43E`
- [ ] 5.8 Wire the person icon and gear icon `Command` bindings to a `NavigateToSettingsCommand` in `MainViewModel`

## 6. MainViewModel — Settings Navigation

- [ ] 6.1 Add `[RelayCommand] NavigateToSettingsAsync()` in `MainViewModel` that calls `await Shell.Current.GoToAsync("//settings")` (or equivalent)

## 7. Verification

- [ ] 7.1 Build the project: `dotnet build BodyMeasurement/BodyMeasurement.csproj` with no errors
- [ ] 7.2 Run on iOS Simulator: verify green theme on all tabs, profile banner, progress bar, and measurement cards
- [ ] 7.3 Run on Android Emulator: verify the same visual elements and Android `colors.xml` update
- [ ] 7.4 Set a name and goal weight in Settings; verify home screen reflects them after navigating back
- [ ] 7.5 Verify both EN and DE languages show correct strings on the home screen and Settings Profile section
