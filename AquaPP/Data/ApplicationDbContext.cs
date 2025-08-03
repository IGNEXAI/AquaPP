using Microsoft.EntityFrameworkCore;
using AquaPP.Models;

namespace AquaPP.Data;

public class ApplicationDbContext : DbContext
{
    public DbSet<WaterQualityReading> WaterQualityReadings { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure your models here.
        // Example: modelBuilder.Entity<WaterQualityReading>().HasKey(w => w.Id);
    }
}

