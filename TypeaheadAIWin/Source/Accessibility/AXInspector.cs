using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Automation;
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

        private AutomationElement focusedElement;

        public AXInspector() {
            focusedElement = AutomationElement.FocusedElement;
            Subscribe();
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

        public ApplicationContext GetCurrentAppContext()
        {
            IntPtr hWnd = GetForegroundWindow();
            GetWindowThreadProcessId(hWnd, out uint pid);

            using Process process = Process.GetProcessById((int)pid);

            ApplicationContext context = new ApplicationContext()
            {
                AppName = process.MainWindowTitle,
                ProcessName = process.ProcessName,
                Pid = pid
            };

            return context;
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
