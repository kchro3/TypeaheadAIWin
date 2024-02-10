using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Windows;
using TypeaheadAIWin.Source;

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

            // Register Supabase client as a singleton
            services.AddSingleton(async serviceProvider =>
            {
                return await CreateSupabaseClientAsync();
            });

            // Since the above registration is async, it registers a Task<Supabase.Client>.
            // We register Supabase.Client to resolve it from the Task.
            services.AddSingleton<Supabase.Client>(serviceProvider =>
                serviceProvider.GetRequiredService<Task<Supabase.Client>>().Result);
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            Trace.WriteLine(TypeaheadAIWin.Properties.Settings.Default.Session);

            this.ShutdownMode = ShutdownMode.OnMainWindowClose;

            try
            {
                var supabaseClient = serviceProvider.GetService<Supabase.Client>()!;

                Trace.WriteLine(supabaseClient.Auth.CurrentUser);
                var mainWindow = serviceProvider.GetService<MainWindow>()!;
                var loginWindow = serviceProvider.GetService<LoginWindow>()!;
                var result = loginWindow.ShowDialog();

                if (result.HasValue && result.Value)
                {
                    loginWindow.Close();
                    mainWindow.Show();
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

        private async Task<Supabase.Client> CreateSupabaseClientAsync()
        {
            var url = "https://hwkkvezmbrlrhvipbsum.supabase.co";
            var key = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6Imh3a2t2ZXptYnJscmh2aXBic3VtIiwicm9sZSI6ImFub24iLCJpYXQiOjE2OTgzNjY4NTEsImV4cCI6MjAxMzk0Mjg1MX0.aDzWW0p2uI7wsVGsu1mtfvEh4my8s9zhgVTr4r008YU";

            var options = new Supabase.SupabaseOptions
            {
                AutoConnectRealtime = false,  // NOTE: It hangs when true, not sure why...
                SessionHandler = new SupabaseSessionHandler()
            };

            var supabase = new Supabase.Client(url, key, options);
            await supabase.InitializeAsync();
            return supabase;
        }
    }
}
