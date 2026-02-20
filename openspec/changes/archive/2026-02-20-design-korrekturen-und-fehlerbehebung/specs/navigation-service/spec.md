## ADDED Requirements

### Requirement: Navigation and dialogs are abstracted behind INavigationService
The system SHALL provide an `INavigationService` interface that encapsulates all Shell navigation and modal dialog operations, so that ViewModels have no direct dependency on `Shell.Current` or `Application.Current.MainPage`.

#### Scenario: ViewModel navigates without Shell dependency
- **WHEN** a ViewModel needs to navigate to another page
- **THEN** it calls a method on `INavigationService` without referencing `Shell.Current` directly

#### Scenario: ViewModel shows confirmation dialog without Application dependency
- **WHEN** a ViewModel needs to confirm a destructive action
- **THEN** it calls `INavigationService.ShowConfirmationAsync` without referencing `Application.Current.MainPage` directly

#### Scenario: ViewModel shows alert without Application dependency
- **WHEN** a ViewModel needs to display an error or success message
- **THEN** it calls `INavigationService.ShowAlertAsync` without referencing `Application.Current.MainPage` directly

### Requirement: INavigationService provides domain-specific navigation methods
The system SHALL expose named, domain-specific navigation methods rather than generic route strings, so call sites are readable and type-safe.

#### Scenario: Navigate to add measurement screen
- **WHEN** `INavigationService.OpenAddMeasurementAsync()` is called
- **THEN** the system navigates to the add measurement page

#### Scenario: Navigate to edit measurement screen with typed parameter
- **WHEN** `INavigationService.OpenEditMeasurementAsync(int measurementId)` is called
- **THEN** the system navigates to the edit page and the target ViewModel receives the measurement ID as a typed `int` (not a string)

#### Scenario: Navigate back
- **WHEN** `INavigationService.GoBackAsync()` is called
- **THEN** the system pops the current page from the navigation stack

### Requirement: Navigation parameters are passed type-safely
The system SHALL use `ShellNavigationQueryParameters` for object-based navigation data and `IQueryAttributable` on receiving ViewModels, so no string-to-primitive parsing is required and navigation is trim-safe.

#### Scenario: Edit measurement ID received as int
- **WHEN** navigating to the edit page with a measurement ID
- **THEN** `AddEditWeightViewModel.ApplyQueryAttributes` receives the value as an `int`, not a string, and no `int.TryParse` is needed

#### Scenario: Navigation parameters cleared after use
- **WHEN** navigation with `ShellNavigationQueryParameters` completes
- **THEN** the parameter dictionary is automatically cleared and not retained in memory for subsequent navigations

#### Scenario: No QueryProperty attributes on ViewModels
- **WHEN** inspecting ViewModel classes
- **THEN** no `[QueryProperty]` attributes are present; all navigation data is received via `IQueryAttributable`

### Requirement: ShellNavigationService is the MAUI implementation of INavigationService
The system SHALL provide a `ShellNavigationService` class that implements `INavigationService` using `Shell.Current` and `Application.Current.MainPage`, registered as a singleton in the DI container.

#### Scenario: Service registered in DI container
- **WHEN** the app starts
- **THEN** `INavigationService` resolves to `ShellNavigationService` via dependency injection

#### Scenario: ViewModels receive INavigationService via constructor injection
- **WHEN** a ViewModel is resolved from the DI container
- **THEN** its `INavigationService` dependency is automatically injected

### Requirement: INavigationService is mockable in unit tests
The system SHALL allow unit tests to substitute `INavigationService` with a mock, so ViewModel navigation and dialog logic can be verified without a running MAUI host.

#### Scenario: Mock navigation in ViewModel test
- **WHEN** a ViewModel test sets up a mock `INavigationService`
- **THEN** the test can verify that the correct navigation method was called with the correct arguments

#### Scenario: Mock dialog response in ViewModel test
- **WHEN** a ViewModel test configures the mock to return `true` for `ShowConfirmationAsync`
- **THEN** the ViewModel proceeds as if the user confirmed the action
