using CommunityToolkit.Mvvm.ComponentModel;
using MdXaml;
using System.Windows.Automation;
using System.Windows.Documents;
using System.Windows.Media;

namespace TypeaheadAIWin.Source.Model
{
    public partial class ChatMessage : ObservableObject
    {
        [ObservableProperty]
        private string _text;

        [ObservableProperty]
        private ImageSource _image;

        [ObservableProperty]
        private AutomationElement _focusedElement;

        private FlowDocument _markdownContent;

        [ObservableProperty]
        private ChatMessageRole _role;

        public FlowDocument MarkdownContent
        {
            get => _markdownContent;
            set => SetProperty(ref _markdownContent, value);
        }

        // Constructor
        public ChatMessage()
        {
            _text = string.Empty;
            _markdownContent = new FlowDocument();
        }

        partial void OnTextChanged(string value)
        {
            var md = new Markdown();
            // Replace all "\n" with "\n\n" in _text
            string updatedText = _text.Replace("\n", "\n\n");
            MarkdownContent = md.Transform(updatedText);
        }
    }
}
