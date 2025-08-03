using System;
using AquaPP.Controls;

namespace AquaPP.Services;

public class ToastNotificationManager {
    
    // An event that the UI can subscribe to
    public static event Action<ToastNotification>? OnShowToast;

    // A method that any part of the app can call
    public static void Show(ToastNotification toast)
    {
        OnShowToast?.Invoke(toast);
    }
}