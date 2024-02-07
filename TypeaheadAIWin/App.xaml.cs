using Microsoft.Extensions.DependencyInjection;
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
        private ServiceProvider serviceProvider;

        public App()
        {
            ServiceCollection services = new ServiceCollection();
            ConfigureServices(services);
            serviceProvider = services.BuildServiceProvider();
        }

        private void ConfigureServices(ServiceCollection services)
        {
            services.AddSingleton<MainWindow>();
            services.AddSingleton<LoginWindow>();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            this.ShutdownMode = ShutdownMode.OnMainWindowClose;

            try
            {
                var mainWindow = serviceProvider.GetService<MainWindow>()!;
                var loginWindow = serviceProvider.GetService<LoginWindow>()!;
                var result = loginWindow.ShowDialog();

                if (result.HasValue && result.Value)
                {
                    loginWindow.Close();
                }
                else
                {
                    // Optionally, provide feedback before shutting down
                    MessageBox.Show("Login failed or was cancelled. The application will now close.", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Shutdown();
                }
            }
            catch (Exception ex)
            {
                // Log the exception or show an error message
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Shutdown();
            }
        }
    }
}
