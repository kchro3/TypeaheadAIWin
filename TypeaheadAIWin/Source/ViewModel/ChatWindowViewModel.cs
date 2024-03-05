using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Media;
using System.Windows;
using TypeaheadAIWin.Source.Model;
using TypeaheadAIWin.Source.Service;
using TypeaheadAIWin.Source.Speech;

namespace TypeaheadAIWin.Source.ViewModel
{
    public class ChatWindowViewModel : ObservableObject
    {
        private readonly ChatService _chatService;
        private readonly SoundPlayer _soundPlayer;
        private readonly StreamingSpeechProcessor _speechProcessor;
        private readonly Supabase.Client _supabaseClient;

        private CancellationTokenSource? cancellationToken;

        public ObservableCollection<ChatMessage> ChatMessages { get; } = new();

        public ChatWindowViewModel(
            ChatService chatService,
            SoundPlayer soundPlayer,
            StreamingSpeechProcessor speechProcessor,
            Supabase.Client supabaseClient)
        {
            _chatService = chatService;
            _soundPlayer = soundPlayer;
            _speechProcessor = speechProcessor;
            _supabaseClient = supabaseClient;

            _chatService.OnChatResponseReceived += ChatService_OnChatResponseReceived;
        }

        // Send a message
        public void SendMessage(ChatMessage message, ApplicationContext appContext)
        {
            Cancel();
            cancellationToken = new CancellationTokenSource();

            ChatMessages.Add(message);

            var uuid = _supabaseClient.Auth.CurrentUser?.Id ?? throw new InvalidOperationException("User is not authenticated");
            var requestData = new ChatRequest
            {
                Uuid = uuid,
                Messages = ChatMessages.ToList(),
                AppContext = appContext,
            };

            // Play the snap sound on a loop
            _soundPlayer.PlayLooping();

            Task.Run(async () =>
            {
                try
                {
                    await _chatService.StreamChatAsync(requestData, cancellationToken.Token);
                }
                catch (Exception ex)
                {
                    // Stop any ongoing sound playback
                    _soundPlayer.Stop();

                    // Execute on UI thread since MessageBox.Show must be called on the UI thread
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        // Show error dialog
                        MessageBox.Show(Application.Current.MainWindow, $"Failed to send message: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    });
                }
            });
        }

        public void Clear()
        {
            ChatMessages.Clear();
        }

        public void Cancel()
        {
            cancellationToken?.Cancel();
            cancellationToken?.Dispose();
            cancellationToken = null;

            _speechProcessor.Cancel();
            _soundPlayer.Stop();
        }

        // Handle the chat response
        private async void ChatService_OnChatResponseReceived(object sender, ChatResponse e)
        {
            _soundPlayer.Stop();
            Application.Current.Dispatcher.Invoke(() =>
            {
                AddToken(e);
            });
        }

        // Add the token to the chat response
        private void AddToken(ChatResponse response)
        {
            if (response.FinishReason != null)
            {
                // Check if the last message in the collection is an assistant message                
                if (ChatMessages.Count > 0 && ChatMessages.Last().Role == ChatMessageRole.Assistant)
                {
                    _speechProcessor.FlushBuffer();
                }
            }
            else if (response.Text != null)
            {
                _speechProcessor.ProcessToken(response.Text);

                // Check if the last message in the collection is an assistant message
                if (ChatMessages.Count > 0 && ChatMessages.Last().Role == ChatMessageRole.Assistant)
                {
                    // Append the text to the last assistant message
                    ChatMessages.Last().Text += response.Text;
                }
                else
                {
                    // If the last message is not an assistant message, create a new one
                    var messageId = Guid.NewGuid();
                    var chatMessage = new ChatMessage
                    {
                        Role = ChatMessageRole.Assistant,
                        Text = response.Text,
                        RootId = ChatMessages.Count > 0 ? ChatMessages[0].RootId : messageId,
                        InReplyToId = ChatMessages.Count > 0 ? ChatMessages[^1].Id : null
                    };
                    ChatMessages.Add(chatMessage);
                }
            }
        }
    }
}
