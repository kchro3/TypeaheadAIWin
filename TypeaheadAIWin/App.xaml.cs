using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using System.Media;
using System.Net.Http;
using System.Windows;
using TypeaheadAIWin.Source;
using TypeaheadAIWin.Source.Accessibility;
using TypeaheadAIWin.Source.Model;
using TypeaheadAIWin.Source.Service;
using TypeaheadAIWin.Source.Speech;
using TypeaheadAIWin.Source.ViewModel;
using TypeaheadAIWin.Source.Views;
using Forms = System.Windows.Forms;

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

        private readonly Forms.NotifyIcon _notifyIcon;
        private MainWindow _mainWindow;

        public App()
        {
            ServiceCollection services = new ServiceCollection();
            ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider();

            _notifyIcon = new Forms.NotifyIcon();
        }

        private void ConfigureServices(ServiceCollection services)
        {
            // Bind views
            services.AddSingleton(provider => new MainWindow
            {
                DataContext = provider.GetRequiredService<MainWindowViewModel>()
            });
            services.AddSingleton<MainWindowViewModel>();
            services.AddSingleton<ChatPageViewModel>();
            services.AddSingleton<LoginPageViewModel>();

            // View Model factory
            services.AddSingleton<Func<Type, ObservableObject>>(provider => viewModelType => (ObservableObject)provider.GetRequiredService(viewModelType));
            services.AddSingleton<MenuBar>();

            // Synchronously initialize Supabase client
            var supabaseClient = CreateSupabaseClientAsync().GetAwaiter().GetResult(); // This is a blocking call

            // Initialize the SoundPlayer
            var soundPlayer = new SoundPlayer(TypeaheadAIWin.Properties.Resources.snap);
            soundPlayer.Load();

            // Initalize the HTTP Client
            var httpClient = new HttpClient();
            httpClient.Timeout = TimeSpan.FromSeconds(10);

            // Bind singletons
            services.AddSingleton<AXInspector>();
            services.AddSingleton<CursorSettingsViewModel>();
            services.AddSingleton<ChatService>();
            services.AddSingleton(httpClient);
            services.AddSingleton<MenuBarViewModel>();
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<ISpeechSynthesizerWrapper, SpeechSynthesizerWrapper>();
            services.AddSingleton(soundPlayer);
            services.AddSingleton<SpeechSettingsViewModel>();
            services.AddSingleton<StreamingSpeechProcessor>();
            services.AddSingleton(supabaseClient);
            services.AddSingleton<UserDefaults>();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            _notifyIcon.Icon = new System.Drawing.Icon("Resources/typeahead.ico");
            _notifyIcon.Text = "Typeahead AI";
            _notifyIcon.Visible = true;
            _notifyIcon.Click += NotifyIcon_Click;

            this.ShutdownMode = ShutdownMode.OnExplicitShutdown;

            OpenMainWindow();
        }

        private void OpenMainWindow()
        {
            if (_mainWindow == null || !_mainWindow.IsLoaded)
            {
                _mainWindow = _serviceProvider.GetRequiredService<MainWindow>();

                // Subscribe to the Closing event to prevent actual close
                _mainWindow.Closing += (sender, args) =>
                {
                    args.Cancel = true; // Cancel the close operation
                    _mainWindow.Hide(); // Hide the window instead
                };
            }

            if (!_mainWindow.IsVisible)
            {
                _mainWindow.Show();
            }

            _mainWindow.WindowState = WindowState.Normal;
            _mainWindow.Activate();
        }

        private async Task<Supabase.Client> CreateSupabaseClientAsync()
        {
            var url = "https://hwkkvezmbrlrhvipbsum.supabase.co";
            var key = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6Imh3a2t2ZXptYnJscmh2aXBic3VtIiwicm9sZSI6ImFub24iLCJpYXQiOjE2OTgzNjY4NTEsImV4cCI6MjAxMzk0Mjg1MX0.aDzWW0p2uI7wsVGsu1mtfvEh4my8s9zhgVTr4r008YU";

            var options = new Supabase.SupabaseOptions
            {
                AutoRefreshToken = true,
                AutoConnectRealtime = true,
                SessionHandler = new SupabaseSessionHandler()
            };

            var supabase = new Supabase.Client(url, key, options);
            supabase.Auth.LoadSession();
            await supabase.InitializeAsync();
            return supabase;
        }

        private void NotifyIcon_Click(object sender, EventArgs e)
        {
            OpenMainWindow();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            _notifyIcon.Dispose();
        }
    }
}
