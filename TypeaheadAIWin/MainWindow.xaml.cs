using MahApps.Metro.Controls;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;

namespace TypeaheadAIWin
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow() {
            InitializeComponent();

            // Subscribe to the Closing event to prevent actual close
            this.Closing += (sender, args) =>
            {
                Trace.WriteLine("Hiding window");
                args.Cancel = true; // Cancel the close operation
                Hide(); // Hide the window instead
            };
        }

        /// <summary>
        /// Keep in the back pocket for now.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        private static AutomationElement GetRootWindow(AutomationElement element)
        {
            while (true)
            {
                var walker = TreeWalker.ControlViewWalker;
                var parent = walker.GetParent(element);
                if (parent == null || parent == AutomationElement.RootElement) // RootElement represents the desktop
                {
                    break; // We've found the topmost window
                }
                element = parent;
            }

            return element;
        }
    }
}