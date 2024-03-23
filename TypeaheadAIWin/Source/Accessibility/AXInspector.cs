using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Media;
using TypeaheadAIWin.Source.Components;
using TypeaheadAIWin.Source.Components.Accessibility;
using TypeaheadAIWin.Source.Model;

namespace TypeaheadAIWin.Source.Accessibility
{
    public class AXInspector
    {
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetCursorPos(out POINT lpPoint);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;
        }

        private readonly AXUIElementSerializer _axUIElementSerializer;
        private readonly Screenshotter _screenshotter;
        private readonly UserDefaults _userDefaults;
        private AutomationElement focusedElement;

        public AXInspector(
            AXUIElementSerializer axUIElementSerializer,
            Screenshotter screenshotter,
            UserDefaults userDefaults) 
        {
            _axUIElementSerializer = axUIElementSerializer;
            _screenshotter = screenshotter;
            _userDefaults = userDefaults;

            focusedElement = AutomationElement.FocusedElement;
            Subscribe();
        }

        public ImageSource? TakeScreenshot()
        {
            // Window is not visible, take a screenshot and open the window
            var currentElement = _userDefaults.CursorType switch
            {
                CursorType.ScreenReader => GetFocusedElement(),
                _ => GetElementUnderCursor()
            };

            var bounds = currentElement.Current.BoundingRectangle;
            var screenshot = _screenshotter.CaptureArea((int)bounds.X, (int)bounds.Y, (int)bounds.Width, (int)bounds.Height);
            var imageSource = _screenshotter.ConvertBitmapToImageSource(screenshot);
            return imageSource;
        }

        public AutomationElement GetElementUnderCursor()
        {
            var isSuccess = GetCursorPos(out POINT cursorPos);
            if (isSuccess)
            {
                Point pt = new Point(cursorPos.X, cursorPos.Y);

                // Get the Automation Element at the cursor position
                AutomationElement elementAtCursor = AutomationElement.FromPoint(pt);

                // Ensure an element was actually found
                if (elementAtCursor != null)
                {
                    return elementAtCursor;
                }
                else
                {
                    throw new InvalidOperationException("No UI element found under the cursor.");
                }
            }
            else
            {
                throw new InvalidOperationException("Failed to get cursor position.");
            }
        }

        public AutomationElement GetFocusedElement()
        {
            return focusedElement;
        }

        public async Task<ApplicationContext> GetCurrentAppContext()
        {
            IntPtr hWnd = GetForegroundWindow();
            GetWindowThreadProcessId(hWnd, out uint pid);

            using Process process = Process.GetProcessById((int)pid);

            Trace.WriteLine("serializing...");
            ApplicationContext context = new ApplicationContext()
            {
                AppName = process.MainWindowTitle,
                ProcessName = process.ProcessName,
                Pid = pid,
                SerializedUIElement = await _axUIElementSerializer.SerializeAsync(AutomationElement.FromHandle(hWnd))
            };
            Trace.WriteLine("done serializing!");

            return context;
        }

        private AutomationElement GetFocusedWindow()
        {
            var tmpElement = GetFocusedElement();
            while (true)
            {
                Trace.WriteLine("Getting parent");
                var walker = TreeWalker.ControlViewWalker;
                var parent = walker.GetParent(tmpElement);
                if (parent == null || parent == AutomationElement.RootElement) // RootElement represents the desktop
                {
                    break; // We've found the topmost window
                }
                tmpElement = parent;
            }

            return tmpElement;
        }

        public async Task<string> SerializeElementAsync(AutomationElement element)
        {
            return await _axUIElementSerializer.SerializeAsync(element);
        }

        private void Subscribe()
        {
            Automation.AddAutomationFocusChangedEventHandler(OnFocusChanged);
        }

        public void Unsubscribe()
        {
            Automation.RemoveAutomationFocusChangedEventHandler(OnFocusChanged);
        }

        private void OnFocusChanged(object sender, AutomationFocusChangedEventArgs e)
        {
            var element = sender as AutomationElement;
            if (element != null)
            {
                focusedElement = element;
            }
        }
    }
}
