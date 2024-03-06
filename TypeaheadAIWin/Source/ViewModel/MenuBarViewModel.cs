using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;
using TypeaheadAIWin.Source.Service;

namespace TypeaheadAIWin.Source.ViewModel
{
    public partial class MenuBarViewModel: ObservableObject
    {
        private readonly INavigationService _navigationService;
        private readonly ChatPageViewModel _chatWindowViewModel;

        public MenuBarViewModel(
            ChatPageViewModel chatWindowViewModel,
            INavigationService navigationService) {
            _chatWindowViewModel = chatWindowViewModel;
            _navigationService = navigationService;
        }

        [RelayCommand]
        private void New()
        {
            _chatWindowViewModel.Clear();
        }

        [RelayCommand]
        private void Exit()
        {
            // Implement the action for the Exit command
            Application.Current.Shutdown();
        }

        [RelayCommand]
        private void General()
        {
            _navigationService.NavigateTo<SettingsPageViewModel>();
        }

        [RelayCommand]
        private void Account()
        {
            _navigationService.NavigateTo<AccountPageViewModel>();
        }

        [RelayCommand]
        private void About()
        {
            // Implement the action for the About command
            MessageBox.Show("Not implemented yet.");
        }
    }
}
