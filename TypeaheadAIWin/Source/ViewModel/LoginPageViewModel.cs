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
using System.Runtime.InteropServices;

namespace TypeaheadAIWin.Source.ViewModel
{
    public partial class LoginPageViewModel : ObservableObject
    {
        private readonly Supabase.Client _supabaseClient;
        private readonly HttpAuthServer _httpAuthServer;
        private readonly INavigationService _navigationService;
        private readonly MainWindowViewModel _mainWindowViewModel;

        [ObservableProperty]
        private string _email;

        public LoginPageViewModel(
            Supabase.Client supabaseClient,
            HttpAuthServer httpAuthServer,
            INavigationService navigationService,
            MainWindowViewModel mainWindowViewModel)
        {
            _supabaseClient = supabaseClient;
            _httpAuthServer = httpAuthServer;
            _navigationService = navigationService;
            _mainWindowViewModel = mainWindowViewModel;
        }

        [RelayCommand]
        private async void SignInWithApple()
        {
            var state = await _supabaseClient.Auth.SignIn(Supabase.Gotrue.Constants.Provider.Apple, new Supabase.Gotrue.SignInOptions
            {
                RedirectTo = HttpAuthServer.CallbackUrl + "login-callback",
                FlowType = Supabase.Gotrue.Constants.OAuthFlowType.PKCE
            });

            Trace.WriteLine(state.Uri);

            OpenUrlInBrowser(state.Uri.ToString());
            var authCode = await _httpAuthServer.StartAndWaitForAuthorizationAsync();
            
            try
            {
                var session = await _supabaseClient.Auth.ExchangeCodeForSession(state.PKCEVerifier, authCode);
                SignInSuccessful();
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Failed to login: " + ex.Message);
                MessageBox.Show("Failed to login...");
            }
        }

        [RelayCommand]
        private async void SignInWithGoogle()
        {
            var state = await _supabaseClient.Auth.SignIn(Supabase.Gotrue.Constants.Provider.Google, new Supabase.Gotrue.SignInOptions
            {
                RedirectTo = HttpAuthServer.CallbackUrl + "login-callback",
                FlowType = Supabase.Gotrue.Constants.OAuthFlowType.PKCE
            });

            Trace.WriteLine(state.Uri);

            OpenUrlInBrowser(state.Uri.ToString());
            var authCode = await _httpAuthServer.StartAndWaitForAuthorizationAsync();

            try
            {
                var session = await _supabaseClient.Auth.ExchangeCodeForSession(state.PKCEVerifier, authCode);
                SignInSuccessful();
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Failed to login: " + ex.Message);
                MessageBox.Show("Failed to login...");
            }
        }

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

            if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter both email and password.", "Missing Information", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Use Supabase to sign in with email and password
                var response = await _supabaseClient.Auth.SignUp(Email, password);
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
            _mainWindowViewModel.Activated = true;
        }

        private void OpenUrlInBrowser(string url)
        {
            try
            {
                // Universal method for .NET Core 3.1 and above, including .NET 5/6
                var psi = new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true // Necessary for .NET Core
                };
                Process.Start(psi);
            }
            catch (Exception ex)
            {
                // Handle any further exceptions or log them
                Trace.WriteLine($"Failed to open URL in browser: {ex.Message}");
            }
        }
    }
}
