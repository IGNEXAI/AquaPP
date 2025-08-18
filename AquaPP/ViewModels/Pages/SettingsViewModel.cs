using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Styling;

namespace AquaPP.ViewModels.Pages;

public class SettingsViewModel : PageBase
{
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

    public SettingsViewModel() : base("Settings", "fa-solid fa-gear", 4)
    {
        _selectedTheme = Application.Current?.RequestedThemeVariant ?? ThemeVariant.Default;
    }
}