using System;
using SukiUI;

namespace AquaTain.Services;

public class ThemeProvider
{
    public ThemeProvider()
    {
        SukiTheme.GetInstance().OnBaseThemeChanged += variant => { Console.WriteLine("Theme changed triggered !"); };
    }
}