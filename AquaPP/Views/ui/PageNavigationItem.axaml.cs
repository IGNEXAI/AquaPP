using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace AquaPP.Views.ui;

public partial class PageNavigationItem : UserControl
{
    public PageNavigationItem()
    {
        InitializeComponent();
    }
    
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}