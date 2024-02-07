using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Windows;

namespace TypeaheadAIWin
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            this.ShutdownMode = ShutdownMode.OnMainWindowClose;

            try
            {
                var mainWindow = new MainWindow();
                var loginWindow = new LoginWindow();
                var result = loginWindow.ShowDialog();

                if (result.HasValue && result.Value)
                {
                    mainWindow.Show();
                }
                else
                {
                    // Optionally, provide feedback before shutting down
                    MessageBox.Show("Login failed or was cancelled. The application will now close.", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Information);
                    //this.Shutdown();
                }
            }
            catch (Exception ex)
            {
                // Log the exception or show an error message
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                //this.Shutdown();
            }
        }
    }
}
