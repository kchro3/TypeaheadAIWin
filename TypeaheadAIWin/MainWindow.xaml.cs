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
    }
}