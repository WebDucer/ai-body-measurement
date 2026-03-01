## Context

The app uses .NET MAUI with MVVM (CommunityToolkit.Mvvm), SQLite storage, Shell navigation (4 tabs: Home, Chart, Statistics, Settings), and two RESX localization files (EN/DE). The current theme is the default MAUI purple (`#512BD4`) defined in `Colors.xaml`. The MainPage is a simple stats header + scrollable list of weight entries.

The mockup defines a branded "LemoWeight" identity with olive-lime green (`#8CB43E`) as the primary color, and a richer home screen showing a profile banner, a gradient progress bar toward a goal weight, and body measurement overview cards. Settings are stored via MAUI `Preferences` API in `SettingsService`.

## Goals / Non-Goals

**Goals:**
- Replace app-wide color palette from purple to LemoWeight green with minimal blast radius
- Implement the full MainPage redesign matching the mockup layout
- Add `UserName` and `GoalWeightKg` to settings (persistence + UI)
- Show meaningful progress toward goal on MainPage; gracefully degrade when goal not set
- Add all new strings in both EN and DE locales

**Non-Goals:**
- Implementing real tracking for Viszeralfett, Wasser, Muskel, BMI (future feature)
- Profile photo upload or user avatar image (placeholder logo used)
- Dark mode design variants for the new MainPage sections (existing AppThemeBinding patterns carry forward)
- Changing the Shell navigation structure or tab layout

## Decisions

### D1: Single `Colors.xaml` palette swap, no duplication

**Decision:** Update existing color keys (`Primary`, `PrimaryDark`, `Secondary`, `Tertiary`, `Magenta`, `MidnightBlue`) in-place rather than adding new named keys alongside old ones.

**Rationale:** `Styles.xaml` and all pages already reference these keys via `{StaticResource Primary}` and `{AppThemeBinding}`. Renaming in-place means no change needed in any consumer. The tab bar accent currently references `Magenta` — changing `Magenta` to the same green value keeps the cascading behavior.

**Alternative considered:** Add new `LemoGreen` key and update all references. Rejected — high noise, many files to touch.

---

### D2: Custom header inside MainPage, not Shell NavBar

**Decision:** Set `Shell.NavBarIsVisible="False"` on MainPage and build a custom header `Border` at the top of the page XAML with the logo, title, person icon, and gear icon.

**Rationale:** The mockup header layout (left: logo+text, right: two icons) cannot be achieved with MAUI Shell's standard `ToolbarItems` which only supports right-aligned items. A custom header gives full control over layout and matches the design exactly.

**Alternative considered:** Use Shell `TitleView` for the header. Works for custom content but requires injecting the shell infrastructure — more complex and still restricted in layout on some platforms.

---

### D3: Gradient progress bar via `LinearGradientBrush` on `BoxView` + `AbsoluteLayout` marker

**Decision:** Render the progress bar as a `BoxView` with a `LinearGradientBrush` (red → yellow → green, horizontal). Use an `AbsoluteLayout` to overlay a thin vertical `BoxView` marker positioned proportionally via `AbsoluteLayout.LayoutFlags="XProportional"`. A small `Image` or `Label` (logo glyph) floats above the marker.

**Rationale:** MAUI's native `ProgressBar` control only supports a single fill color — no gradient support. `BoxView` with `LinearGradientBrush` is the idiomatic MAUI approach for static gradient visuals.

**Alternative considered:** `SkiaSharp` canvas for a fully custom gradient bar. Rejected — adds a new dependency; the BoxView approach is sufficient.

**Progress calculation:**
```
progressValue = clamp((startWeight - currentWeight) / (startWeight - goalWeight), 0, 1)
```
- If `goalWeight` is null or equals `startWeight`: `progressValue = 0`
- If user has exceeded goal: clamped to 1.0

---

### D4: Measurement cards as static `VerticalStackLayout`, not `CollectionView`

**Decision:** Render the 5 measurement cards as 5 explicit `Border` elements in a `VerticalStackLayout` inside a `ScrollView`. No `CollectionView` or `DataTemplate`.

**Rationale:** The card set is fixed (Weight, Viszeralfett, Wasser, Muskel, BMI). Using `CollectionView` with a dynamic list adds unnecessary complexity for a static 5-item layout. Explicit elements also make it trivial to wire up individual bindings or add card-specific behavior later.

**Alternative considered:** Dynamic list from a `List<MeasurementCard>` model. Rejected — over-engineering for a known-static set.

---

### D5: `GoalWeightKg` stored as `double` in Preferences using sentinel value

**Decision:** Store goal weight as `double` using `Preferences.Get("GoalWeightKg", -1.0)`. Return `null` from the property when the stored value is `-1.0`.

**Rationale:** MAUI `Preferences` doesn't natively support nullable types. Sentinel `-1.0` is unambiguous (negative weight is impossible) and simpler than checking `ContainsKey` on every read.

---

### D6: `GoalWeightKg` in ViewModel bound as `string` for Entry

**Decision:** Expose `GoalWeightKgText` (string) in `SettingsViewModel` for the `Entry` control, parse to `double?` in the property-changed handler, and save to settings only if valid.

**Rationale:** MAUI `Entry` with `Keyboard=Numeric` returns a string; direct `double?` binding is unreliable on all platforms. String property + explicit parse is the idiomatic MAUI MVVM pattern.

## Risks / Trade-offs

- **Gradient bar accuracy**: The `AbsoluteLayout` proportional marker is a visual approximation. On very small or very large screens it may appear slightly off. → Mitigation: Clamp `ProgressValue` and test on multiple screen sizes.

- **Shell NavBar hidden globally**: Setting `Shell.NavBarIsVisible="False"` on MainPage removes the default back-button handling for any modally pushed pages over MainPage. → Mitigation: MainPage has no modal children; AddEdit pages are pushed separately and retain their own nav bars.

- **Color cascade**: Changing `Magenta` (which was `#D600AA`) to green affects any hardcoded reference to `{StaticResource Magenta}` by name. → Mitigation: Audit `Styles.xaml` and all XAML pages for `Magenta` references before changing. (Only `Styles.xaml` Shell styles use it.)

- **Placeholder cards**: Showing `--` for Viszeralfett/Wasser/Muskel/BMI may confuse users who expect to enter values. → Mitigation: This is acceptable for now; future feature spec will address data entry.

## Migration Plan

1. Update `Colors.xaml` first — confirms cascading works before touching page layouts
2. Update `ISettingsService` + `SettingsService` — new properties with defaults
3. Update `SettingsPage` + `SettingsViewModel` — profile section wired and testable
4. Update `MainViewModel` — new computed properties
5. Rewrite `MainPage.xaml` — full layout swap
6. Add RESX strings last (or alongside step 5 to avoid broken bindings)
7. Build and verify on iOS Simulator + Android Emulator

No database migration required. No rollback complexity — all changes are UI/preferences.

## Open Questions

- Should the person icon navigate somewhere specific (e.g., scroll to profile section in Settings), or simply open the Settings tab?
  - **Assumed decision**: Navigate to Settings tab (same as gear icon) — acceptable for the current scope.
- Should the motivational message vary (multiple phrases) or be a single fixed string?
  - **Assumed decision**: Single string from RESX for now; variety is a future enhancement.
