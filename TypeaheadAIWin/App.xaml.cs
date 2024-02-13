using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Windows;
using TypeaheadAIWin.Source;
using TypeaheadAIWin.Source.Accessibility;
using TypeaheadAIWin.Source.Model;
using TypeaheadAIWin.Source.Speech;
using TypeaheadAIWin.Source.ViewModel;
using TypeaheadAIWin.Source.Views;

namespace TypeaheadAIWin
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private ServiceProvider _serviceProvider;
        public static ServiceProvider ServiceProvider
        {
            get
            {
                if (Current is App app)
                {
                    return app._serviceProvider;
                }

                return null;
            }
        }

        public App()
        {
            ServiceCollection services = new ServiceCollection();
            ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider();
        }

        private void ConfigureServices(ServiceCollection services)
        {
            // Bind views
            services.AddSingleton<MainWindow>();
            services.AddSingleton<LoginWindow>();
            services.AddSingleton<MenuBar>();

            // Synchronously initialize Supabase client
            var supabaseClient = CreateSupabaseClientAsync().GetAwaiter().GetResult(); // This is a blocking call

            // Bind singletons
            services.AddSingleton<AXInspector>();
            services.AddSingleton<MenuBarViewModel>();
            services.AddSingleton<SpeechSettingsViewModel>();
            services.AddSingleton<StreamingSpeechProcessor>();
            services.AddSingleton(supabaseClient);
            services.AddSingleton<UserDefaults>();

            // Bind scoped
            services.AddScoped<ISpeechSynthesizerWrapper, SpeechSynthesizerWrapper>();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            this.ShutdownMode = ShutdownMode.OnMainWindowClose;

            var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();

            // Check if the user is already signed in.
            var supabaseClient = _serviceProvider.GetRequiredService<Supabase.Client>();
            var session = await supabaseClient.Auth.RetrieveSessionAsync();

            if (session == null)
            {
                Trace.WriteLine("User is not signed in");
                try
                {
                    var loginWindow = _serviceProvider.GetRequiredService<LoginWindow>();
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
            else
            {
                Trace.WriteLine("User is logged in.");
                mainWindow.Show();
            }
        }

        private async Task<Supabase.Client> CreateSupabaseClientAsync()
        {
            var url = "https://hwkkvezmbrlrhvipbsum.supabase.co";
            var key = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6Imh3a2t2ZXptYnJscmh2aXBic3VtIiwicm9sZSI6ImFub24iLCJpYXQiOjE2OTgzNjY4NTEsImV4cCI6MjAxMzk0Mjg1MX0.aDzWW0p2uI7wsVGsu1mtfvEh4my8s9zhgVTr4r008YU";

            var options = new Supabase.SupabaseOptions
            {
                AutoConnectRealtime = true,
                SessionHandler = new SupabaseSessionHandler()
            };

            var supabase = new Supabase.Client(url, key, options);
            supabase.Auth.LoadSession();
            await supabase.InitializeAsync();
            return supabase;
        }
    }
}
