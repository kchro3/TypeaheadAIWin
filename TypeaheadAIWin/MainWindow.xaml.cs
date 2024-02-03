using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Collections.ObjectModel;


namespace TypeaheadAIWin
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
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

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetCursorPos(out POINT lpPoint);

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;
        }

        private DispatcherTimer replyTimer;

        ObservableCollection<ChatMessage> chatMessages = new ObservableCollection<ChatMessage>();
        AutomationElement? currentElement = null;  // Current state of the message input

        public MainWindow()
        {
            InitializeComponent();
            ChatHistoryListView.ItemsSource = chatMessages;
            chatMessages.CollectionChanged += ChatMessages_CollectionChanged;

            // Initialize the timer
            replyTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(3)
            };
            replyTimer.Tick += ReplyTimer_Tick;
        }

        private void AddMessage(string text, ImageSource image = null)
        {
            chatMessages.Add(new ChatMessage { Text = text, Image = image });
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            var chatMessage = new ChatMessage();

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
                chatMessages.Add(chatMessage); // Add the message to the ObservableCollection

                // Clear the MessageInput RichTextBox
                MessageInput.Document.Blocks.Clear();
                MessageInput.Document.Blocks.Add(new Paragraph());

                // Reset and start the timer for the fake reply
                replyTimer.Stop();
                replyTimer.Start();
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

        private void ReplyTimer_Tick(object sender, EventArgs e)
        {
            // Stop the timer to prevent it from repeating
            replyTimer.Stop();

            // Reverse the last message sent
            // Append the fake reply to the chat history
            AddMessage("Assistant: ack");
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            var helper = new WindowInteropHelper(this);
            RegisterHotKey(helper.Handle, HOTKEY_ID, MOD_CTRL | MOD_ALT, VK_T);

            ComponentDispatcher.ThreadFilterMessage += new ThreadMessageEventHandler(ComponentDispatcher_ThreadFilterMessage);
        }

        private void ComponentDispatcher_ThreadFilterMessage(ref MSG msg, ref bool handled)
        {
            if (msg.message == WM_HOTKEY && (int)msg.wParam == HOTKEY_ID)
            {
                if (this.Visibility == Visibility.Visible)
                {
                    // Close the window if it's already open
                    this.Hide();
                }
                else
                {
                    // Window is not visible, take a screenshot and open the window
                    currentElement = GetElementUnderCursor();

                    var bounds = currentElement.Current.BoundingRectangle;
                    var screenshot = CaptureArea((int)bounds.X, (int)bounds.Y, (int)bounds.Width, (int)bounds.Height);
                    var imageSource = ConvertBitmapToImageSource(screenshot);

                    // Set the captured information to the text field
                    Dispatcher.Invoke(() => {
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
        }

        private string SerializeElementProperties(AutomationElement element)
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

        private AutomationElement GetElementUnderCursor()
        {
            GetCursorPos(out POINT cursorPos);

            // Convert the point to a System.Windows.Point
            System.Windows.Point pt = new System.Windows.Point(cursorPos.X, cursorPos.Y);

            // Get the Automation Element at the cursor position
            AutomationElement elementAtCursor = AutomationElement.FromPoint(pt);

            return elementAtCursor;
        }

        public Bitmap CaptureArea(int x, int y, int width, int height)
        {
            if (width <= 0 || height <= 0)
            {
                throw new ArgumentException("Capture area dimensions must be greater than zero.");
            }

            try
            {
                Bitmap bmp = new Bitmap(width, height);
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.CopyFromScreen(x, y, 0, 0, new System.Drawing.Size(width, height), CopyPixelOperation.SourceCopy);
                }
                return bmp;
            }
            catch (Exception ex)
            {
                // Handle the exception (e.g., log the details)
                Console.WriteLine("Error capturing screen area: " + ex.Message);
                return null;
            }
        }

        private ImageSource ConvertBitmapToImageSource(Bitmap bitmap)
        {
            if (bitmap == null)
            {
                throw new ArgumentNullException(nameof(bitmap), "Bitmap cannot be null.");
            }

            try
            {
                using (var memory = new MemoryStream())
                {
                    bitmap.Save(memory, ImageFormat.Png);
                    memory.Position = 0;

                    var bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = memory;
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.EndInit();

                    return bitmapImage;
                }
            }
            catch (Exception ex)
            {
                // Handle the exception (e.g., log the details)
                Console.WriteLine("Error converting bitmap to ImageSource: " + ex.Message);
                return null;
            }
        }

        private void InsertImageToRichTextBox(ImageSource image)
        {
            var newImage = new System.Windows.Controls.Image();
            newImage.Source = image;

            var container = new InlineUIContainer(newImage);
            MessageInput.Document.Blocks.Add(new Paragraph(container));
        }

        private void AppendTextToRichTextBox(string text)
        {
            var paragraph = new Paragraph(new Run(text));
            MessageInput.Document.Blocks.Add(paragraph);
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