using Avalonia;
using Avalonia.Controls;
using Splat;
using SukiUI.Controls;
using SukiUI.Models;
using SukiUI.Toasts;
using ILogger = Serilog.ILogger;

namespace AquaPP.Views;

public partial class MainWindow : SukiWindow
{
    private readonly ILogger _logger;
    
    public static ISukiToastManager? ToastManager;

    public MainWindow()
    {
        InitializeComponent();
        this.AttachDevTools();

        _logger = Locator.Current.GetService<ILogger>()!;
        
        ToastManager = this.FindControl<SukiToastHost>("ToastHost")!.Manager;
    }
}
