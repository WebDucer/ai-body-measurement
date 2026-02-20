## MODIFIED Requirements

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

## ADDED Requirements

### Requirement: Microsoft.Extensions.Localization package is not used
The system SHALL NOT reference the `Microsoft.Extensions.Localization` NuGet package. Localization is implemented entirely via `.resx` resource files and the custom `ILocalizationService` backed by `System.Resources.ResourceManager`.

#### Scenario: Package absent from project file
- **WHEN** inspecting `BodyMeasurement.csproj`
- **THEN** no `<PackageReference>` for `Microsoft.Extensions.Localization` is present

#### Scenario: Localization works without the package
- **WHEN** the app runs without `Microsoft.Extensions.Localization`
- **THEN** all UI strings resolve correctly in both English and German
