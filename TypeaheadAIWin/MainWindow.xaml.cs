using H.Hooks;
using MahApps.Metro.Controls;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.Windows;
using TypeaheadAIWin.Source.PageView;
using TypeaheadAIWin.Source.ViewModel;
using Application = System.Windows.Application;

namespace TypeaheadAIWin
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private LowLevelKeyboardHook _lowLevelKeyboardHook;

        public MainWindow() {
            InitializeComponent();

            // Subscribe to the Closing event to prevent actual close
            this.Closing += (sender, args) =>
            {
                Trace.WriteLine("Hiding window");
                args.Cancel = true; // Cancel the close operation
                Hide(); // Hide the window instead
            };

            _lowLevelKeyboardHook = new LowLevelKeyboardHook();
            _lowLevelKeyboardHook.HandleModifierKeys = true;
            _lowLevelKeyboardHook.Handling = true;
            _lowLevelKeyboardHook.Up += LowLevelKeyboardHook_Up;
            _lowLevelKeyboardHook.Start();
        }

        /// <summary>
        /// Wrapper for opening the window properly.
        /// </summary>
        public void OpenWindow()
        {
            Show();
            WindowState = WindowState.Normal;
            Activate();
        }

        public void Toggle()
        {
            if (!IsVisible)
            {
                OpenWindow();
            } 
            else
            {
                Hide();
            }
        }

        private void LowLevelKeyboardHook_Up(object? sender, KeyboardEventArgs e)
        {
            // Activate Window hotkey
            if (e.Keys.Are(Key.Shift, Key.LeftWindows, Key.Space))
            {
                e.IsHandled = true;
                Application.Current.Dispatcher.Invoke(() => Toggle());
            }
            // New Window hotkey
            else if (e.Keys.Are(Key.Shift, Key.LeftWindows, Key.N))
            {
                e.IsHandled = true;
                Application.Current.Dispatcher.Invoke(() => 
                {
                    var chatPageViewModel = App.ServiceProvider.GetRequiredService<ChatPageViewModel>();
                    chatPageViewModel.Clear();
                    
                    if (!IsVisible)
                    {
                        OpenWindow();
                    }
                });
            }
            // Screenshot Window hotkey
            else if (e.Keys.Are(Key.Shift, Key.LeftWindows, Key.I))
            {
                e.IsHandled = true;
                Application.Current.Dispatcher.Invoke(() =>
                {
                    var chatPageViewModel = App.ServiceProvider.GetRequiredService<ChatPageViewModel>();
                    chatPageViewModel.Clear();
                    chatPageViewModel.TakeScreenshot();
                    
                    if (!IsVisible)
                    {
                        OpenWindow();
                    }
                });
            }
        }
    }
}