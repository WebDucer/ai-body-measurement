# Specification: Localization

## ADDED Requirements

### Requirement: App supports German and English languages
The system SHALL provide complete user interface translations for German (de) and English (en) languages.

#### Scenario: App available in German
- **WHEN** user sets language to German
- **THEN** all UI text displays in German

#### Scenario: App available in English
- **WHEN** user sets language to English
- **THEN** all UI text displays in English

### Requirement: User can change app language
The system SHALL allow users to change the app language through settings.

#### Scenario: Change language from German to English
- **WHEN** user changes language setting from German to English
- **THEN** app immediately updates all UI text to English

#### Scenario: Change language from English to German
- **WHEN** user changes language setting from English to German
- **THEN** app immediately updates all UI text to German

#### Scenario: Language preference persists
- **WHEN** user changes language, closes app, and reopens
- **THEN** app continues to display in the selected language

### Requirement: App detects system language on first launch
The system SHALL automatically set the app language based on device system language on first launch.

#### Scenario: First launch on German device
- **WHEN** user opens app for first time on device with German system language
- **THEN** app defaults to German language

#### Scenario: First launch on English device
- **WHEN** user opens app for first time on device with English system language
- **THEN** app defaults to English language

#### Scenario: First launch on unsupported language device
- **WHEN** user opens app for first time on device with unsupported language (e.g., French)
- **THEN** app defaults to English language

### Requirement: All user-facing text is localized
The system SHALL localize all user-visible text including labels, buttons, messages, and placeholders. This requirement is extended to include all previously hard-coded strings in ViewModels and Views that were not yet covered by localization.

#### Scenario: Navigation labels localized
- **WHEN** viewing navigation menu
- **THEN** all menu items display in selected language

#### Scenario: Button text localized
- **WHEN** viewing any screen with buttons
- **THEN** all button labels display in selected language

#### Scenario: Form labels and placeholders localized
- **WHEN** viewing input forms
- **THEN** field labels and placeholder text display in selected language

#### Scenario: Error messages localized
- **WHEN** validation error occurs
- **THEN** error message displays in selected language

#### Scenario: Success messages localized
- **WHEN** operation succeeds
- **THEN** success message displays in selected language

#### Scenario: Empty state messages localized
- **WHEN** viewing empty lists or charts
- **THEN** empty state messages display in selected language

#### Scenario: Confirmation dialog text localized
- **WHEN** a confirmation dialog is shown (e.g., delete, language change)
- **THEN** title and message text display in the selected language

#### Scenario: Alert dialog text localized
- **WHEN** an alert dialog is shown (e.g., export success, save error)
- **THEN** title and message text display in the selected language

#### Scenario: Add and edit screen titles localized
- **WHEN** user opens the add measurement screen
- **THEN** page title displays in the selected language

#### Scenario: Validation error messages localized
- **WHEN** user submits invalid weight or future date
- **THEN** validation messages display in the selected language

### Requirement: Date and number formatting respects locale
The system SHALL format dates and numbers according to the selected language's locale conventions.

#### Scenario: Date format in German
- **WHEN** app language is German
- **THEN** dates display in German format (e.g., "15.02.2026" or "15. Februar 2026")

#### Scenario: Date format in English
- **WHEN** app language is English
- **THEN** dates display in English format (e.g., "02/15/2026" or "February 15, 2026")

#### Scenario: Number formatting in German
- **WHEN** app language is German
- **THEN** numbers use comma as decimal separator (e.g., "75,5 kg")

#### Scenario: Number formatting in English
- **WHEN** app language is English
- **THEN** numbers use period as decimal separator (e.g., "75.5 kg")

### Requirement: Localization does not affect data storage
The system SHALL store data in a language-independent format regardless of UI language.

#### Scenario: Data stored in neutral format
- **WHEN** user records measurement in German UI
- **THEN** database stores weight as numeric value without locale-specific formatting

#### Scenario: Data readable across languages
- **WHEN** user records data in German, switches to English
- **THEN** all measurements display correctly in English with English formatting

### Requirement: Unit labels are localized
The system SHALL display unit labels (kg, lbs) appropriately for the selected language.

#### Scenario: Weight unit display in German
- **WHEN** app language is German
- **THEN** weight displays as "75,5 kg" with German formatting

#### Scenario: Weight unit display in English
- **WHEN** app language is English
- **THEN** weight displays as "75.5 kg" or "166.0 lbs" with English formatting

### Requirement: Onboarding content is localized
The system SHALL display onboarding screens and instructions in the selected language.

#### Scenario: Onboarding in German
- **WHEN** user sees onboarding for first time with German language
- **THEN** all onboarding screens display German text

#### Scenario: Onboarding in English
- **WHEN** user sees onboarding for first time with English language
- **THEN** all onboarding screens display English text

### Requirement: Export file content respects localization
The system SHALL generate export files with headers and metadata in the selected language.

#### Scenario: CSV headers in German
- **WHEN** exporting to CSV with German language selected
- **THEN** CSV headers are "Datum", "Gewicht (kg)", "Gewicht (lbs)", "Notizen"

#### Scenario: CSV headers in English
- **WHEN** exporting to CSV with English language selected
- **THEN** CSV headers are "Date", "Weight (kg)", "Weight (lbs)", "Notes"

#### Scenario: JSON metadata in selected language
- **WHEN** exporting to JSON
- **THEN** field names remain in English (standard), but any metadata text respects selected language

### Requirement: Localization handles text expansion
The system SHALL accommodate text length variations between German and English without UI breaking.

#### Scenario: German text longer than English
- **WHEN** German translation is longer than English equivalent
- **THEN** UI layouts adapt without text truncation or overflow

#### Scenario: Labels with sufficient space
- **WHEN** displaying localized text on buttons or labels
- **THEN** layout provides sufficient space for both languages

### Requirement: Missing translations fallback gracefully
The system SHALL handle missing translations without crashing or displaying keys.

#### Scenario: Missing translation key
- **WHEN** a translation key is missing for selected language
- **THEN** system displays English text as fallback

#### Scenario: Partial translation coverage
- **WHEN** some strings lack translations
- **THEN** app displays available translations and falls back to English for missing ones

### Requirement: Microsoft.Extensions.Localization package is not used
The system SHALL NOT reference the `Microsoft.Extensions.Localization` NuGet package. Localization is implemented entirely via `.resx` resource files. The MSBuild `GenerateResource` task generates a strongly-typed `Strings` class (backed by `System.Resources.ResourceManager`) at build time. ViewModels access localized strings directly via `Strings.<PropertyName>` (compile-time type-safe). `ILocalizationService` is responsible for language switching (`SetLanguage`) and culture state management only.

#### Scenario: Package absent from project file
- **WHEN** inspecting `BodyMeasurement.csproj`
- **THEN** no `<PackageReference>` for `Microsoft.Extensions.Localization` is present

#### Scenario: Localization works without the package
- **WHEN** the app runs without `Microsoft.Extensions.Localization`
- **THEN** all UI strings resolve correctly in both English and German
