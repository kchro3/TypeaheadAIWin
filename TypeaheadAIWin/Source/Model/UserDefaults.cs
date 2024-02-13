using CommunityToolkit.Mvvm.ComponentModel;
using System.Speech.Synthesis;

namespace TypeaheadAIWin.Source.Model
{
    public partial class UserDefaults : ObservableObject
    {
        [ObservableProperty]
        private PromptRate _promptRate;

        public UserDefaults()
        {
            PromptRate = (PromptRate)Properties.Settings.Default.PromptRate;
        }

        partial void OnPromptRateChanged(PromptRate value)
        {
            Properties.Settings.Default.PromptRate = (int)value;
            Properties.Settings.Default.Save();
        }
    }
}
