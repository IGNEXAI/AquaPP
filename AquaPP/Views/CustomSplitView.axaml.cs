using System;
using System.Reactive;
using AquaPP.Services;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ReactiveUI;
using Splat;
using ILogger = Serilog.ILogger;

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