using System;
using System.Reactive;
using AquaPP.Services;
using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using Splat;
using ILogger = Serilog.ILogger;

namespace AquaPP.ViewModels;

public class CustomSplitViewModel : ViewModelBase
{
    private readonly ILogger _logger = Locator.Current.GetService<ILogger>()!;

    public ReactiveCommand<object, Unit> OpenUrlCommand { get; set; }

    public CustomSplitViewModel()
    {
        var urlService = App.Services.GetRequiredService<IUrlService>();
        
        OpenUrlCommand = ReactiveCommand.CreateFromTask<object>(async (parameter) =>
        {
            _logger.Information("OpenUrlCommand executed. Received parameter type: {type}", parameter?.GetType());

            // --- defensive code starts here ---
            if (parameter is not object[] parameters || parameters.Length < 2)
            {
                _logger.Error("Invalid CommandParameter. Expected an object array with at least 2 elements.");
                return; // Exit gracefully
            }

            if (parameters[0] is not Control control)
            {
                _logger.Error("First element in CommandParameter was not a valid Control.");
                return;
            }

            var url = parameters[1] as string;

            if (string.IsNullOrEmpty(url))
            {
                _logger.Error("Second element in CommandParameter was not a valid URL string.");
                return;
            }

            // --- defensive code ends here ---

            try
            {
                // Now we know control and url are not null
                await urlService.OpenUrlAsync(control, url);
                _logger.Information("OpenUrlCommand: Url opened successfully: {url}", url);
            }
            catch (Exception e)
            {
                // This catch block is still good for handling errors from the _urlService itself
                _logger.Error(e, "Error opening url: {url}", url);
            }
        });
    }
}