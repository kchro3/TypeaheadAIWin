using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;
using TypeaheadAIWin.Source.Model;
using TypeaheadAIWin.Source.Service;

namespace TypeaheadAIWin.Source.ViewModel
{
    public partial class SettingsPageViewModel : ObservableObject
    {
        private readonly INavigationService _navigationService;

        [ObservableProperty]
        private UserDefaults _userDefaults;

        [ObservableProperty]
        private Array _cursorTypes;

        [ObservableProperty]
        private Array _typeaheadKeys;

        [ObservableProperty]
        private Array _promptRates;

        public SettingsPageViewModel(
            INavigationService navigationService,
            UserDefaults userDefaults)
        {
            _navigationService = navigationService;
            _userDefaults = userDefaults;
            _cursorTypes = Enum.GetValues(typeof(CursorType));
            _typeaheadKeys = Enum.GetValues(typeof(TypeaheadKey));
            _promptRates = Enum.GetValues(typeof(PromptRate));
        }

        [RelayCommand]
        private void NavigateToChat()
        {
            _navigationService.NavigateTo<ChatPageViewModel>();
        }
    }
}
