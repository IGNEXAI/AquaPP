using System;
using System.IO;
using AquaPP.Data;
using AquaPP.Services.Repositories;
using AquaPP.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AquaPP.Services;

public static class ServiceCollectionExtensions
{
    public static void AddCommonServices(this IServiceCollection collection)
    {
        // Add logging (optional but highly recommended for debugging Semantic Kernel)
        collection.AddLogging(builder =>
        {
            builder.SetMinimumLevel(LogLevel.Trace); // Set appropriate log level
        });
        
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        var dbPath = Path.Join(path, "aquapp.db");
        
        // Setup database context
        collection.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlite($"Data Source={dbPath}"));
        
        collection.AddScoped<IWaterQualityRepository, WaterQualityRepository>();
        collection.AddSingleton<CustomSplitViewModel>();
    }
}