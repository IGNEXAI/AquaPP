using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WaterUtilities.Models;

public class RegulatoryReport
{
    public Guid Id { get; set; } = Guid.NewGuid();
    [Column(TypeName = "varchar(64)")]
    public string ReportType { get; set; } = string.Empty;
    public DateTime ReportingPeriod { get; set; }
    public DateTime DueDate { get; set; }
    public RegulatoryReportStatus Status { get; set; }
    [Column(TypeName = "varchar(64)")]
    public string RegulatoryAgency { get; set; } = string.Empty;
    public Dictionary<string, object> Data { get; set; } = new();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? SubmittedAt { get; set; }
    [Column(TypeName = "varchar(64)")]
    public string? SubmissionReference { get; set; }
    public List<string> ValidationErrors { get; set; } = [];
}

public enum RegulatoryReportStatus
{
    Pending,
    DataCollected,
    Validated,
    Submitted,
    Approved,
    Rejected,
    Failed
}

public class ComplianceThreshold
{
    public string Parameter { get; set; } = string.Empty;
    public decimal MaxValue { get; set; }
    public decimal MinValue { get; set; }
    public string Unit { get; set; } = string.Empty;
}
