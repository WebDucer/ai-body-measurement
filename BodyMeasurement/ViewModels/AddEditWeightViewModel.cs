using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BodyMeasurement.Models;
using BodyMeasurement.Services;

namespace BodyMeasurement.ViewModels;

/// <summary>
/// ViewModel for adding or editing weight measurements
/// </summary>
[QueryProperty(nameof(EntryId), "id")]
public partial class AddEditWeightViewModel : ObservableObject
{
    private readonly IDatabaseService _databaseService;
    private readonly ISettingsService _settingsService;

    [ObservableProperty]
    private int? _entryId;

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

    public AddEditWeightViewModel(
        IDatabaseService databaseService,
        ISettingsService settingsService)
    {
        _databaseService = databaseService;
        _settingsService = settingsService;

        _preferredUnit = _settingsService.PreferredUnit;
    }

    /// <summary>
    /// Called when the EntryId property changes (navigation parameter)
    /// </summary>
    partial void OnEntryIdChanged(int? value)
    {
        if (value.HasValue)
        {
            IsEditMode = true;
            Title = "Edit Weight";
            _ = LoadWeightEntryAsync(value.Value);
        }
        else
        {
            IsEditMode = false;
            Title = "Add Weight";
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
            var entry = await _databaseService.GetWeightEntryByIdAsync(id);
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
            System.Diagnostics.Debug.WriteLine($"Error loading weight entry: {ex.Message}");
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
            WeightError = "Weight must be greater than zero";
            isValid = false;
        }
        else
        {
            WeightError = null;
        }

        // Validate date
        if (Date > DateTime.Today)
        {
            DateError = "Date cannot be in the future";
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

            if (IsEditMode && EntryId.HasValue)
            {
                // Update existing entry
                var entry = await _databaseService.GetWeightEntryByIdAsync(EntryId.Value);
                if (entry != null)
                {
                    entry.WeightKg = weightKg;
                    entry.Date = Date;
                    entry.Notes = Notes;
                    await _databaseService.UpdateWeightEntryAsync(entry);
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
                await _databaseService.InsertWeightEntryAsync(entry);
            }

            // Navigate back
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error saving weight entry: {ex.Message}");
            await Application.Current!.MainPage!.DisplayAlert(
                "Error",
                "Failed to save measurement",
                "OK");
        }
    }

    /// <summary>
    /// Cancels and navigates back
    /// </summary>
    [RelayCommand]
    private async Task CancelAsync()
    {
        await Shell.Current.GoToAsync("..");
    }
}
