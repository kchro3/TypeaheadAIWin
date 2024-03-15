using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using TypeaheadAIWin.Source.ViewModel;

namespace TypeaheadAIWin.Source.PageView
{
    /// <summary>
    /// Interaction logic for ChatPageView.xaml
    /// </summary>
    public partial class ChatPageView : Page
    {
        public ChatPageView()
        {
            InitializeComponent();

            var chatPageViewModel = App.ServiceProvider.GetRequiredService<ChatPageViewModel>();
            chatPageViewModel.OnScreenshotTaken += ViewModel_OnScreenshotTaken;
        }

        private void MessageInput_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && Keyboard.Modifiers != ModifierKeys.Shift)
            {
                e.Handled = true;
                ((ChatPageViewModel)this.DataContext).Send(MessageInput);
            }
        }

        private void ViewModel_OnScreenshotTaken(object? sender, ImageSource e)
        {
            MessageInput.Document.Blocks.Clear();

            var newImage = new Image()
            {
                Source = e,
                Stretch = Stretch.None
            };
            AutomationProperties.SetName(newImage, "Screenshot of cursor");

            var container = new InlineUIContainer(newImage);
            MessageInput.Document.Blocks.Add(new Paragraph(container));

            // Add a new line
            MessageInput.Document.Blocks.Add(new Paragraph());

            MessageInput.CaretPosition = MessageInput.Document.ContentEnd;
            MessageInput.ScrollToEnd();
        }
    }
}
