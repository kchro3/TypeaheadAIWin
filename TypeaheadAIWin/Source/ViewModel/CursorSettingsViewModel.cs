using CommunityToolkit.Mvvm.ComponentModel;
using TypeaheadAIWin.Source.Model;

namespace TypeaheadAIWin.Source.ViewModel
{
    public partial class CursorSettingsViewModel : ObservableObject
    {
        public UserDefaults UserDefaults { get; }

        [ObservableProperty]
        private Array _cursorTypes;

        public CursorSettingsViewModel(UserDefaults userDefaults)
        {
            UserDefaults = userDefaults;
            CursorTypes = Enum.GetValues(typeof(CursorType));
        }
    }
}
