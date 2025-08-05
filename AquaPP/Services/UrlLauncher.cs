using System;
using System.Threading.Tasks;
using AquaPP.Controls;
using Avalonia.Controls;
using Avalonia.Media;
using Splat;
using ILogger = Serilog.ILogger;


namespace AquaPP.Services;

public interface IUrlService
{
    Task OpenUrlAsync(Control control, string url);
}

public class UrlService : IUrlService
{   
    private readonly ILogger _logger = Locator.Current.GetService<ILogger>()!;
    
    public async Task OpenUrlAsync(Control control, string url)
    {
        if (Uri.TryCreate(url, UriKind.Absolute, out Uri? uri))
        {
            var topLevel = TopLevel.GetTopLevel(control);
            
            if (topLevel == null)
            {
                _logger.Error("Failed to open URL: {url}. TopLevel is null.", url);
                return;
            }
            
            var launcher = topLevel.Launcher;
            
            _logger.Information("Opening URL: {url}", url);
            
            // LaunchUriAsync returns a boolean indicating success
            var success = await launcher.LaunchUriAsync(uri);
            
            
            if (!success)
            {
                _logger.Error("Failed to open URL: {url}", url);
                ToastNotificationManager.Show(new ToastNotification
                {
                    Title = "Error Opening URL",
                    Message = "Failed to open the URL. Please check your network settings and try again.",
                    Icon = "/Assets/status-failed-svgrepo-com.svg",
                    Background = Brushes.OrangeRed,
                    BorderBrush =  Brushes.Red
                });
            }
            else
            {
                _logger.Information("URL opened successfully: {success}", success);
            }
        }
    }
}