using System;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using AquaPP.ViewModels;

namespace AquaPP;

public class ViewLocator : IDataTemplate
{
    public Control? Build(object? param)
    {
        if (param is null)
            return null;

        var name = param.GetType().FullName!.Replace("ViewModel", "View", StringComparison.Ordinal);
        var type = Type.GetType(name);

        if (type != null)
        {
            return (Control)Activator.CreateInstance(type)!;
        }

        // Fallback for nested view folders
        name = param.GetType().Name.Replace("ViewModel", "View", StringComparison.Ordinal);
        var viewNamespace = param.GetType().Namespace?
            .Replace("ViewModels", "Views.Pages", StringComparison.Ordinal)
            .Replace("ViewModels", "Views.ChatViews", StringComparison.Ordinal);
        
        if (viewNamespace != null)
        {
            type = Type.GetType($"{viewNamespace}.{name}");
            if (type != null)
            {
                return (Control)Activator.CreateInstance(type)!;
            }
        }

        return new TextBlock { Text = "Not Found: " + name };
    }

    public bool Match(object? data)
    {
        return data is ViewModelBase;
    }
}