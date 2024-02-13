using CommunityToolkit.Mvvm.ComponentModel;
using System.Speech.Synthesis;
using System.Windows.Input;

namespace TypeaheadAIWin.Source.Model
{
    public partial class UserDefaults : ObservableObject
    {
        [ObservableProperty]
        private PromptRate _promptRate;

        [ObservableProperty]
        private CursorType _cursorType;

        public UserDefaults()
        {
            PromptRate = (PromptRate)Properties.Settings.Default.PromptRate;
            CursorType = (CursorType)Properties.Settings.Default.CursorType;
        }

        partial void OnPromptRateChanged(PromptRate value)
        {
            Properties.Settings.Default.PromptRate = (int)value;
            Properties.Settings.Default.Save();
        }

        partial void OnCursorTypeChanged(CursorType value)
        {
            Properties.Settings.Default.CursorType = (int)value;
            Properties.Settings.Default.Save();
        }
    }
}
