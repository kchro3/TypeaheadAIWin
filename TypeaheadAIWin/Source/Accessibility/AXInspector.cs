using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Automation;

namespace TypeaheadAIWin.Source.Accessibility
{
    public class AXInspector
    {
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetCursorPos(out POINT lpPoint);

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

        private void Subscribe()
        {
            Automation.AddAutomationFocusChangedEventHandler(OnFocusChanged);
        }

        private void OnFocusChanged(object sender, AutomationFocusChangedEventArgs e)
        {
            var element = sender as AutomationElement;
            if (element != null)
            {
                focusedElement = element;
            }
        }

        public void Unsubscribe()
        {
            Automation.RemoveAutomationFocusChangedEventHandler(OnFocusChanged);
        }
    }
}
