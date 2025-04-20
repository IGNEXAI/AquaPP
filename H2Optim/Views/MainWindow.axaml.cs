using System;
using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace H2Optim.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {   
        InitializeComponent();
    }
    private void ButtonOnClick(object? sender, RoutedEventArgs e)
    {
        Debug.WriteLine("Click!");
    }
}