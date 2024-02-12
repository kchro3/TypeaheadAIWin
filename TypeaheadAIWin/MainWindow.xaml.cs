using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using System.IO;
using System.Collections.ObjectModel;
using System.Media;
using System.Net.Http;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using TypeaheadAIWin.Source;
using MahApps.Metro.Controls;
using TypeaheadAIWin.Source.Accessibility;
using TypeaheadAIWin.Source.Speech;
using TypeaheadAIWin.Views;
using TypeaheadAIWin.Source.ViewModel;

namespace TypeaheadAIWin
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private const int WM_HOTKEY = 0x0312;
        private const int HOTKEY_ID = 9000;
        private const uint MOD_CTRL = 0x0002; // Control key
        private const uint MOD_ALT = 0x0001;  // Alt key
        private const uint VK_T = 0x54;       // T key

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private readonly HttpClient client;
        private readonly Supabase.Client _supabaseClient;
        private readonly AXInspector _axInspector;
        private readonly StreamingSpeechProcessor _speechProcessor;
        private ChatMessagesViewModel _viewModel;

        ObservableCollection<ChatMessage> chatMessages = [];
        private CancellationTokenSource? streamCancellationTokenSource;

        private SoundPlayer audio;

        public MainWindow(
            Supabase.Client supabaseClient,
            AXInspector axInspector,
            ChatMessagesViewModel chatMessagesViewModel,
            StreamingSpeechProcessor speechProcessor
        ) {

            InitializeComponent();
            _viewModel = chatMessagesViewModel;
            this.DataContext = _viewModel;
            // Event to update ViewModel when the RichTextBox content changes
            MessageInput.TextChanged += (s, e) =>
            {
                Trace.WriteLine("on text changed");
                var document = MessageInput.Document;
                _viewModel.ChatDraftRichTextXaml = ChatMessagesViewModel.FlowDocumentToXaml(document);
            };

            _supabaseClient = supabaseClient;
            _axInspector = axInspector;
            _speechProcessor = speechProcessor;

            client = new HttpClient();

            ChatHistoryListView.ItemsSource = chatMessages;
            chatMessages.CollectionChanged += ChatMessages_CollectionChanged;

            audio = new SoundPlayer(Properties.Resources.snap);
            audio.Load();
        }

        //private void SendButton_Click(object sender, RoutedEventArgs e)
        //{
        //    streamCancellationTokenSource?.Cancel();
        //    _speechProcessor.Cancel();
        //    audio.Stop();

        //    var chatMessage = new ChatMessage
        //    {
        //        Role = ChatMessageRole.User,
        //    };

        //    // Iterate through the blocks in the RichTextBox
        //    foreach (Block block in MessageInput.Document.Blocks)
        //    {
        //        if (block is Paragraph paragraph)
        //        {
        //            foreach (Inline inline in paragraph.Inlines)
        //            {
        //                if (inline is Run run)
        //                {
        //                    // Append text to the ChatMessage's Text property
        //                    if (chatMessage.Text != null)
        //                    {
        //                        chatMessage.Text += "\r\n" + run.Text;
        //                    }
        //                    else
        //                    {
        //                        chatMessage.Text = run.Text;
        //                    }
        //                }
        //                else if (inline is InlineUIContainer uiContainer && uiContainer.Child is System.Windows.Controls.Image image)
        //                {
        //                    // Extract the ImageSource from the Image and set it in the ChatMessage
        //                    chatMessage.Image = image.Source;
        //                }
        //            }
        //        }
        //    }

        //    // Check if there's content to send
        //    if (!string.IsNullOrEmpty(chatMessage.Text) || chatMessage.Image != null)
        //    {
        //        chatMessages.Add(chatMessage); // Add the message to the ObservableCollection

        //        // Clear the MessageInput RichTextBox
        //        MessageInput.Document.Blocks.Clear();
        //        MessageInput.Document.Blocks.Add(new Paragraph());

        //        // Play the snap sound on a loop
        //        audio.PlayLooping();

        //        // Send chat history as an RPC
        //        Task.Run(async () =>
        //        {
        //            await SendChatHistoryAsync();
        //        });
        //    }
        //}

        private void MessageInput_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && Keyboard.Modifiers != ModifierKeys.Shift)
            {
                var chatMessage = new ChatMessage
                {
                    Role = ChatMessageRole.User,
                };

                // Iterate through the blocks in the RichTextBox
                foreach (Block block in MessageInput.Document.Blocks)
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

                // Send chat history as an RPC
                Trace.WriteLine("placeholder sending");

                //SendButton_Click(sender, new RoutedEventArgs());
                e.Handled = true; // Prevent the enter key from being further processed
            }
        }

        //protected override void OnSourceInitialized(EventArgs e)
        //{
        //    base.OnSourceInitialized(e);
        //    var helper = new WindowInteropHelper(this);
        //    RegisterHotKey(helper.Handle, HOTKEY_ID, MOD_CTRL | MOD_ALT, VK_T);

        //    ComponentDispatcher.ThreadFilterMessage += new ThreadMessageEventHandler(ComponentDispatcher_ThreadFilterMessage);
        //}

        //private void ComponentDispatcher_ThreadFilterMessage(ref MSG msg, ref bool handled)
        //{
        //    if (msg.message == WM_HOTKEY && (int)msg.wParam == HOTKEY_ID)
        //    {
        //        if (this.Visibility == Visibility.Visible)
        //        {
        //            // Close the window if it's already open
        //            this.Hide();
        //            chatMessages.Clear();
        //            _speechProcessor.Cancel();
        //            audio.Stop();
        //        }
        //        else
        //        {
        //            // Window is not visible, take a screenshot and open the window
        //            var currentElement = _axInspector.GetElementUnderCursor();
        //            var bounds = currentElement.Current.BoundingRectangle;
        //            var screenshot = ScreenshotUtil.CaptureArea((int)bounds.X, (int)bounds.Y, (int)bounds.Width, (int)bounds.Height);
        //            var imageSource = ScreenshotUtil.ConvertBitmapToImageSource(screenshot);

        //            // Set the captured information to the text field
        //            Dispatcher.Invoke(() => {
        //                // Clear the MessageInput RichTextBox
        //                MessageInput.Document.Blocks.Clear();

        //                InsertImageToRichTextBox(imageSource); // Insert the image
        //                MessageInput.Document.Blocks.Add(new Paragraph());
        //                MoveCursorToEnd(MessageInput);        // Move cursor to the end

        //                MessageInput.Focus(); // Set focus to the RichTextBox
        //            });

        //            // Now open the window
        //            this.Show();
        //            this.Activate(); // Brings window to front and gives it focus

        //            handled = true;
        //        }
        //    }
        //}

        private static string SerializeElementProperties(AutomationElement element)
        {
            if (element == null)
            {
                return "No element in focus";
            }

            var sb = new StringBuilder();

            // Basic Properties
            sb.AppendLine($"Name: {element.Current.Name}");
            sb.AppendLine($"Control Type: {element.Current.ControlType.ProgrammaticName}");
            sb.AppendLine($"Bounding Rectangle: {element.Current.BoundingRectangle}");

            // Additional Properties
            try
            {
                // FrameworkId - Type of framework (WPF, WinForms, etc.)
                sb.AppendLine($"Framework ID: {element.Current.FrameworkId}");

                // AutomationId - Unique within the same application
                sb.AppendLine($"Automation ID: {element.Current.AutomationId}");

                // ClassName - Underlying UI class name
                sb.AppendLine($"Class Name: {element.Current.ClassName}");

                // IsControlElement - Whether it's a control element
                sb.AppendLine($"Is Control Element: {element.Current.IsControlElement}");

                // IsContentElement - Whether it's a content element
                sb.AppendLine($"Is Content Element: {element.Current.IsContentElement}");

                // IsEnabled - Whether the control is enabled
                sb.AppendLine($"Is Enabled: {element.Current.IsEnabled}");

                // IsOffscreen - Whether the control is off-screen
                sb.AppendLine($"Is Offscreen: {element.Current.IsOffscreen}");

                // IsKeyboardFocusable - Whether the element can accept keyboard focus
                sb.AppendLine($"Is Keyboard Focusable: {element.Current.IsKeyboardFocusable}");

                // ProcessId - Associated process ID
                sb.AppendLine($"Process ID: {element.Current.ProcessId}");

                // Additional properties can be added as needed
            }
            catch (ElementNotAvailableException)
            {
                sb.AppendLine("Error: Element properties not available.");
            }

            return sb.ToString();
        }

        protected override void OnClosed(EventArgs e)
        {
            var helper = new WindowInteropHelper(this);
            UnregisterHotKey(helper.Handle, HOTKEY_ID);
            base.OnClosed(e);
        }

        //private void InsertImageToRichTextBox(ImageSource? image)
        //{
        //    if (image == null) return;

        //    var newImage = new Image()
        //    {
        //        Source = image,
        //        Width = image.Width,
        //        Height = image.Height,
        //    };

        //    var container = new InlineUIContainer(newImage);
        //    MessageInput.Document.Blocks.Add(new Paragraph(container));
        //}

        private void ChatMessages_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                Dispatcher.InvokeAsync(() =>
                {
                    ChatHistoryListView.ScrollIntoView(e.NewItems[0]);
                }, DispatcherPriority.Background);
            }
        }

        private void MoveCursorToEnd(RichTextBox richTextBox)
        {
            if (richTextBox.Document == null) return;

            richTextBox.CaretPosition = richTextBox.Document.ContentEnd;
            richTextBox.ScrollToEnd();
        }

        private async Task SendChatHistoryAsync()
        {
            // Cancel the previous stream if it exists
            streamCancellationTokenSource?.Cancel();
            streamCancellationTokenSource = new CancellationTokenSource();

            try
            {
                var completionResult = CreateCompletionAsStream(streamCancellationTokenSource.Token);
                audio.Stop();
                await foreach (var completion in completionResult)
                {
                    Trace.WriteLine(completion);
                    Dispatcher.Invoke(() =>
                    {
                        AppendToLastAssistantMessage(completion);
                    });
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
            }
        }

        private void AppendToLastAssistantMessage(ChatResponse response)
        {
            if (response.FinishReason != null)
            {
                // Check if the last message in the collection is an assistant message                
                if (chatMessages.Count > 0 && chatMessages.Last().Role == ChatMessageRole.Assistant)
                {
                    _speechProcessor.FlushBuffer();
                }
            } 
            else if (response.Text != null)
            {
                _speechProcessor.ProcessToken(response.Text);

                // Check if the last message in the collection is an assistant message
                if (chatMessages.Count > 0 && chatMessages.Last().Role == ChatMessageRole.Assistant)
                {
                    // Append the text to the last assistant message
                    chatMessages.Last().Text += response.Text;
                }
                else
                {
                    // If the last message is not an assistant message, create a new one
                    var chatMessage = new ChatMessage
                    {
                        Role = ChatMessageRole.Assistant,
                        Text = response.Text,
                    };
                    chatMessages.Add(chatMessage);
                }
            }
        }

        public async IAsyncEnumerable<ChatResponse> CreateCompletionAsStream(
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var uuid = _supabaseClient.Auth.CurrentUser?.Id ?? throw new InvalidOperationException("User is not authenticated");
            var requestData = new ChatRequest
            {
                Uuid = uuid,
                Messages = chatMessages.ToList(),
            };

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
            };

            using var response = client.PostAsStreamAsync(
                AppConfig.GetApiBaseUrl() + "/v5/wstream",
                requestData, 
                new JsonSerializerOptions
                {
                    Converters = { new ChatMessageJsonConverter() },
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
                    PropertyNameCaseInsensitive = true,
                    PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
                },
                cancellationToken
            );

            await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            using var reader = new StreamReader(stream);
            // Continuously read the stream until the end of it
            while (!reader.EndOfStream)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var line = await reader.ReadLineAsync(cancellationToken);

                // Skip empty lines
                if (string.IsNullOrEmpty(line))
                {
                    continue;
                }

                var responseObj = JsonSerializer.Deserialize<ChatResponse>(line, options: options);
                if (responseObj != null)
                {
                    yield return responseObj;
                }
            }
        }

        // Method to load content into RichTextBox when ViewModel property changes
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.Property.Name == nameof(ChatMessagesViewModel.ChatDraftRichTextXaml))
            {
                Trace.WriteLine("on property changed");
                var document = ChatMessagesViewModel.XamlToFlowDocument(_viewModel.ChatDraftRichTextXaml);
                MessageInput.Document = document;
            } 
            else
            {
                Trace.WriteLine("nothing changed");
            }
        }
    }
}