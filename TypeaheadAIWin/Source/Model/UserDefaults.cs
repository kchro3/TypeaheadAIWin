using CommunityToolkit.Mvvm.ComponentModel;
using System.Speech.Synthesis;

namespace TypeaheadAIWin.Source.Model
{
    public partial class UserDefaults : ObservableObject
    {
        [ObservableProperty]
        private PromptRate _promptRate;

        [ObservableProperty]
        private CursorType _cursorType;

        [ObservableProperty]
        private TypeaheadKey _typeaheadKey;

        public UserDefaults()
        {
            PromptRate = (PromptRate)Properties.Settings.Default.PromptRate;
            CursorType = (CursorType)Properties.Settings.Default.CursorType;
            TypeaheadKey = (TypeaheadKey)Properties.Settings.Default.TypeaheadKey;
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

        partial void OnTypeaheadKeyChanged(TypeaheadKey value)
        {
            Properties.Settings.Default.TypeaheadKey = (int)value;
            Properties.Settings.Default.Save();
        }
    }
}
