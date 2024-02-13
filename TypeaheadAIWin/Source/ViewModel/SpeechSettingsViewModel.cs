﻿using CommunityToolkit.Mvvm.ComponentModel;
using System.Speech.Synthesis;
using TypeaheadAIWin.Source.Model;

namespace TypeaheadAIWin.Source.ViewModel
{
    public partial class SpeechSettingsViewModel: ObservableObject
    {
        public UserDefaults UserDefaults;

        [ObservableProperty]
        private Array _promptRates;

        public SpeechSettingsViewModel(UserDefaults userDefaults)
        {
            UserDefaults = userDefaults;
            PromptRates = Enum.GetValues(typeof(PromptRate));
        }
    }
}
