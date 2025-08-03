using CommunityToolkit.Mvvm.ComponentModel;

namespace AquaPP.Models;

public enum MessageSender
{
    User,
    Agent
}

public partial class ChatMessage : ObservableObject
{
    [ObservableProperty]
    private string? _content;
    public MessageSender Sender { get; set; }
}