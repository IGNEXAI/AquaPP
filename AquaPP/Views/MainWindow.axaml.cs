using System;
using System.Threading.Tasks;
using AquaPP.Controls;
using AquaPP.Services;
using Avalonia;
using Avalonia.Controls;
using Splat;
using ILogger = Serilog.ILogger;

namespace AquaPP.Views;

public partial class MainWindow : Window
{
    private readonly ILogger _logger;
    
    public MainWindow()
    {
        InitializeComponent();
        this.AttachDevTools();

        _logger = Locator.Current.GetService<ILogger>()!;
        
        // Subscribe to the static event
        ToastNotificationManager.OnShowToast += ShowToastNotification;
    }
    
    private async void ShowToastNotification(ToastNotification toast)
    {
        try
        {
            ToastContainer?.Children.Add(toast);

            // 2. Wait for 3 seconds (3000 milliseconds) without blocking the UI
            await Task.Delay(3000);

            // 3. Remove the control
            ToastContainer?.Children.Remove(toast);
        }
        catch (Exception exception)
        {
            _logger.Information("There was a problem trying to add the toast to the ui: {exception}", exception);
        }
    }
}