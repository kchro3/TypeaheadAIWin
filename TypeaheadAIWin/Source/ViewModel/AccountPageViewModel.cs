using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TypeaheadAIWin.Source.Service;

namespace TypeaheadAIWin.Source.ViewModel
{
    public partial class AccountPageViewModel : ObservableObject
    {
        private readonly Supabase.Client _supabaseClient;
        private readonly INavigationService _navigationService;

        public AccountPageViewModel(
            Supabase.Client supabaseClient,
            INavigationService navigationService)
        {
            _supabaseClient = supabaseClient;
            _navigationService = navigationService;
        }

        [RelayCommand]
        private void NavigateToChat()
        {
            _navigationService.NavigateTo<ChatPageViewModel>();
        }

        [RelayCommand]
        private async Task SignOut()
        {
            await _supabaseClient.Auth.SignOut();
            _navigationService.NavigateTo<LoginPageViewModel>();
        }
    }
}
