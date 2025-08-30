using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Metrics;

namespace WaterUtilities.Models;

public class WaterQualityData
{
    [Key]
    public Guid Id { get; set; }
    
    public DateTime TimeStamp = DateTime.Now; // When the reading was taken
    
    public Dictionary<string, Measurement> Parameters { get; set; } = [];
    
    public bool MeetsStandards { get; set; }
}


public class Measurement
{
    public double Value { get; set; }
    public string Unit { get; set; }

    public override string ToString() =>  $"{Value} {Unit}";
}
