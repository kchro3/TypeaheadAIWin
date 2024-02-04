using System.ComponentModel;
using System.Windows.Automation;
using System.Windows.Media;

namespace TypeaheadAIWin
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
        private ChatMessageRole _role;

        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                OnPropertyChanged(nameof(Text));
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

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
