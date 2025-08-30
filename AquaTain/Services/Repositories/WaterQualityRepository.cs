using AquaTain.Data;
using AquaTain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Splat;
using ILogger = Serilog.ILogger;

namespace AquaTain.Services.Repositories;

public interface IWaterQualityRepository
{
    Task<List<WaterQualityReading>> GetAllReadingsAsync();
    Task<WaterQualityReading?> GetReadingByIdAsync(Guid id);
    Task<List<WaterQualityReading>> GetReadingsByDateAsync(DateTime date);
    Task AddReadingAsync(WaterQualityReading reading);
    Task UpdateReadingAsync(WaterQualityReading reading);
    Task DeleteReadingAsync(Guid id);
}

public class WaterQualityRepository(ApplicationDbContext context) : IWaterQualityRepository
{
    private readonly ILogger _logger = Locator.Current.GetService<ILogger>()!;

    public async Task<List<WaterQualityReading>> GetAllReadingsAsync()
    {
        return await context.WaterQualityReadings.ToListAsync();
    }

    public async Task<WaterQualityReading?> GetReadingByIdAsync(Guid id)
    {
        var data = await context.WaterQualityReadings.FindAsync(id);
        
        if (data == null)  _logger.Information("No data found for id {id}", id);
        
        return data;
    }
    
    public async Task<List<WaterQualityReading>>  GetReadingsByDateAsync(DateTime date)
    {
        _logger.Information("Getting all readings for date {date}", date);
        // The dates are stored in YYYY-MM-DD HH:mm:ss format. But we should only query based on the day and not the exact time
        var data = await context.WaterQualityReadings
            .Where(r => r.Timestamp.Date == date.Date)
            .ToListAsync();
        
        if (data.Count == 0)  _logger.Information("No data found for date {date}", date);
        
        return data;
    }

    public async Task AddReadingAsync(WaterQualityReading reading)
    {
        try
        {
            ValidateReading(reading);
            await context.WaterQualityReadings.AddAsync(reading);
            await context.SaveChangesAsync();
        } 
        catch (Exception e)
        {
            _logger.Error(e, "Error adding reading");
            throw; 
        }
    }

    public async Task UpdateReadingAsync(WaterQualityReading reading)
    {
        try
        {
            ValidateReading(reading);
            context.WaterQualityReadings.Update(reading);
            await context.SaveChangesAsync();
        } 
        catch (Exception e)
        {
            _logger.Error(e, "Error updating reading");
            throw; 
        }
    }

    public async Task DeleteReadingAsync(Guid id)
    {
        var reading = await context.WaterQualityReadings.FindAsync(id);
        if (reading != null)
        {
            context.WaterQualityReadings.Remove(reading);
            await context.SaveChangesAsync();
        }
        else
        {
            _logger.Information("No data found for id {id}", id);
        }
    }
    
    private void ValidateReading(WaterQualityReading reading)
    {
        var validationContext = new ValidationContext(reading);
        var validationResults = new List<ValidationResult>();
    
        if (!Validator.TryValidateObject(reading, validationContext, validationResults, true))
        {
            var errors = string.Join(", ", validationResults.Select(r => r.ErrorMessage));
            throw new ValidationException($"Invalid reading: {errors}");
        }
    }
}