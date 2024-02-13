using CommunityToolkit.Mvvm.ComponentModel;
using System.Speech.Synthesis;

namespace TypeaheadAIWin.Source.Model
{
    public partial class UserDefaults : ObservableObject
    {
        [ObservableProperty]
        private CursorType _cursorType;

        [ObservableProperty]
        private PromptRate _promptRate;

        public UserDefaults()
        {
            CursorType = (CursorType)Properties.Settings.Default.CursorType;
            PromptRate = (PromptRate)Properties.Settings.Default.PromptRate;
        }

        partial void OnCursorTypeChanged(CursorType value)
        {
            Properties.Settings.Default.CursorType = (int)value;
            Properties.Settings.Default.Save();
        }

        partial void OnPromptRateChanged(PromptRate value)
        {
            Properties.Settings.Default.PromptRate = (int)value;
            Properties.Settings.Default.Save();
        }
    }
}
