using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Markup;
using TypeaheadAIWin.Source.Services;
using TypeaheadAIWin.Source.Speech;

namespace TypeaheadAIWin.Source.ViewModel
{
    public partial class ChatMessagesViewModel : ObservableObject
    {
        private readonly IChatService _chatService;
        private readonly StreamingSpeechProcessor _speechProcessor;
        private readonly Supabase.Client _supabaseClient;

        private CancellationTokenSource? streamCancellationTokenSource;

        [ObservableProperty]
        private ObservableCollection<ChatMessage> chatMessages;

        [ObservableProperty]
        private ChatMessage chatDraft;

        [ObservableProperty]
        private string chatDraftRichTextXaml;

        public ChatMessagesViewModel(
            IChatService chatService,
            StreamingSpeechProcessor speechProcessor,
            Supabase.Client supabaseClient) 
        {
            _chatService = chatService;
            _speechProcessor = speechProcessor;
            _supabaseClient = supabaseClient;

            _chatService.OnChatResponseReceived += ChatService_OnChatResponseReceived;

            ChatMessages = new ObservableCollection<ChatMessage>();
            ChatDraft = new ChatMessage()
            {
                Role = ChatMessageRole.User
            };
        }

        [RelayCommand]
        private async Task Send()
        {
            // Cancel the previous stream if it exists
            streamCancellationTokenSource?.Cancel();
            streamCancellationTokenSource?.Dispose();

            streamCancellationTokenSource = new CancellationTokenSource();

            if (!string.IsNullOrEmpty(ChatDraft.Text) || ChatDraft.Image != null)
            {
                ChatMessages.Add(ChatDraft);

                // Reset the draft message
                ChatDraft = new ChatMessage()
                {
                    Role = ChatMessageRole.User
                };

                // Construct ChatService request
                var uuid = _supabaseClient.Auth.CurrentUser?.Id ?? throw new InvalidOperationException("User is not authenticated");

                Trace.WriteLine(uuid);
                var chatRequest = new ChatRequest
                {
                    Uuid = uuid,
                    Messages = ChatMessages.ToList(),
                };

                Trace.WriteLine(chatRequest);
                await _chatService.StreamChatAsync(chatRequest, streamCancellationTokenSource.Token);
            }
            else
            {
                Trace.WriteLine(ChatDraftRichTextXaml);
                Trace.WriteLine("Nothing happened");
            }
        }

        [RelayCommand]
        private void ClearMessages()
        {
            ChatMessages.Clear();
        }

        private void ChatService_OnChatResponseReceived(object sender, ChatResponse e)
        {
            // Handle the chat response, such as appending to a message list
            // Ensure you marshal the call to the UI thread if updating UI elements directly
            Application.Current.Dispatcher.Invoke(() =>
            {
                Trace.WriteLine("chat received");
                AppendToLastAssistantMessage(e);
            });
        }

        private void AppendToLastAssistantMessage(ChatResponse response)
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
                    var chatMessage = new ChatMessage
                    {
                        Role = ChatMessageRole.Assistant,
                        Text = response.Text,
                    };
                    ChatMessages.Add(chatMessage);
                }
            }
        }

        // Helper method to convert FlowDocument to XAML string
        public static string FlowDocumentToXaml(FlowDocument document)
        {
            var range = new TextRange(document.ContentStart, document.ContentEnd);
            using (var stream = new MemoryStream())
            {
                XamlWriter.Save(range, stream);
                stream.Seek(0, SeekOrigin.Begin);
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        // Helper method to load XAML string into FlowDocument
        public static FlowDocument XamlToFlowDocument(string xamlString)
        {
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(xamlString)))
            {
                return XamlReader.Load(stream) as FlowDocument;
            }
        }
    }
}
