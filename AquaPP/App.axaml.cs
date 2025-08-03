using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using AquaPP.Data;
using AquaPP.Services; 
using Microsoft.Extensions.DependencyInjection;
using Avalonia.Markup.Xaml;
using AquaPP.ViewModels;
using AquaPP.Views;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;
using Splat;

namespace AquaPP;

#pragma warning disable SKEXP0070

public partial class App : Application
{
    public static IServiceProvider Services { get; private set; } = null!;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        // Configure and register the logger as before
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Verbose)
            .WriteTo.File("logs/app-log-.txt", rollingInterval: RollingInterval.Day, 
                outputTemplate: "{Timestaecho $DISPLAYmp:yyyy-MM-dd HH.mm.ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
            .CreateLogger();

        Log.Information("Serilog logger configured and registered with Splat.");
        Locator.CurrentMutable.Register(() => Log.Logger);

        BindingPlugins.DataValidators.RemoveAt(0);
        
        // Register all the services needed for the application to run
        Log.Information("Creating new ServiceCollection.");
        var collection = new ServiceCollection();

        try
        {
            Log.Information("Calling AddCommonServices() to register services.");
            // Your AddCommonServices extension method should be located here
            collection.AddCommonServices();
            Log.Information("Finished calling AddCommonServices().");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "An error occurred during AddCommonServices(). Check the service registrations inside that method.");
            // Re-throw the exception so the app still crashes, but we have a log of it.
            throw; 
        }

        try
        {
            Log.Information("Building the ServiceProvider.");
            Services = collection.BuildServiceProvider();
            Log.Information("ServiceProvider built successfully.");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "An error occurred while building the ServiceProvider. This often indicates a dependency resolution problem.");
            throw;
        }
        
        try
        {
            Log.Information("Attempting to create a database scope and perform migration.");
            using var scope = Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            Log.Information("Successfully retrieved ApplicationDbContext from the service provider.");
            
            dbContext.Database.Migrate();
            Log.Information("Database migration completed successfully.");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "An error occurred during database migration. This is the most likely culprit if your app crashes here.");
            throw;
        }

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            DisableAvaloniaDataAnnotationValidation();

            try
            {
                Log.Information("Attempting to get CustomSplitViewModel for the MainWindow.");
                desktop.MainWindow = new MainWindow {
                    DataContext = Services.GetRequiredService<CustomSplitViewModel>() 
                };
                Log.Information("MainWindow and its DataContext set successfully.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while getting the CustomSplitViewModel. This means the view model or one of its dependencies could not be resolved.");
                throw;
            }
        }
        
        Log.Information("Avalonia application initialization completed successfully.");
        base.OnFrameworkInitializationCompleted();
    }

    private void DisableAvaloniaDataAnnotationValidation()
    {
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }
}
