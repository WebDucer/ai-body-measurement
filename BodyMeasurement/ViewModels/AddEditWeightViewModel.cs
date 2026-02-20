using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using BodyMeasurement.Models;
using BodyMeasurement.Resources.Strings;
using BodyMeasurement.Services;

namespace BodyMeasurement.ViewModels;

/// <summary>
/// ViewModel for adding or editing weight measurements
/// </summary>
public partial class AddEditWeightViewModel : ObservableObject, IQueryAttributable
{
    private readonly IDatabaseService _databaseService;
    private readonly ISettingsService _settingsService;
    private readonly INavigationService _navigationService;
    private readonly ILogger<AddEditWeightViewModel> _logger;

    [ObservableProperty]
    private double _weight;

    [ObservableProperty]
    private DateTime _date = DateTime.Today;

    [ObservableProperty]
    private string? _notes;

    [ObservableProperty]
    private bool _isEditMode;

    [ObservableProperty]
    private string _title = "Add Weight";

    [ObservableProperty]
    private string _preferredUnit = "kg";

    [ObservableProperty]
    private string? _weightError;

    [ObservableProperty]
    private string? _dateError;

    private int? _editId;

    public AddEditWeightViewModel(
        IDatabaseService databaseService,
        ISettingsService settingsService,
        INavigationService navigationService,
        ILogger<AddEditWeightViewModel> logger)
    {
        _databaseService = databaseService;
        _settingsService = settingsService;
        _navigationService = navigationService;
        _logger = logger;

        _preferredUnit = _settingsService.PreferredUnit;
    }

    /// <summary>
    /// Applies query attributes passed via Shell navigation parameters
    /// </summary>
    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("MeasurementId", out var value) && value is int id)
        {
            _editId = id;
            IsEditMode = true;
            Title = Strings.EditWeightTitle;
            _ = LoadWeightEntryAsync(id);
        }
        else
        {
            _editId = null;
            IsEditMode = false;
            Title = Strings.AddWeightTitle;
            Weight = 0;
            Date = DateTime.Today;
            Notes = null;
        }
    }

    /// <summary>
    /// Loads an existing weight entry for editing
    /// </summary>
    private async Task LoadWeightEntryAsync(int id)
    {
        try
        {
            var entry = await _databaseService.FindMeasurementAsync(id);
            if (entry != null)
            {
                // Convert from kg if user prefers lbs
                if (PreferredUnit == "lbs")
                {
                    Weight = WeightConverter.KgToLbs(entry.WeightKg);
                }
                else
                {
                    Weight = entry.WeightKg;
                }

                Date = entry.Date;
                Notes = entry.Notes;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading weight entry");
        }
    }

    /// <summary>
    /// Validates the input
    /// </summary>
    private bool ValidateInput()
    {
        bool isValid = true;

        // Validate weight
        if (Weight <= 0)
        {
            WeightError = Strings.ValidationWeightPositive;
            isValid = false;
        }
        else
        {
            WeightError = null;
        }

        // Validate date
        if (Date > DateTime.Today)
        {
            DateError = Strings.ValidationDateNotFuture;
            isValid = false;
        }
        else
        {
            DateError = null;
        }

        return isValid;
    }

    /// <summary>
    /// Saves the weight entry
    /// </summary>
    [RelayCommand]
    private async Task SaveAsync()
    {
        if (!ValidateInput())
        {
            return;
        }

        try
        {
            // Convert to kg if user entered in lbs
            double weightKg = PreferredUnit == "lbs"
                ? WeightConverter.LbsToKg(Weight)
                : Weight;

            if (IsEditMode && _editId.HasValue)
            {
                // Update existing entry
                var entry = await _databaseService.FindMeasurementAsync(_editId.Value);
                if (entry != null)
                {
                    entry.WeightKg = weightKg;
                    entry.Date = Date;
                    entry.Notes = Notes;
                    await _databaseService.UpdateMeasurementAsync(entry);
                }
            }
            else
            {
                // Create new entry
                var entry = new WeightEntry
                {
                    WeightKg = weightKg,
                    Date = Date,
                    Notes = Notes,
                    CreatedAt = DateTime.UtcNow
                };
                await _databaseService.RecordMeasurementAsync(entry);
            }

            // Navigate back
            await _navigationService.GoBackAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving weight entry");
            await _navigationService.ShowAlertAsync(
                Strings.ErrorTitle,
                Strings.ErrorSaveMeasurement,
                Strings.Ok);
        }
    }

    /// <summary>
    /// Cancels and navigates back
    /// </summary>
    [RelayCommand]
    private async Task CancelAsync()
    {
        await _navigationService.GoBackAsync();
    }
}
