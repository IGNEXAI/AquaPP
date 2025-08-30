using CommunityToolkit.Mvvm.ComponentModel;

namespace AquaTain.ViewModels.Pages;

public partial class PageBase(string displayName, string icon, int index = 0) : ObservableValidator
{
    [ObservableProperty] private string _displayName = displayName;
    [ObservableProperty] private string _icon = icon;
    [ObservableProperty] private int _index = index;
}