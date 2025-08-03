using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using AquaPP.AI;
using AquaPP.Models;
using Microsoft.SemanticKernel;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using ReactiveUI;
using Splat;
using ILogger = Serilog.ILogger;

namespace AquaPP.ViewModels.Pages;

public partial class ChatViewModel : ViewModelBase
{
    private readonly Agent _agent;
    private string _message = string.Empty;
    private readonly ILogger _logger;

    public ObservableCollection<ChatMessage> Messages { get; }

    public event Action? RequestScrollToBottom;

    public ChatViewModel()
    {
        _logger = Locator.Current.GetService<ILogger>()!;
        _agent = new Agent();
        Messages = new ObservableCollection<ChatMessage>();
        SendCommand = new AsyncRelayCommand(SendMessage);
        
        _logger.Information("ChatViewModel initialized.");
    }

    public string Message
    {
        get => _message;
        set => this.RaiseAndSetIfChanged(ref _message, value);
    }

    public ICommand SendCommand { get; }

    private async Task SendMessage()
    {
        if (string.IsNullOrWhiteSpace(Message))
        {
            return;
        }

        var userMessage = new ChatMessage { Content = Message, Sender = MessageSender.User };
        Messages.Add(userMessage);

        try
        {
            var agentResponse = await _agent.SendMessage(Message);

            var newAgentMessage = new ChatMessage { Content = agentResponse.ToString(), Sender = MessageSender.Agent };
            Messages.Add(newAgentMessage);
            
            // Streaming chat didn't so we had to remove
            /*await foreach (var chunk in agentResponse)
            {
                _logger.Debug("Response item: {ItemType} - {ItemText}", chunk.GetType().Name, chunk.ToString());

                // Adding chunks of agent response to the ui
                foreach (char character in chunk.ToString())
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(10));
                    Messages[^1].Content = Messages.Last().Content + character;
                    RequestScrollToBottom?.Invoke();
                }
            }*/
            
            int chunkSize = 5; // Define your chunk size
            string fullContent = agentResponse.Content ?? string.Empty; // Get the full content string
            string currentDisplayedContent = string.Empty; // To build up the displayed content

            // Loop through the string in chunks
            for (int i = 0; i < fullContent.Length; i += chunkSize)
            {
                // Calculate the length of the current chunk, ensuring it doesn't go out of bounds
                int length = Math.Min(chunkSize, fullContent.Length - i);
                string chunk = fullContent.Substring(i, length);

                await Task.Delay(TimeSpan.FromMilliseconds(13));

                // Append the chunk to the current displayed content
                currentDisplayedContent += chunk;
                
                if (Messages != null && Messages.Any())
                {
                    Messages[^1].Content = currentDisplayedContent;
                    RequestScrollToBottom?.Invoke();
                }
            }
            
        } catch (HttpOperationException ex)
        {
            _logger.Error($"Error when sending message to Gemini chat completion model: {ex}");
            var box = MessageBoxManager
                .GetMessageBoxStandard("Error", "Internet connection error. Please check your network settings and try again",
                    ButtonEnum.YesNo);

            var result = await box.ShowAsync();
        }

        Message = string.Empty;
    }
}