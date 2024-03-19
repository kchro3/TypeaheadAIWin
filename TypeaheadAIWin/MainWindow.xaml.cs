using H.Hooks;
using MahApps.Metro.Controls;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using TypeaheadAIWin.Source.Accessibility;
using TypeaheadAIWin.Source.Components;
using TypeaheadAIWin.Source.Components.Accessibility;
using TypeaheadAIWin.Source.Model;
using TypeaheadAIWin.Source.ViewModel;
using Application = System.Windows.Application;

namespace TypeaheadAIWin
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private readonly UserDefaults _userDefaults;
        private LowLevelKeyboardHook _lowLevelKeyboardHook;

        public MainWindow() {
            InitializeComponent();

            _userDefaults = App.ServiceProvider.GetRequiredService<UserDefaults>();

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
            _lowLevelKeyboardHook.IsCapsLock = true;
            _lowLevelKeyboardHook.Down += LowLevelKeyboardHook_Down;
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

        private void LowLevelKeyboardHook_Down(object? sender, KeyboardEventArgs e)
        {
            // Activate Window hotkey
            if ((e.Keys.Are(Key.Shift, Key.LeftWindows, Key.Space) && _userDefaults.TypeaheadKey == TypeaheadKey.ShiftLeftWindow) ||
                (e.Keys.Are(Key.Shift, Key.Insert, Key.Space) && _userDefaults.TypeaheadKey == TypeaheadKey.ShiftInsert) ||
                (e.Keys.Are(Key.Shift, Key.CapsLock, Key.Space) && _userDefaults.TypeaheadKey == TypeaheadKey.ShiftCapsLock))
            {
                e.IsHandled = true;
                Application.Current.Dispatcher.Invoke(() => {
                    Toggle();
                });
            }
            // New Window hotkey
            else if ((e.Keys.Are(Key.Shift, Key.LeftWindows, Key.N) && _userDefaults.TypeaheadKey == TypeaheadKey.ShiftLeftWindow) ||
                     (e.Keys.Are(Key.Shift, Key.Insert, Key.N) && _userDefaults.TypeaheadKey == TypeaheadKey.ShiftInsert) ||
                     (e.Keys.Are(Key.Shift, Key.CapsLock, Key.N) && _userDefaults.TypeaheadKey == TypeaheadKey.ShiftCapsLock))
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
            else if ((e.Keys.Are(Key.Shift, Key.LeftWindows, Key.I) && _userDefaults.TypeaheadKey == TypeaheadKey.ShiftLeftWindow) ||
                     (e.Keys.Are(Key.Shift, Key.Insert, Key.I) && _userDefaults.TypeaheadKey == TypeaheadKey.ShiftInsert) ||
                     (e.Keys.Are(Key.Shift, Key.CapsLock, Key.I) && _userDefaults.TypeaheadKey == TypeaheadKey.ShiftCapsLock))
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
            // Cancel speaking
            else if (e.Keys.Are(Key.LeftWindows, Key.Escape))
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    var chatPageViewModel = App.ServiceProvider.GetRequiredService<ChatPageViewModel>();
                    chatPageViewModel.Cancel();
                });
            }
        }

        private void LowLevelKeyboardHook_Up(object? sender, KeyboardEventArgs e)
        {
            // Activate Window hotkey
            if ((e.Keys.Are(Key.Shift, Key.LeftWindows, Key.Space) && _userDefaults.TypeaheadKey == TypeaheadKey.ShiftLeftWindow) ||
                (e.Keys.Are(Key.Shift, Key.Insert, Key.Space) && _userDefaults.TypeaheadKey == TypeaheadKey.ShiftInsert) ||
                (e.Keys.Are(Key.Shift, Key.CapsLock, Key.Space) && _userDefaults.TypeaheadKey == TypeaheadKey.ShiftCapsLock))
            {
                e.IsHandled = true;
            }
            // New Window hotkey
            else if ((e.Keys.Are(Key.Shift, Key.LeftWindows, Key.N) && _userDefaults.TypeaheadKey == TypeaheadKey.ShiftLeftWindow) ||
                     (e.Keys.Are(Key.Shift, Key.Insert, Key.N) && _userDefaults.TypeaheadKey == TypeaheadKey.ShiftInsert) ||
                     (e.Keys.Are(Key.Shift, Key.CapsLock, Key.N) && _userDefaults.TypeaheadKey == TypeaheadKey.ShiftCapsLock))
            {
                e.IsHandled = true;
            }
            // Screenshot Window hotkey
            else if ((e.Keys.Are(Key.Shift, Key.LeftWindows, Key.I) && _userDefaults.TypeaheadKey == TypeaheadKey.ShiftLeftWindow) ||
                     (e.Keys.Are(Key.Shift, Key.Insert, Key.I) && _userDefaults.TypeaheadKey == TypeaheadKey.ShiftInsert) ||
                     (e.Keys.Are(Key.Shift, Key.CapsLock, Key.I) && _userDefaults.TypeaheadKey == TypeaheadKey.ShiftCapsLock))
            {
                e.IsHandled = true;
            }
        }
    }
}