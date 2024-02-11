using CommunityToolkit.Mvvm.ComponentModel;
using System.Speech.Synthesis;
using TypeaheadAIWin.Source.Speech;

namespace TypeaheadAIWin.Source.ViewModel
{
    public partial class SpeechSettingsViewModel: ObservableObject
    {
        private readonly ISpeechSynthesizerWrapper _speechSynthesizer;

        [ObservableProperty]
		private Array _promptRates;

        [ObservableProperty]
        private PromptRate _selectedPromptRate;

        public SpeechSettingsViewModel(ISpeechSynthesizerWrapper speechSynthesizer)
        {
            _speechSynthesizer = speechSynthesizer;
            PromptRates = Enum.GetValues(typeof(PromptRate));
            SelectedPromptRate = speechSynthesizer.PromptRate;
        }

        // This method is automatically called when SelectedPromptRate changes
        partial void OnSelectedPromptRateChanged(PromptRate value)
        {
            // Synchronize the change with the speech synthesizer
            _speechSynthesizer.PromptRate = value;
        }
    }
}
