namespace BodyMeasurement.Services;

public class ShellNavigationService : INavigationService
{
    public Task OpenAddMeasurementAsync() =>
        Shell.Current.GoToAsync("addweight");

    public Task OpenEditMeasurementAsync(int measurementId) =>
        Shell.Current.GoToAsync("editweight", new ShellNavigationQueryParameters
        {
            { "MeasurementId", measurementId }
        });

    public Task GoBackAsync() => Shell.Current.GoToAsync("..");

    public Task<bool> ShowConfirmationAsync(string title, string message, string accept, string cancel) =>
        Application.Current!.Windows[0].Page!.DisplayAlertAsync(title, message, accept, cancel);

    public Task ShowAlertAsync(string title, string message, string cancel) =>
        Application.Current!.Windows[0].Page!.DisplayAlertAsync(title, message, cancel);
}
