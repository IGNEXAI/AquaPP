using System.Collections.Generic;
using Avalonia;
using Avalonia.Styling;
using Serilog;

namespace AquaPP.ViewModels.Pages;

public class SettingsViewModel : PageBase
{
    private ILogger _logger;
    public List<ThemeVariant> Themes { get; } = new() { ThemeVariant.Light, ThemeVariant.Dark };

    private ThemeVariant _selectedTheme;

    public ThemeVariant SelectedTheme
    {
        get => _selectedTheme;
        set
        {
            if (_selectedTheme != value)
            {
                _selectedTheme = value;
                if (Application.Current is not null)
                {
                    Application.Current.RequestedThemeVariant = value;
                }
            }
        }
    }

    public SettingsViewModel(ILogger logger) : base("Settings", "fa-solid fa-gear", 4)
    {
        _logger = logger;
        _selectedTheme = Application.Current?.RequestedThemeVariant ?? ThemeVariant.Default;
    }

    public SettingsViewModel() : base("Settings", "fa-solid fa-gear", 4)
    {
        throw new System.NotImplementedException();
    }
}