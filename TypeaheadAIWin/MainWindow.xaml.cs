using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
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

namespace TypeaheadAIWin
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int WM_HOTKEY = 0x0312;
        private const int HOTKEY_ID = 9000;
        private const uint MOD_CTRL = 0x0002;
        private const uint MOD_ALT = 0x0001;
        private const uint VK_SPACE = 0x20;

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);


        private string lastSentMessage;
        private DispatcherTimer replyTimer;

        public MainWindow()
        {
            InitializeComponent();
 
            // Initialize the timer
            replyTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(3)
            };
            replyTimer.Tick += ReplyTimer_Tick;
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            string message = MessageInput.Text.Trim();

            if (!string.IsNullOrEmpty(message))
            {
                lastSentMessage = message; // Store the last sent message

                AXElementVisitor visitor = new AXElementVisitor();
                string elements = visitor.SerializeElements();

                ChatHistory.Text += "You: " + message + "\n" + elements + "\n";
                MessageInput.Text = string.Empty;

                // Scroll to the bottom to show the latest message
                ScrollViewer.ScrollToBottom();

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
            string reversedMessage = new string(lastSentMessage.Reverse().ToArray());

            // Append the fake reply to the chat history
            ChatHistory.Text += "Assistant: " + reversedMessage + "\n";

            // Scroll to the bottom to show the latest message
            ScrollViewer.ScrollToBottom();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            var helper = new WindowInteropHelper(this);
            RegisterHotKey(helper.Handle, HOTKEY_ID, MOD_CTRL | MOD_ALT, VK_SPACE);

            ComponentDispatcher.ThreadFilterMessage += new ThreadMessageEventHandler(ComponentDispatcher_ThreadFilterMessage);
        }

        private void ComponentDispatcher_ThreadFilterMessage(ref MSG msg, ref bool handled)
        {
            if (msg.message == WM_HOTKEY && (int)msg.wParam == HOTKEY_ID)
            {
                ToggleWindowVisibility();
            }
        }

        private void ToggleWindowVisibility()
        {
            if (this.Visibility == Visibility.Visible)
            {
                this.Hide();
            }
            else
            {
                this.Show();
                this.Activate(); // Brings window to front and gives it focus
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            var helper = new WindowInteropHelper(this);
            UnregisterHotKey(helper.Handle, HOTKEY_ID);
            base.OnClosed(e);
        }
    }
}