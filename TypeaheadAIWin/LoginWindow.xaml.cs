using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace TypeaheadAIWin
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private readonly Supabase.Client _supabaseClient;

        public LoginWindow(Supabase.Client supabaseClient)
        {
            Trace.WriteLine("Login initializing.");
            InitializeComponent();
            _supabaseClient = supabaseClient;
            Trace.WriteLine("Login initialized.");
        }

        private async void SignInButton_Click(object sender, RoutedEventArgs e)
        {
            string email = EmailTextBox.Text;
            string password = PasswordBox.Password; // Assuming you use a PasswordBox for password input

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter both email and password.", "Missing Information", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Use Supabase to sign in with email and password
                var response = await _supabaseClient.Auth.SignIn(email, password);
                MessageBox.Show("Login Successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                this.DialogResult = true; // Close the login dialog
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SignInWithGoogleButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Implement Google Sign-In Logic
            // For now, just closing the login window
            this.DialogResult = true;
        }

        private async void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            string email = EmailTextBox.Text; // Assuming you have a TextBox for email input
            string password = PasswordBox.Password; // Assuming you have a PasswordBox for password input

            // Basic validation
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Email and password are required.", "Missing Information", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Use Supabase to register a new user
                var response = await _supabaseClient.Auth.SignUp(email, password);
                MessageBox.Show("Registration Successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Registration Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
