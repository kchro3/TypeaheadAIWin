using System.Windows;
using TypeaheadAIWin.Source.ViewModel;

namespace TypeaheadAIWin.Views
{
    /// <summary>
    /// Interaction logic for SpeechSettingsWindow.xaml
    /// </summary>
    public partial class SpeechSettingsWindow : Window
    {
        public SpeechSettingsWindow(Window parent, SpeechSettingsViewModel viewModel)
        {
            Owner = parent;
            InitializeComponent();
            this.DataContext = viewModel;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
