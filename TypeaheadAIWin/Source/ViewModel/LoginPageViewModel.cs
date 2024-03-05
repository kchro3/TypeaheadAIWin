using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using TypeaheadAIWin.Source.Service;

namespace TypeaheadAIWin.Source.ViewModel
{
    public partial class LoginPageViewModel : ObservableObject
    {
        private readonly Supabase.Client _supabaseClient;
        private readonly INavigationService _navigationService;

        [ObservableProperty]
        private string _email;

        public LoginPageViewModel(
            Supabase.Client supabaseClient,
            INavigationService navigationService)
        {
            _supabaseClient = supabaseClient;
            _navigationService = navigationService;
        }

        //[RelayCommand]
        //private async void SignInWithApple()
        //{
        //    var state = await _supabaseClient.Auth.SignIn(Supabase.Gotrue.Constants.Provider.Apple, new Supabase.Gotrue.SignInOptions
        //    {
        //        RedirectTo = "app.typeahead://login-callback"
        //    });

        //    Trace.WriteLine(state.Uri);

        //    Uri callbackUrl = await ForkedWebAuthenticator.AuthenticateAsync(
        //        state.Uri,
        //        new Uri("app.typeahead://login-callback")
        //    );

        //    var session = await _supabaseClient.Auth.GetSessionFromUrl(callbackUrl);
        //    SignInSuccessful?.Invoke(this, EventArgs.Empty);
        //}

        //[RelayCommand]
        //private async void SignInWithGoogle()
        //{
        //    var state = await _supabaseClient.Auth.SignIn(Supabase.Gotrue.Constants.Provider.Google, new Supabase.Gotrue.SignInOptions
        //    {
        //        RedirectTo = "app.typeahead://login-callback"
        //    });

        //    Trace.WriteLine(state.Uri);

        //    Uri callbackUrl = await ForkedWebAuthenticator.AuthenticateAsync(
        //        state.Uri,
        //        new Uri("app.typeahead://login-callback")
        //    );

        //    var session = await _supabaseClient.Auth.GetSessionFromUrl(callbackUrl);
        //    SignInSuccessful?.Invoke(this, EventArgs.Empty);
        //}

        [RelayCommand]
        private async void SignIn(object parameter)
        {
            var passwordBox = parameter as PasswordBox;
            var password = passwordBox?.Password;

            if (string.IsNullOrEmpty(_email) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter both email and password.", "Missing Information", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Use Supabase to sign in with email and password
                var response = await _supabaseClient.Auth.SignIn(_email, password);
                Trace.WriteLine(response);

                SignInSuccessful();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private async void Register(object parameter)
        {
            var passwordBox = parameter as PasswordBox;
            var password = passwordBox?.Password;

            if (string.IsNullOrEmpty(_email) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter both email and password.", "Missing Information", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Use Supabase to sign in with email and password
                var response = await _supabaseClient.Auth.SignUp(_email, password);
                Trace.WriteLine(response);

                SignInSuccessful();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Registration Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SignInSuccessful()
        {
            Trace.WriteLine("Successfully signed in.");
            _navigationService.NavigateTo<ChatPageViewModel>();
        }
    }
}
