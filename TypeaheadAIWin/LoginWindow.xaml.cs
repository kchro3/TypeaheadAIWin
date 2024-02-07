using System.Windows;
using System.Windows.Controls;

namespace TypeaheadAIWin
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
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
                bool isAuthenticated = true;
                //bool isAuthenticated = await AuthenticateUser(email, password);
                if (isAuthenticated)
                {
                    this.DialogResult = true;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Invalid email or password.", "Authentication Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SignInWithGoogleButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Implement Google Sign-In Logic
            // For now, just closing the login window
            this.DialogResult = true;
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Navigate to Registration Screen or Logic
            // For now, showing a simple message
            StatusTextBlock.Text = "Registration functionality not implemented yet.";
        }
    }
}
