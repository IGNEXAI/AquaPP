using Microsoft.EntityFrameworkCore;
using WaterUtilities.Data.Converters;
using WaterUtilities.Models;

namespace WaterUtilities.Data;

public class ApplicationDbContext : DbContext
{
    public DbSet<RegulatoryReport> RegulatoryReports { get; set; }
    public DbSet<ReportingSchedule> ReportingSchedules { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RegulatoryReport>()
            .Property(r => r.Data)
            .HasConversion<ReportDataConverter>();
        
        modelBuilder.Entity<RegulatoryReport>()
            .Property(data => data.Status)
            .HasConversion<ReportStatusConverter>();
        
        modelBuilder.Ignore<ComplianceThreshold>();
        modelBuilder.Ignore<WaterQualityData>();
        modelBuilder.Ignore<Measurement>();

        base.OnModelCreating(modelBuilder);
    }

}