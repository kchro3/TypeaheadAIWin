using CommunityToolkit.Mvvm.ComponentModel;
using MdXaml;
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

    public partial class ChatMessage : ObservableObject
    {
        [ObservableProperty]
        private string _text;

        [ObservableProperty]
        private ImageSource _image;

        [ObservableProperty]
        private AutomationElement _focusedElement;

        [ObservableProperty]
        private ChatMessageRole _role;

        // Constructor
        public ChatMessage()
        {
            _text = string.Empty;
        }
    }
}
