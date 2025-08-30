using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WaterUtilities.Models;

public class ReportingSchedule
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Column(TypeName = "varchar(64)")]
    public string ReportType { get; set; } = string.Empty;
    
    [Column(TypeName = "varchar(64)")]
    public string RegulatoryAgency { get; set; } = string.Empty;
    
    public ReportingFrequency Frequency { get; set; }
    public DateTime NextDueDate { get; set; }
    public bool IsActive { get; set; } = true;
    public List<ComplianceThreshold> Thresholds { get; set; } = [];
}

public enum ReportingFrequency
{
    Monthly,
    Quarterly,
    SemiAnnual,
    Annual,
    Weekly,
    Daily
}
