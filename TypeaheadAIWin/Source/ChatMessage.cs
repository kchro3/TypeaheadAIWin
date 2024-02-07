using MdXaml;
using System.ComponentModel;
using System.Windows.Automation;
using System.Windows.Documents;
using System.Windows.Media;

namespace TypeaheadAIWin.Source
{
    public enum ChatMessageRole
    {
        User,
        Tool,
        Assistant
    }

    public class ChatMessage : INotifyPropertyChanged
    {
        private string _text;
        private ImageSource _image;
        private AutomationElement _focusedElement;
        private FlowDocument _markdownContent;
        private ChatMessageRole _role;

        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                OnPropertyChanged(nameof(Text));
                UpdateMarkdownContent(); // Synchronize FlowDocument with Text
            }
        }

        public FlowDocument MarkdownContent
        {
            get => _markdownContent;
            set
            {
                _markdownContent = value;
                OnPropertyChanged(nameof(MarkdownContent));
            }
        }

        public ImageSource Image
        {
            get => _image;
            set
            {
                _image = value;
                OnPropertyChanged(nameof(Image));
            }
        }

        public AutomationElement FocusedElement
        {
            get => _focusedElement;
            set
            {
                _focusedElement = value;
                OnPropertyChanged(nameof(FocusedElement));
            }
        }

        public ChatMessageRole Role
        {
            get => _role;
            set
            {
                _role = value;
                OnPropertyChanged(nameof(Role));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Call this method to update MarkdownContent whenever Text changes
        private void UpdateMarkdownContent()
        {
            var md = new Markdown();

            // Replace all "\n" with "\n\n" in _text
            string updatedText = _text.Replace("\n", "\n\n");

            MarkdownContent = md.Transform(updatedText);
        }
    }
}
