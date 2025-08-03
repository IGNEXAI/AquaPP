using System;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Styling;
using AquaPP.Models;

namespace AquaPP.Converters;


public class SenderToBrushConverter : IValueConverter
{
    // public static readonly SenderToBrushConverter Instance = new SenderToBrushConverter();
    
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var currentThemeVariant = Application.Current?.ActualThemeVariant;
        
        if (value is MessageSender sender)
        {
            if (currentThemeVariant == ThemeVariant.Light)
            {
                return sender == MessageSender.User ? Brushes.LightGray : Brushes.LightBlue;

            }
            return sender == MessageSender.User ? Brushes.Black : Brushes.DarkBlue;
        }
        return Brushes.Transparent;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class SenderToAlignmentConverter : IValueConverter
{
    // public static readonly SenderToBrushConverter Instance = new SenderToBrushConverter();
    
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is MessageSender sender)
        {
            return sender == MessageSender.User ? HorizontalAlignment.Right : HorizontalAlignment.Left;
        }
        return HorizontalAlignment.Left;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class SenderToIconData : IValueConverter
{
    // public static readonly SenderToBrushConverter Instance = new SenderToBrushConverter();
    public readonly String AgentIconData = "M3 5V20.7929C3 21.2383 3.53857 21.4614 3.85355 21.1464L7.70711 17.2929C7.89464 17.1054 8.149 17 8.41421 17H19C20.1046 17 21 16.1046 21 15V5C21 3.89543 20.1046 3 19 3H5C3.89543 3 3 3.89543 3 5Z";
    public readonly String UserIconData = "M8 7C9.65685 7 11 5.65685 11 4C11 2.34315 9.65685 1 8 1C6.34315 1 5 2.34315 5 4C5 5.65685 6.34315 7 8 7Z";
    
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is MessageSender sender)
        {
            return sender == MessageSender.User ? AgentIconData: UserIconData;
        }
        return UserIconData;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}