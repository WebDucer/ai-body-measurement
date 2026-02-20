namespace BodyMeasurement.Services;

public interface INavigationService
{
    Task OpenAddMeasurementAsync();
    Task OpenEditMeasurementAsync(int measurementId);
    Task GoBackAsync();
    Task<bool> ShowConfirmationAsync(string title, string message, string accept, string cancel);
    Task ShowAlertAsync(string title, string message, string cancel);
}
