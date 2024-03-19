using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using TypeaheadAIWin.Source.Accessibility;
using TypeaheadAIWin.Source.Components.Functions;
using TypeaheadAIWin.Source.Model;
using TypeaheadAIWin.Source.Model.Chat;
using TypeaheadAIWin.Source.Service;
using TypeaheadAIWin.Source.Speech;

namespace TypeaheadAIWin.Source.ViewModel
{
    public partial class ChatPageViewModel : ObservableObject
    {
        private readonly AXInspector _axInspector;
        private readonly ChatService _chatService;
        private readonly FunctionCaller _functionCaller;
        private readonly SoundPlayer _soundPlayer;
        private readonly StreamingSpeechProcessor _speechProcessor;
        private readonly Supabase.Client _supabaseClient;

        private CancellationTokenSource? cancellationToken;

        public ObservableCollection<ChatMessage> ChatMessages { get; } = new();

        // Fires an event with the screenshot
        public event EventHandler<ImageSource> OnScreenshotTaken;

        public ChatPageViewModel(
            AXInspector axInspector,
            ChatService chatService,
            FunctionCaller functionCaller,
            SoundPlayer soundPlayer,
            StreamingSpeechProcessor speechProcessor,
            Supabase.Client supabaseClient)
        {
            _axInspector = axInspector;
            _chatService = chatService;
            _functionCaller = functionCaller;
            _soundPlayer = soundPlayer;
            _speechProcessor = speechProcessor;
            _supabaseClient = supabaseClient;

            _chatService.OnChatResponseReceived += ChatService_OnChatResponseReceived;
        }

        [RelayCommand]
        public void Send(object parameter)
        {
            Cancel();

            var messageInput = parameter as RichTextBox;

            var messageId = Guid.NewGuid();
            var chatMessage = new ChatMessage
            {
                Id = messageId,
                Role = ChatMessageRole.User,
                RootId = ChatMessages.Count > 0 ? ChatMessages[0].RootId : messageId,
                InReplyToId = ChatMessages.Count > 0 ? ChatMessages[^1].Id : null
            };

            // Iterate through the blocks in the RichTextBox
            foreach (Block block in messageInput.Document.Blocks)
            {
                if (block is Paragraph paragraph)
                {
                    foreach (Inline inline in paragraph.Inlines)
                    {
                        if (inline is Run run)
                        {
                            // Append text to the ChatMessage's Text property
                            if (chatMessage.Text != null)
                            {
                                chatMessage.Text += "\r\n" + run.Text;
                            }
                            else
                            {
                                chatMessage.Text = run.Text;
                            }
                        }
                        else if (inline is InlineUIContainer uiContainer && uiContainer.Child is System.Windows.Controls.Image image)
                        {
                            // Extract the ImageSource from the Image and set it in the ChatMessage
                            chatMessage.Image = image.Source;
                        }
                    }
                }
            }

            // Check if there's content to send
            if (!string.IsNullOrEmpty(chatMessage.Text) || chatMessage.Image != null)
            {
                ChatMessages.Add(chatMessage);

                Reply();

                // Clear the MessageInput RichTextBox
                messageInput.Document.Blocks.Clear();
                messageInput.Document.Blocks.Add(new Paragraph());
            }
        }
 
        // Send a message
        public void Reply()
        {
            Cancel();
            cancellationToken = new CancellationTokenSource();


            /// TODO: Add handling for this error.
            var uuid = _supabaseClient.Auth.CurrentUser?.Id ?? throw new InvalidOperationException("User is not authenticated");
            var requestData = new ChatRequest
            {
                Uuid = uuid,
                Messages = ChatMessages.ToList(),
                AppContext = _axInspector.GetCurrentAppContext(),
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
            Cancel();
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

        public void TakeScreenshot()
        {
            var screenshot = _axInspector.TakeScreenshot();
            if (screenshot != null)
            {
                // Kind of silly, but I need to get access to the MessageInput from the view, which is tricky.
                OnScreenshotTaken?.Invoke(this, screenshot);
            }
        }

        // Handle the chat response
        private async void ChatService_OnChatResponseReceived(object sender, ChatResponse e)
        {
            _soundPlayer.Stop();
            Application.Current.Dispatcher.Invoke(() =>
            {
                switch (e.Mode)
                {
                    case ResponseMode.Text:
                        AddToken(e);
                        break;
                    case ResponseMode.Function:
                        AddFunction(e);
                        break;
                    default:
                        Trace.WriteLine("Unhandled response mode");
                        break;
                }
            });
        }

        private void AddFunction(ChatResponse response)
        {
            if (response.Text != null)
            {
                var functionCall = _functionCaller.Parse(response.Text);
                var functionArgs = functionCall.ParseArgs();
                if (functionArgs?.HumanReadable != null)
                {
                    _speechProcessor.ProcessToken(functionArgs.HumanReadable);
                    _speechProcessor.FlushBuffer();
                }

                ChatMessages.Add(new ChatMessage
                {
                    Role = ChatMessageRole.Assistant,
                    IsHidden = true,
                    Text = response.Text,
                    RootId = ChatMessages.Count > 0 ? ChatMessages[0].RootId : Guid.NewGuid(),
                    InReplyToId = ChatMessages.Count > 0 ? ChatMessages[^1].Id : null,
                    FunctionCalls = [functionCall]
                });

                _functionCaller.Call(functionCall);

                _speechProcessor.ProcessToken("Completed Task");
                _speechProcessor.FlushBuffer();

                ChatMessages.Add(new ChatMessage
                {
                    Role = ChatMessageRole.Tool,
                    IsHidden = true,
                    Text = "Completed Task",
                    RootId = ChatMessages.Count > 0 ? ChatMessages[0].RootId : Guid.NewGuid(),
                    InReplyToId = ChatMessages.Count > 0 ? ChatMessages[^1].Id : null,
                    InReplyToFunctionCallId = functionCall.Id
                });

                App.ServiceProvider.GetRequiredService<MainWindowViewModel>().Activated = true;

                Reply();
            }
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