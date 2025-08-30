using Avalonia.Controls;
// using Avalonia.Interactivity;
// using AquaTain.ViewModels.Pages;
// using Splat;
// using ILogger = Serilog.ILogger;

namespace AquaTain.Views.Pages;

public partial class ChatView : UserControl
{
    // private ScrollViewer? _chatScrollViewer;
    // private readonly ILogger _logger;
    
    public ChatView()
    {
        InitializeComponent();
        // _logger = Locator.Current.GetService<ILogger>()!; 
    }

    /*protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);

        _chatScrollViewer = this.FindControl<ScrollViewer>("ChatScrollViewer");
        if (_chatScrollViewer != null)
        {
            _logger.Information("Found ChatScrollViewer");
        }
        
        if (DataContext is ChatViewModel viewModel)
        {
            viewModel.RequestScrollToBottom += ScrollToBottom;
        }
    }

    protected override void OnUnloaded(RoutedEventArgs e)
    {
        base.OnUnloaded(e);

        if (DataContext is ChatViewModel viewModel)
        {
            viewModel.RequestScrollToBottom -= ScrollToBottom;
        }
    }

    private void ScrollToBottom()
    {
        _chatScrollViewer?.ScrollToEnd();
    }
    */
}