using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.ComponentModel.DataAnnotations;

namespace AquaTain.Models;

public partial class WaterQualityReading : ObservableValidator
{
    [ObservableProperty] private Guid _id;
    
    [ObservableProperty] private DateTime _timestamp = DateTime.Now; // When the reading was taken

    [ObservableProperty]
    [Required(ErrorMessage = "Location is required")]
    [StringLength(100, ErrorMessage = "Location must be less than 100 characters")] private string _location = string.Empty;

    // Key Water Quality Parameters - add more as needed
    [ObservableProperty] 
    [Range(0, 14, ErrorMessage = "pH must be between 0 and 14")]
    private double _pH;

    [ObservableProperty] 
    [Range(0, double.MaxValue, ErrorMessage = "Conductivity must be positive")]
    private double _conductivity; // Unit: µS/cm

    [ObservableProperty] 
    [Range(0, double.MaxValue, ErrorMessage = "Turbidity must be positive")]
    private double _turbidity; // Unit: NTU (Nephelometric Turbidity Units)

    [ObservableProperty] 
    [Range(0, double.MaxValue, ErrorMessage = "Chlorine residual must be positive")]
    private double _chlorineResidual; // Unit: mg/L

    [ObservableProperty] private string _notes = string.Empty; // Any observations

    public WaterQualityReading()
    {
        Id = Guid.NewGuid();
    }
}