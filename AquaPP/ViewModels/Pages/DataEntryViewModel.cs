using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using AquaPP.Models;
using AquaPP.Services.Repositories;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Splat;
using SukiUI.Toasts;
using ILogger = Serilog.ILogger;

namespace AquaPP.ViewModels.Pages;

public partial class DataEntryViewModel : PageBase
{
    private readonly ILogger _logger;
    private readonly IWaterQualityRepository _repository;
    private readonly ISukiToastManager _toastService;
    private bool _isInitialLoad = true; // Flag to track initial load

    [ObservableProperty]
    private DateTime _currentDate = DateTime.Now;
    
    [ObservableProperty]
    private bool _showNoDataMessage;
    
    
    private ObservableCollection<WaterQualityReading> _selectedReadings = [];
    public ObservableCollection<WaterQualityReading> SelectedReadings
    {
        get => _selectedReadings;
        set => SetProperty(ref _selectedReadings, value);
    }

    public ICommand SelectedDateChangedCommand { get; set; }
    public ICommand AddBlankReadingRowCommand { get; }

    public event Action? RequestScrollToBottom;


    private ObservableCollection<WaterQualityReading> _readings = [];

    public ObservableCollection<WaterQualityReading> Readings
    {
        get => _readings;
        set
        {
            if (_readings == value) return;

            _readings.Clear();
            foreach (var item in value)
            {
                _readings.Add(item);
            }
        }
    }

    public DataEntryViewModel(IWaterQualityRepository repository, ISukiToastManager toastManager) 
    : base("Data Entry", "fa-solid fa-database", 3)
    {
        _logger = Locator.Current.GetService<ILogger>()!;
        _logger.Information("DataEntryViewModel initialized.");

        _repository = repository;
        _toastService = toastManager;

        AddBlankReadingRowCommand = new RelayCommand(AddBlankReadingRow);
        SelectedDateChangedCommand = new RelayCommand<DateTime?>(SelectedDateChanged);

        SelectedReadings.CollectionChanged += (_, _) => DeleteSelectedReadingsCommand.NotifyCanExecuteChanged();

        ShowNoDataMessage = true;

        Dispatcher.UIThread.Post(async void () =>
        {
            try
            {
                await LoadCurrentDateReadingsAsync();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to load current date readings.");
            }
        });
    }

    private void SelectedDateChanged(DateTime? date)
    {
        if (date.HasValue && CurrentDate != date.Value)
        {
            CurrentDate = date.Value;
        }

        // Clear the displayed reading and load the reading for the current selected date
        LoadCurrentDateReadingsCommand.Execute(null);
    }
    

    [RelayCommand]
    private async Task LoadAllReadingsAsync()
    {
        var data = await _repository.GetAllReadingsAsync();

        _logger.Information($"LoadReadingAsync returned: {data}");

        Readings.Clear();
        if (data.Count == 0)
        {
            _logger.Information("No data found in the database.");
            ShowNoDataMessage = true;

            _toastService.CreateToast()
                .WithTitle("No Data Found")
                .WithContent("No readings were found in the database.")
                .Dismiss().After(TimeSpan.FromSeconds(3))
                .Dismiss().ByClicking()
                .Queue();
        }
        else
        {
            ShowNoDataMessage = false;
            Readings = new ObservableCollection<WaterQualityReading>(data);
        }
    }

    [RelayCommand]
    private async Task LoadCurrentDateReadingsAsync()
    {
        var data = await _repository.GetReadingsByDateAsync(CurrentDate);

        _logger.Information($"LoadCurrentDateReadingsAsync returned: {data.Count} readings.");

        Readings.Clear();

        if (data.Count == 0)
        {
            _logger.Information("No data found in the database.");
            ShowNoDataMessage = true;

            // Only show toast if not initial load
            if (!_isInitialLoad)
            {
                _toastService.CreateToast()
                    .WithTitle("No Data Found")
                    .WithContent($"No readings were found for {CurrentDate:MMMM dd, yyyy}")
                    .Dismiss().After(TimeSpan.FromSeconds(3))
                    .Dismiss().ByClicking()
                    .Queue();
            }
        }
        else
        {
            ShowNoDataMessage = false;
            Readings = new ObservableCollection<WaterQualityReading>(data);
        }

        // After the first load, set _isInitialLoad to false
        _isInitialLoad = false;
    }
    

    [RelayCommand]
    private async Task SaveReadings()
    {
        try
        {
            // Validate all readings first
            foreach (var reading in Readings)
            {
                var validationContext = new ValidationContext(reading);
                var validationResults = new List<ValidationResult>();

                if (!Validator.TryValidateObject(reading, validationContext, validationResults, true))
                {
                    var errors = string.Join(", ", validationResults.Select(r => r.ErrorMessage));
                    throw new ValidationException($"Validation failed: {errors}");
                }
            }

            for (var idx = 0; idx < Readings.Count; idx++)
            {
                _logger.Information("Saving reading {idx} with id {Id}: {Reading}", idx, Readings[idx].Id, Readings[idx]);

                // Check if reading already exists
                var existingReading = await _repository.GetReadingByIdAsync(Readings[idx].Id);

                if (existingReading == null)
                {
                    await _repository.AddReadingAsync(Readings[idx]);
                }
                else
                {
                    await _repository.UpdateReadingAsync(Readings[idx]);
                }
            }

            _toastService.CreateToast()
                .WithTitle("Reading Saved")
                .WithContent("All readings were saved successfully")
                .Dismiss().After(TimeSpan.FromSeconds(3))
                .Dismiss().ByClicking()
                .Queue();
        }
        catch (ValidationException ex)
        {
            _logger.Error(ex, "Validation error saving readings");
            _toastService.CreateToast()
                .WithTitle("Validation Error")
                .WithContent(ex.Message)
                .Dismiss().After(TimeSpan.FromSeconds(3))
                .Dismiss().ByClicking()
                .Queue();
        }
        catch (Exception e)
        {
            _logger.Error(e, "Error saving readings");
            _toastService.CreateToast()
                .WithTitle("Error Saving Readings")
                .WithContent("There was an error saving the readings. Please try again.")
                .Dismiss().After(TimeSpan.FromSeconds(3))
                .Dismiss().ByClicking()
                .Queue();
        }
    }
    

    [RelayCommand(CanExecute = nameof(CanDelete))]
    private async Task DeleteSelectedReadings()
    {
        var selected = SelectedReadings.ToList();

        foreach (var reading in selected)
        {
            Readings.Remove(reading);
            if (reading.Id != Guid.Empty)
            {
                await _repository.DeleteReadingAsync(reading.Id);
            }
        }

        _toastService.CreateToast()
            .WithTitle("Readings Deleted")
            .WithContent($"{selected.Count} readings were deleted successfully")
            .Dismiss().After(TimeSpan.FromSeconds(3))
            .Dismiss().ByClicking()
            .Queue();
    }

    private bool CanDelete()
    {
        return SelectedReadings.Any();
    }

    private void AddBlankReadingRow()
    {
        // Create a new, empty model instance and add it to the collection.
        // The DataGrid will automatically update.
        Readings.Add(new WaterQualityReading());
        RequestScrollToBottom?.Invoke();
    }
}