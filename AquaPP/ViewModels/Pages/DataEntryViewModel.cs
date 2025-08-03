using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using System.Windows.Input;
using AquaPP.Controls;
using AquaPP.Models;
using AquaPP.Services;
using AquaPP.Services.Repositories;
using Avalonia.Media;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Splat;
using ILogger = Serilog.ILogger;
using ReactiveUI;

namespace AquaPP.ViewModels.Pages;

public partial class DataEntryViewModel : ViewModelBase
{
    private readonly ILogger _logger;
    private readonly IWaterQualityRepository _repository;
    
    private bool _editModeEnabled = true;
    public bool EditModeEnabled
    {
        get => _editModeEnabled;
        set => this.RaiseAndSetIfChanged(ref _editModeEnabled, value);
    }
    
    private DateTime _currentDate = DateTime.Now;
    public DateTime CurrentDate
    {
        get => _currentDate;
        set => this.RaiseAndSetIfChanged(ref _currentDate, value);
    }

    public ICommand ToggleEditModeCommand { get; }
    public ICommand SelectedDateChangedCommand { get; set; }
    public ICommand AddBlankReadingRowCommand { get; }
    

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
        
        ToggleEditModeCommand = new RelayCommand(ToggleEditMode);
        AddBlankReadingRowCommand = new RelayCommand(AddBlankReadingRow);
        SelectedDateChangedCommand = new RelayCommand<DateTime?>(SelectedDateChanged);

    }

    private void ToggleEditMode()
    {
        EditModeEnabled = !EditModeEnabled;
        _logger.Information("Toggled Edit Mode to {EditModeEnabled}.", EditModeEnabled ? "Enabled" : "Disabled");
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

        if (data.Count == 0)
        {
            _logger.Information("No data found in the database.");
            _logger.Information("Displaying temporary data");
            
            ToastNotificationManager.Show(new ToastNotification
            {
                Title = "No data found",
                Icon = "/Assets/status-failed-svgrepo-com.svg",
                Message = "No data found was found. Using temporary placeholder data for showcase",
                Background = Brushes.LightGray,
                BorderBrush =  Brushes.Gray
            });

            Readings.Clear();

            for (int idx = 0; idx < 10; idx++)
            {
                Readings.Add(new WaterQualityReading() );
            }
        }
        else
        {
            Readings = new ObservableCollection<WaterQualityReading>(data);
        }
    }
    
    [RelayCommand]
    private async Task LoadCurrentDateReadingsAsync()
    {
        var data = await _repository.GetReadingsByDateAsync(CurrentDate);
        
        _logger.Information($"LoadCurrentDateReadingsAsync returned: {data.Count} readings.");

        if (data.Count == 0)
        {
            _logger.Information("No data found in the database.");
            _logger.Information("Displaying temporary data");
            
            ToastNotificationManager.Show(new ToastNotification
            {
                Title = "No data found",
                Icon = "/Assets/status-failed-svgrepo-com.svg",
                Message = "No data found was found. Using temporary placeholder data for showcase",
                Background = Brushes.LightGray,
                BorderBrush =  Brushes.Gray
            });
            
            Readings.Clear();
            
            for (int idx = 0; idx < 8; idx++)
            {
                Readings.Add(new WaterQualityReading());
            }
        }
        else
        {
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
            ToastNotificationManager.Show(new ToastNotification
            {
                Title = "Readings Saved",
                Message = "All readings were saved successfully",
                Icon = "/Assets/check-mark-svgrepo-com.svg",
                Background = Brushes.LightGreen,
                BorderBrush =  Brushes.Green
            });
        } catch (ValidationException ex)
        {
            _logger.Error(ex, "Validation error saving readings");
            ToastNotificationManager.Show(new ToastNotification
            {
                Title = "Validation Error",
                Message = ex.Message,
                Icon = "/Assets/status-failed-svgrepo-com.svg",
                Background = Brushes.OrangeRed,
                BorderBrush = Brushes.Red
            });
        }
        catch (Exception e)
        {
            _logger.Error(e, "Error saving readings");
            ToastNotificationManager.Show(new ToastNotification
            {
                Title = "Error Saving Readings",
                Icon = "/Assets/status-failed-svgrepo-com.svg",
                Message = "There was an error saving the readings. Please try again.",
                Background = Brushes.OrangeRed,
                BorderBrush =  Brushes.Red
            });
        }
    }
    
    private void AddBlankReadingRow()
    {
        // Create a new, empty model instance and add it to the collection.
        // The DataGrid will automatically update.
        Readings.Add(new WaterQualityReading());
    }
}