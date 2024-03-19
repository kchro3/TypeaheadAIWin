using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Automation;
using System.Windows.Media;
using TypeaheadAIWin.Source.Model.Functions;

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
        private bool _isHidden = false;

        [ObservableProperty]
        private AutomationElement _focusedElement;

        [ObservableProperty]
        private List<FunctionCall> _functionCalls;

        [ObservableProperty]
        private string _inReplyToFunctionCallId;

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
