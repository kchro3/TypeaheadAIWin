using H.Hooks;
using MahApps.Metro.Controls;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using TypeaheadAIWin.Source.Accessibility;
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
            if ((e.Keys.Are(Key.Shift, Key.LeftWindows, Key.Space) && _userDefaults.TypeaheadKey == TypeaheadKey.ShiftLeftWindow) ||
                (e.Keys.Are(Key.Shift, Key.Insert, Key.Space) && _userDefaults.TypeaheadKey == TypeaheadKey.ShiftInsert) ||
                (e.Keys.Are(Key.Shift, Key.CapsLock, Key.Space) && _userDefaults.TypeaheadKey == TypeaheadKey.ShiftCapsLock))
            {
                e.IsHandled = true;
                Application.Current.Dispatcher.Invoke(() => {
                    PrintAppState();
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
                        PrintAppState();
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
        }

        private void PrintAppState()
        {
            var axInspector = App.ServiceProvider.GetRequiredService<AXInspector>();
            var appContext = axInspector.GetCurrentAppContext();
            var focusedElement = axInspector.GetFocusedElement();

            AutomationElement rootWindow = GetRootWindow(focusedElement);
            PrintTree(rootWindow, 0);
        }

        static void PrintTree(AutomationElement element, int level)
        {
            string indent = new string(' ', level * 2);

            string name = element.GetCurrentPropertyValue(AutomationElement.NameProperty) as string;
            if (string.IsNullOrEmpty(name))
                return; // Skip unnamed elements to reduce clutter

            string controlType = element.GetCurrentPropertyValue(AutomationElement.ControlTypeProperty) as ControlType;
            string automationId = element.GetCurrentPropertyValue(AutomationElement.AutomationIdProperty) as string;

            // Attempt to get the value pattern and its current value
            object valuePatternObj = null;
            string valuePattern = string.Empty;
            if (element.TryGetCurrentPattern(ValuePattern.Pattern, out valuePatternObj))
            {
                ValuePattern valPattern = valuePatternObj as ValuePattern;
                valuePattern = $"Value: {valPattern.Current.Value}";
            }

            Console.WriteLine($"{indent}Name: {name}, Automation ID: {automationId}, {valuePattern}");

            TreeWalker walker = TreeWalker.ControlViewWalker;
            AutomationElement child = walker.GetFirstChild(element);
            while (child != null)
            {
                PrintTree(child, level + 1);
                child = walker.GetNextSibling(child);
            }
        }

        private static AutomationElement GetRootWindow(AutomationElement element)
        {
            // Traverse up the tree to find the root window
            AutomationElement parent;
            AutomationElement currentElement = element;
            do
            {
                parent = TreeWalker.RawViewWalker.GetParent(currentElement);
                if (parent == null)
                {
                    // Current element is the root
                    return currentElement;
                }
                currentElement = parent;
            } while (true);
        }
    }
}