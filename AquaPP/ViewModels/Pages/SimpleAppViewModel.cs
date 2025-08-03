using System;
using System.Diagnostics;
using System.Reactive;
using Avalonia.Threading;
using UnitsNet;
using AquaPP.Services.WaterTreatment.PrimaryTreatment;
using ReactiveUI;

namespace AquaPP.ViewModels.Pages;

public partial class SimpleAppViewModel: ViewModelBase
{
    private bool _isBusy;

    public bool IsBusy
    {
        get => _isBusy;
        set => this.RaiseAndSetIfChanged(ref _isBusy, value);
    }

    // Properties bound to your input TextBoxes (example)
    public double ParticleDiameterValue { get; set; }
    public double ParticleDensityValue { get; set; } = 2650;

    // Property bound to your output TextBlock
    private string _settlingVelocityResult = "N/A";

    public string SettlingVelocityResult
    {
        get => _settlingVelocityResult;
        set => this.RaiseAndSetIfChanged(ref _settlingVelocityResult, value);
    }

    // Command bound to your calculate Button
    public ReactiveCommand<Unit, Unit> CalculateSettlingVelocityCommand { get; set; }

    public SimpleAppViewModel()
    {
        CalculateSettlingVelocityCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var stopwatch = Stopwatch.StartNew();
            Dispatcher.UIThread.Post(() => { IsBusy = true; });
            Debug.WriteLine("Command execution started. IsBusy set to true.");

            try
            {
                // Access UI-related properties on the UI thread
                Debug.WriteLine("Fetching input values from UI thread.");
                var particleDiameterValue = await Dispatcher.UIThread.InvokeAsync(() => ParticleDiameterValue);
                var particleDensityValue = await Dispatcher.UIThread.InvokeAsync(() => ParticleDensityValue);

                Debug.WriteLine(
                    $"Particle Diameter: {particleDiameterValue}, Particle Density: {particleDensityValue}");

                Debug.WriteLine("Inputs converted to physical quantities.");

                var result = PrimarySedimentation.CalculateTerminalSettlingVelocity(
                    Length.FromMicrometers(particleDiameterValue),
                    Density.FromKilogramsPerCubicMeter(particleDensityValue),
                    Density.FromKilogramsPerCubicMeter(997),
                    Acceleration.FromMetersPerSecondSquared(9.81),
                    DynamicViscosity.FromPascalSeconds(8.90e-4),
                    Temperature.FromDegreesCelsius(25),
                    Ratio.FromDecimalFractions(1),
                    0.001
                );
                Debug.WriteLine($"Settling velocity calculated: {result}");
                Dispatcher.UIThread.Post(() =>
                {
                    SettlingVelocityResult = result.ToString("F4");
                    Debug.WriteLine($"Result updated in UI: {SettlingVelocityResult}");
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error during calculation: {ex.Message}");
            }
            finally
            {
                Dispatcher.UIThread.Post(() =>
                {
                    IsBusy = false;
                    Debug.WriteLine("Command execution completed. IsBusy set to false.");
                });
                stopwatch.Stop();
                Debug.WriteLine($"Warm-up calculation took: {stopwatch.ElapsedMilliseconds} ms");
            }
        });
    }
}

