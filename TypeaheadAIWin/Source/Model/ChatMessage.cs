using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Automation;
using System.Windows.Media;

namespace TypeaheadAIWin.Source.Model
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
        private Guid _id;

        [ObservableProperty]
        private Guid _rootId;
        
        [ObservableProperty]
        private Guid? _inReplyToId;

        [ObservableProperty]
        private string _text;

        [ObservableProperty]
        private ImageSource _image;

        [ObservableProperty]
        private AutomationElement _focusedElement;

        [ObservableProperty]
        private ChatMessageRole _role;

        // Constructor
        public ChatMessage(Guid? rootId = null, Guid? inReplyToId = null)
        {
            _id = Guid.NewGuid();
            _rootId = rootId ?? _id;
            _inReplyToId = inReplyToId;
            _text = string.Empty;
        }
    }
}
