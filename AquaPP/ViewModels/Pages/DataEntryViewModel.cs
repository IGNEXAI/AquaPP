using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using AquaPP.Controls;
using AquaPP.Models;
using AquaPP.Services;
using AquaPP.Services.Repositories;
using AquaPP.Views;
using Avalonia.Media;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Splat;
using ILogger = Serilog.ILogger;
using ReactiveUI;
using SukiUI.Toasts;

namespace AquaPP.ViewModels.Pages;

public partial class DataEntryViewModel : ViewModelBase
{
    private readonly ILogger _logger;
    private readonly IWaterQualityRepository _repository;
    private readonly ISukiToastService _toastService; 

    private DateTime _currentDate = DateTime.Now;

    public DateTime CurrentDate
    {
        get => _currentDate;
        set => this.RaiseAndSetIfChanged(ref _currentDate, value);
    }

    private bool _showNoDataMessage;

    public bool ShowNoDataMessage
    {
        get => _showNoDataMessage;
        set => this.RaiseAndSetIfChanged(ref _showNoDataMessage, value);
    }
    
    private ObservableCollection<WaterQualityReading> _selectedReadings = [];
    public ObservableCollection<WaterQualityReading> SelectedReadings
    {
        get => _selectedReadings;
        set => this.RaiseAndSetIfChanged(ref _selectedReadings, value);
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

    public DataEntryViewModel()
    {
        _logger = Locator.Current.GetService<ILogger>()!;
        _logger.Information("DataEntryViewModel initialized.");

        _repository = App.Services.GetRequiredService<IWaterQualityRepository>();
        _toastService = App.Services.GetRequiredService<ISukiToastService>();

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

            _toastService.ShowToast("No Data Found", "No readings were found in the database.");
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
            
            _toastService.ShowToast("No Data Found", $"No readings were found for {CurrentDate:MMMM dd, yyyy}");
        }
        else
        {
            ShowNoDataMessage = false;
            Readings = new ObservableCollection<WaterQualityReading>(data);
        }
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
            
            _toastService.ShowToast("Reading Saved", "All readings were saved successfully");
        }
        catch (ValidationException ex)
        {
            _logger.Error(ex, "Validation error saving readings");
            _toastService.ShowToast("Validation Error", ex.Message);
        }
        catch (Exception e)
        {
            _logger.Error(e, "Error saving readings");
            _toastService.ShowToast("Error Saving Readings", "There was an error saving the readings. Please try again.");
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

        _toastService.ShowToast("Readings Deleted", $"{selected.Count} readings were deleted successfully");
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