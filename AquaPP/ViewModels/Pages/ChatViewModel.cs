using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using AquaPP.AI;
using AquaPP.Controls;
using AquaPP.Models;
using AquaPP.Services;
using Avalonia.Media;
using Avalonia.Threading;
using Microsoft.SemanticKernel;
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

        var userMessage = new ChatMessage { Content = Message, Sender = MessageSender.User, IsLoading = false};
        Messages.Add(userMessage);

        try
        {
            // Prepares the message to send to the agent
            var messageToSend = Message;
            Message = string.Empty; // Clear the input field
            
            // Display the circular loading indicator and scroll to bottom
            var newAgentMessageLoading = new ChatMessage { Content = string.Empty, Sender = MessageSender.Agent, IsLoading = true};
            Messages.Add(newAgentMessageLoading);
            Dispatcher.UIThread.Post(() => RequestScrollToBottom?.Invoke());
            
            // Sends the message to the agent
            var agentResponse = await _agent.SendMessage(messageToSend);
            
            // Removes the loading indicator
            Messages[^1].IsLoading = false;
            
            // Simulating streaming of response 
            int chunkSize = 5; 
            string fullContent = agentResponse.Content ?? string.Empty; 
            string currentDisplayedContent = string.Empty;

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
            ToastNotificationManager.Show(new ToastNotification
            {
                Title = "Error Sending Message",
                Message = "There was an error sending your message. Please try again.",
                Icon = "/Assets/status-failed-svgrepo-com.svg",
                Background = Brushes.OrangeRed,
                BorderBrush =  Brushes.Red
            });
        } catch (TaskCanceledException exception) {
            _logger.Error($"Error when sending message to Gemini chat completion model: {exception}");
            ToastNotificationManager.Show(new ToastNotification
            {
                Title = "Error Sending Message",
                Message = "Agent took too long to respond.",
                Icon = "/Assets/status-failed-svgrepo-com.svg",
                Background = Brushes.OrangeRed,
                BorderBrush =  Brushes.Red
            });
        } catch (Exception exception) {
            _logger.Error($"Error when sending message to Gemini chat completion model: {exception}");
            ToastNotificationManager.Show(new ToastNotification
            {
                Title = "Error Sending Message",
                Message = "Unexpected error encountered. Please try again.",
                Icon = "/Assets/status-failed-svgrepo-com.svg",
                Background = Brushes.OrangeRed,
                BorderBrush =  Brushes.Red
            });
        }

        Message = string.Empty;
    }
}