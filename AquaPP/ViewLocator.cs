using System;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using AquaPP.ViewModels;
using Splat;
using ILogger = Serilog.ILogger;

namespace AquaPP;

public class ViewLocator : IDataTemplate
{
    private readonly ILogger _logger = Locator.Current.GetService<ILogger>()!;
    
    public Control? Build(object? param)
    {
         
        
        if (param is null)
            return null;

        var name = param.GetType().FullName!.Replace("ViewModel", "View", StringComparison.Ordinal);
        var type = Type.GetType(name);

        // Add debugging
        _logger.Information($"ViewLocator: Looking for {name}");

        if (type != null)
        {
            _logger.Information($"ViewLocator: Found {type.FullName}");
            return (Control)Activator.CreateInstance(type)!;
        }

        // Fallback for nested view folders
        name = param.GetType().Name.Replace("ViewModel", "View", StringComparison.Ordinal);
        var viewNamespace = param.GetType().Namespace?
            .Replace("ViewModels.Pages", "Views.Pages", StringComparison.Ordinal);
        
        if (viewNamespace != null)
        {
            var fullName = $"{viewNamespace}.{name}";
            type = Type.GetType(fullName);
            _logger.Information($"ViewLocator: Fallback looking for {fullName}");
            
            if (type != null)
            {
                _logger.Information($"ViewLocator: Fallback found {type.FullName}");
                return (Control)Activator.CreateInstance(type)!;
            }
        }

        _logger.Information($"ViewLocator: Not found, returning TextBlock");
        return new TextBlock { Text = $"Not Found: {param.GetType().FullName}" };
    }

    public bool Match(object? data)
    {
        return data is ViewModelBase;
    }
}