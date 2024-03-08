using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using H.Hooks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TypeaheadAIWin.Source.Service;

namespace TypeaheadAIWin.Source.ViewModel
{
    public partial class MainWindowViewModel : ObservableObject
    {
        private readonly Supabase.Client _supabaseClient;

        [ObservableProperty]
        private bool _activated;

        [ObservableProperty]
        private INavigationService _navigationService;

        public MainWindowViewModel(
            Supabase.Client supabaseClient,
            INavigationService navigationService)
        {
            _supabaseClient = supabaseClient;
            _navigationService = navigationService;

            if (_supabaseClient.Auth.CurrentSession != null)
            {
                NavigateToChatPage();
            }
            else
            {
                NavigateToLoginPage();
            }
        }

        [RelayCommand]
        private void NavigateToLoginPage()
        {
            NavigationService.NavigateTo<LoginPageViewModel>();
        }

        [RelayCommand]
        private void NavigateToChatPage()
        {
            NavigationService.NavigateTo<ChatPageViewModel>();
        }
    }
}
