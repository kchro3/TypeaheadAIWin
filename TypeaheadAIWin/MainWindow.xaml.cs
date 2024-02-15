﻿using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using TypeaheadAIWin.Source;
using MahApps.Metro.Controls;
using TypeaheadAIWin.Source.Accessibility;
using TypeaheadAIWin.Source.Speech;
using TypeaheadAIWin.Source.Model;
using CursorType = TypeaheadAIWin.Source.Model.CursorType;
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
        private const int NEW_HOTKEY_ID = 9001;
        private const uint MOD_CTRL = 0x0002; // Control key
        private const uint MOD_ALT = 0x0001;  // Alt key
        private const uint VK_N = 0x4E;       // N key
        private const uint VK_T = 0x54;       // T key

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private readonly ChatWindowViewModel _viewModel;
        private readonly AXInspector _axInspector;
        private readonly UserDefaults _userDefaults;

        private ApplicationContext appContext;

        public MainWindow(
            ChatWindowViewModel viewModel,
            Supabase.Client supabaseClient,
            AXInspector axInspector,
            ISpeechSynthesizerWrapper speechSynthesizerWrapper,
            StreamingSpeechProcessor speechProcessor,
            UserDefaults userDefaults
        ) {
            InitializeComponent();
            _viewModel = viewModel;
            _axInspector = axInspector;
            _userDefaults = userDefaults;

            appContext = _axInspector.GetCurrentAppContext();

            ChatHistoryListView.ItemsSource = _viewModel.ChatMessages;
            _viewModel.ChatMessages.CollectionChanged += ChatMessages_CollectionChanged;
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.Cancel();

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

            // Check if there's content to send
            if (!string.IsNullOrEmpty(chatMessage.Text) || chatMessage.Image != null)
            {
                _viewModel.SendMessage(chatMessage, appContext);

                // Clear the MessageInput RichTextBox
                MessageInput.Document.Blocks.Clear();
                MessageInput.Document.Blocks.Add(new Paragraph());
            }
        }

        private void MessageInput_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && Keyboard.Modifiers != ModifierKeys.Shift)
            {
                SendButton_Click(sender, new RoutedEventArgs());
                e.Handled = true; // Prevent the enter key from being further processed
            }
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            var helper = new WindowInteropHelper(this);
            RegisterHotKey(helper.Handle, HOTKEY_ID, MOD_CTRL | MOD_ALT, VK_T);
            RegisterHotKey(helper.Handle, NEW_HOTKEY_ID, MOD_CTRL | MOD_ALT, VK_N);

            ComponentDispatcher.ThreadFilterMessage += new ThreadMessageEventHandler(ComponentDispatcher_ThreadFilterMessage);
        }

        private void ComponentDispatcher_ThreadFilterMessage(ref MSG msg, ref bool handled)
        {
            if (msg.message == WM_HOTKEY)
            {
                // Handle Screenshot
                if ((int)msg.wParam == HOTKEY_ID)
                {
                    if (this.Visibility == Visibility.Visible)
                    {
                        // Close the window if it's already open
                        this.Hide();

                        _viewModel.Cancel();
                        _viewModel.Clear();
                    }
                    else
                    {
                        appContext = _axInspector.GetCurrentAppContext();
                        // Window is not visible, take a screenshot and open the window
                        var currentElement = _userDefaults.CursorType switch
                        {
                            CursorType.ScreenReader => _axInspector.GetFocusedElement(),
                            _ => _axInspector.GetElementUnderCursor()
                        };

                        var bounds = currentElement.Current.BoundingRectangle;
                        var screenshot = ScreenshotUtil.CaptureArea((int)bounds.X, (int)bounds.Y, (int)bounds.Width, (int)bounds.Height);
                        var imageSource = ScreenshotUtil.ConvertBitmapToImageSource(screenshot);

                        // Set the captured information to the text field
                        Dispatcher.Invoke(() =>
                        {
                            // Clear the MessageInput RichTextBox
                            MessageInput.Document.Blocks.Clear();

                            InsertImageToRichTextBox(imageSource); // Insert the image
                            MessageInput.Document.Blocks.Add(new Paragraph());
                            MoveCursorToEnd(MessageInput);        // Move cursor to the end

                            MessageInput.Focus(); // Set focus to the RichTextBox
                        });

                        // Now open the window
                        this.Show();
                        this.Activate(); // Brings window to front and gives it focus
                    }
                    handled = true;
                }
                else if ((int)msg.wParam == NEW_HOTKEY_ID)
                {
                    _viewModel.Cancel();
                    _viewModel.Clear();

                    // Clear the MessageInput RichTextBox
                    MessageInput.Document.Blocks.Clear();

                    if (this.Visibility != Visibility.Visible)
                    {
                        appContext = _axInspector.GetCurrentAppContext();
                        this.Show();
                    }

                    this.Activate();
                    MessageInput.Focus();
                    handled = true;
                }
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            var helper = new WindowInteropHelper(this);
            UnregisterHotKey(helper.Handle, HOTKEY_ID);
            UnregisterHotKey(helper.Handle, NEW_HOTKEY_ID);
            base.OnClosed(e);
        }

        private void InsertImageToRichTextBox(ImageSource? image)
        {
            if (image == null) return;

            var newImage = new Image()
            {
                Source = image,
                Stretch = Stretch.None
            };

            var container = new InlineUIContainer(newImage);
            MessageInput.Document.Blocks.Add(new Paragraph(container));
        }

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
    }
}