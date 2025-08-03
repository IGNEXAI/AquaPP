using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace AquaPP.Views;

public partial class CustomSplitView : UserControl
{
    
    public CustomSplitView()
    {
        InitializeComponent();
    }
    
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
    
}