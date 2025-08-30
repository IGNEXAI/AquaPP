using System;
using AquaTain.ViewModels.Pages;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AquaTain.Services;

public class PageNavigationService
{
    public Action<Type>? NavigationRequested { get; set; }

    public void RequestNavigation<T>() where T : PageBase
    {
        NavigationRequested?.Invoke(typeof(T));
    }
}
